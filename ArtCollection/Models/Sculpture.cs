using System;

namespace ArtCollection.Models;

public class Sculpture : Artwork
{
    public string Material { get; }

    public Sculpture(string title, int year, string author, decimal baseValue, string material)
        : base(title, year, author, baseValue)
    {
        if (string.IsNullOrWhiteSpace(material))
            throw new ArgumentException("Вкажіть матеріал скульптури.");
        Material = material.Trim();
    }

    public int EstimateAge(int currentYear) => Math.Max(0, currentYear - CreationYear);

    public override decimal Evaluate()
    {
        decimal factor = Material.ToLowerInvariant() switch
        {
            "бронза" => 1.30m,
            "мармур" => 1.20m,
            _ => 1m
        };
        return decimal.Round(BaseValue * factor * RestorationMultiplier, 2);
    }

    public override string Restore()
    {
        if (IsRestored) return "Скульптуру вже реставровано.";
        IsRestored = true;
        return $"Скульптуру «{Title}» реставровано.";
    }

    public override string Details() => $"Скульптура | {Material} | Вік: {EstimateAge(DateTime.Now.Year)} р.";
}