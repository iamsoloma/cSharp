using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace example;

public class WebScanner : IDisposable
{
    private readonly HashSet<Uri> _procLinks = new HashSet<Uri>();
    private readonly HttpClient _httpClient = new HttpClient();

    public WebScanner()
    {
        // Без User-Agent сайт www.susu.ru отвечает 403 Forbidden (после редиректа на /en)
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (X11; Linux x86_64; rv:120.0) Gecko/20100101 Firefox/120.0");
    }

    private readonly HashSet<string> _ignoredFiles = new HashSet<string> { ".ico", ".xml" };
    private void OnTargetFound(Uri page, Uri[] links)
    {
        TargetFound?.Invoke(page, links);
    }

    private async Task ProcessAsync(string domain, Uri page, int count)
    {
        if (count <= 0) return;

        if (_procLinks.Contains(page)) return;
        _procLinks.Add(page);


        string html;
        try
        {
            html = await _httpClient.GetStringAsync(page);
        }
        catch (HttpRequestException)
        {
            return;
        }

        //Ссылки @"href=""[\/\w\-\.:]+"""
        //Почты ([-\w]+\[dot\])*[-\w]+\[at\][-\w]+(\[dot\][-\w]+)+
        var hrefs = (from href in Regex.Matches(html, @"href=""[\/\w\-\.:]+""").Cast<Match>()
                     let url = href.Value.Replace("href=", "").Trim('"')
                     let local = url.StartsWith("/")
                     let uriString = local ? $"{domain}{url}" : url
                     let isValid = Uri.TryCreate(uriString, UriKind.Absolute, out _)
                     where isValid
                     let validUri = new Uri(uriString)
                     select new
                     {
                         Ref = validUri,
                         IsLocal = local || url.StartsWith(domain + "/") || url == domain
                     }
        ).ToList();

        var externals = (
            from href in hrefs
            where !href.IsLocal
            select href.Ref
        ).Distinct().ToArray();

        if (externals.Length > 0) OnTargetFound(page, externals);

        var locals = (
            from href in hrefs
            where href.IsLocal
            select href.Ref
        ).Distinct().ToList();

        foreach (var href in locals)
        {
            string fileEx = Path.GetExtension(href.LocalPath).ToLower();
            if (_ignoredFiles.Contains(fileEx)) continue;

            await ProcessAsync(domain, href, count - 1);
        }
    }

    public event Action<Uri, Uri[]> TargetFound;

    public async Task ScanAsync(Uri startPage, int pageCount)
    {
        _procLinks.Clear();

        string domain = $"{startPage.Scheme}://{startPage.Host}";
        await ProcessAsync(domain, startPage, pageCount);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        using (WebScanner scanner = new WebScanner())
        {
            scanner.TargetFound += (page, links) =>
            {
                Console.WriteLine($"\nPage:\n\t{page}\nLinks:");

                foreach (var link in links)
                {
                    Console.WriteLine($"\t{link}");
                }
            };


            scanner.ScanAsync(new Uri("https://www.susu.ru"), 2).GetAwaiter().GetResult();
            Console.WriteLine("Done.");
        }
    }
}
