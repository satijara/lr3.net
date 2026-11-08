using System;

namespace ArtCollection.Models;

public abstract class Artwork
{
    public string Title { get; }
    public int CreationYear { get; }
    public string Author { get; }
    public decimal BaseValue { get; }
    public bool IsRestored { get; protected set; }

    protected Artwork(string title, int creationYear, string author, decimal baseValue)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Введіть назву твору.");
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Введіть автора.");
        if (creationYear < 1 || creationYear > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(creationYear), "Некоректний рік створення.");
        if (baseValue < 0)
            throw new ArgumentOutOfRangeException(nameof(baseValue), "Вартість не може бути від'ємною.");

        Title = title.Trim();
        CreationYear = creationYear;
        Author = author.Trim();
        BaseValue = baseValue;
    }

    public abstract decimal Evaluate();
    public abstract string Restore();

    protected decimal RestorationMultiplier => IsRestored ? 1.05m : 1m;
    public abstract string Details();
}