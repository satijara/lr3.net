using System;

namespace ArtCollection.Models;

public class Painting : Artwork
{
    public string Technique { get; }
    public double WidthCm { get; }
    public double HeightCm { get; }

    public Painting(string title, int year, string author, decimal baseValue,
                    string technique, double widthCm, double heightCm)
        : base(title, year, author, baseValue)
    {
        if (string.IsNullOrWhiteSpace(technique))
            throw new ArgumentException("Вкажіть техніку картини.");
        if (widthCm <= 0 || heightCm <= 0 || double.IsNaN(widthCm) || double.IsNaN(heightCm)
            || double.IsInfinity(widthCm) || double.IsInfinity(heightCm))
            throw new ArgumentOutOfRangeException(nameof(widthCm), "Розміри полотна мають бути додатними.");
        Technique = technique.Trim();
        WidthCm = widthCm;
        HeightCm = heightCm;
    }

    public string GetCanvasSize() => $"{WidthCm:0.##} × {HeightCm:0.##} см";

    public override decimal Evaluate()
    {
        decimal factor = Technique.ToLowerInvariant() switch
        {
            "олія" or "олійний живопис" => 1.25m,
            "акварель" => 1.10m,
            _ => 1m
        };
        return decimal.Round(BaseValue * factor * RestorationMultiplier, 2);
    }

    public override string Restore()
    {
        if (IsRestored) return "Картину вже реставровано.";
        IsRestored = true;
        return $"Картину «{Title}» реставровано.";
    }

    public override string Details() => $"Картина | {Technique} | Полотно: {GetCanvasSize()}";
}