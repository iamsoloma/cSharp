
using System;

namespace cSharp;

public class BankAccout
{
    public string Owner { get; }
    public int Balance { get; private set; }

    public BankAccout(string owner, int balance)
    {
        Owner = owner;
        Balance = balance;
    }

    public void Deposit(int amount)
    {
        if (amount < 0)
        {
            return;
        }
        Balance += amount;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        BankAccout original = new BankAccout("maria", 700);
        BankAccout alias = original;

        alias.Deposit(100);
        original.Deposit(-50);

        Console.WriteLine($"{original.Balance}:{alias.Balance}");
        Console.WriteLine(object.ReferenceEquals(original, alias));
    }
}