using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lab4;

public class Contacts
{
    public string Address;
    public string Phone;
}

public class WebScanner : IDisposable
{
    private readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
    private readonly HashSet<Uri> _visited = new HashSet<Uri>();
    private readonly HashSet<string> _ignoredExt = new HashSet<string>
    {
        ".ico", ".xml"
    };

    private int _maxLevel, _maxPages, _pages;
    private string _baseDomain;

    private static readonly Regex LinkRx = new Regex(
        @"<a\s[^>]*?href=""(?<url>[^""#]+)""[^>]*>(?<text>.*?)</a>",
        RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex ContentRx = new Regex(
        @"<div\s[^>]*class=""[^""]*\bregion-content\b[^""]*""[^>]*>(?<c>.*?)" +
        @"(?=<div\s[^>]*class=""[^""]*\bregion-(?:sidebar-second|footer)\b|</body>)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TagRx = new Regex("<[^>]+>", RegexOptions.Compiled);

    // +7 (351) 267-99-00 | 8-800-100-20-30 | 8 351 267 99 00 | (351) 267-91-71
    private static readonly Regex PhoneRx = new Regex(
        @"(?<!\d)(?:(?:\+7|8)[\s\-]*\(?\d{3}\)?|\(\d{3}\))[\s\-]*\d{3}[\s\-]*\d{2}[\s\-]*\d{2}(?!\d)",
        RegexOptions.Compiled);

    private const string Street =
        @"(?i:улица|ул\.|проспект|просп\.|пр-кт|пр-т|пр\.|шоссе|переулок|пер\.|бульвар|б-р|проезд|площадь|пл\.|набережная|наб\.)";

    // «454080 Челябинск, проспект Ленина, 76», «ул. Сони Кривой, 60», «Комсомольский проспект, 84»
    private static readonly Regex AddressRx = new Regex(
        @"(?:\d{6},?\s*(?:г\.\s*)?[А-ЯЁ][а-яё\-]+,\s*)?" +
        @"(?:" + Street + @"\s*[А-ЯЁ][а-яёА-ЯЁ\s\.\-]{1,40}?" +
        @"|[А-ЯЁ][а-яё\-]+\s+(?i:проспект|бульвар|переулок|проезд)" +
        @")[,\s]+(?:д\.\s*)?\d{1,4}[а-я]?(?:\s*[/\-]\s*\d{1,4})?",
        RegexOptions.Compiled);
    public event Action<Uri, string, int, Contacts> TargetFound;


    public WebScanner()
    {
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (X11; Linux x86_64; rv:120.0) Gecko/20100101 Firefox/120.0");
    }

    public async Task ScanAsync(Uri start, int maxPages, int maxLevel)
    {
        _visited.Clear();
        _pages = 0;
        _maxPages = maxPages;
        _maxLevel = maxLevel;
        _baseDomain = start.Host.ToLowerInvariant().Replace("www.", "");

        await ScanLevelAsync(new List<(Uri, string)> { (start, "Стартовая страница") }, 0);
    }

    private async Task ScanLevelAsync(List<(Uri Page, string Title)> pages, int level)
    {
        if (pages.Count == 0 || level > _maxLevel) return;

        var next = new List<(Uri, string)>();
        foreach (var (page, title) in pages)
        {
            if (_pages >= _maxPages) break;
            next.AddRange(await ProcessPageAsync(page, title, level));
        }

        await ScanLevelAsync(next, level + 1);
    }

    private async Task<List<(Uri, string)>> ProcessPageAsync(Uri page, string title, int level)
    {
        var children = new List<(Uri, string)>();
        if (_visited.Contains(page)) return children;
        _visited.Add(page);
        _pages++;

        string html;
        try { html = await _http.GetStringAsync(page); }
        catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException)
        {
            return children;
        }

        var m = ContentRx.Match(html);
        string content = m.Success && m.Groups["c"].Length > 200 ? m.Groups["c"].Value : html;

        string text = WebUtility.HtmlDecode(TagRx.Replace(content, " "))
            .Replace("\u00AD", "").Replace("\u200B", "");

        foreach (var phone in Find(PhoneRx, text))
            TargetFound?.Invoke(page, title, level, new Contacts { Phone = phone });

        foreach (var address in Find(AddressRx, text))
            TargetFound?.Invoke(page, title, level, new Contacts { Address = address });

        foreach (Match a in LinkRx.Matches(content))
        {
            if (!Uri.TryCreate(page, WebUtility.HtmlDecode(a.Groups["url"].Value), out var uri)) continue;
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) continue;
            if (!IsLocal(uri)) continue;
            if (_ignoredExt.Contains(Path.GetExtension(uri.LocalPath).ToLower())) continue;

            string name = WebUtility.HtmlDecode(TagRx.Replace(a.Groups["text"].Value, ""))
                .Replace("\u00AD", "").Replace("\u200B", "").Trim();
            if (name == "") name = uri.LocalPath;

            children.Add((uri, Regex.Replace(name, @"\s+", " ")));
        }

        return children.DistinctBy(c => c.Item1).ToList();
    }

    private static IEnumerable<string> Find(Regex rx, string text) =>
        rx.Matches(text)
          .Select(x => Regex.Replace(x.Value, @"\s+", " ").Trim())
          .Distinct();

    private bool IsLocal(Uri uri)
    {
        string h = uri.Host.ToLowerInvariant();
        return h == _baseDomain || h == "www." + _baseDomain || h.EndsWith("." + _baseDomain);
    }

    public void Dispose() => _http.Dispose();
}

internal class Program
{
    private static string Csv(string s) => $"\"{(s ?? "").Replace("\"", "\"\"")}\"";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        using var scanner = new WebScanner();
        using var csv = new StreamWriter("result.csv", false, new UTF8Encoding(true));
        csv.WriteLine("Ссылка;URI;Уровень;Адрес;Телефон");

        scanner.TargetFound += (page, title, level, c) =>
        {
            string kind = c.Phone != null ? $"Телефон: {c.Phone}" : $"Адрес: {c.Address}";
            Console.WriteLine($"[{level}] {title} -> {page}\n\t{kind}");
            csv.WriteLine($"{Csv(title)};{Csv(page.ToString())};{level};{Csv(c.Address)};{Csv(c.Phone)}");
        };

        scanner.ScanAsync(new Uri("https://www.susu.ru/ru/structure"), maxPages: 90, maxLevel: 2)
               .GetAwaiter().GetResult();

        Console.WriteLine("Done.");
    }
}