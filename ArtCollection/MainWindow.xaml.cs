using ArtCollection.Models;
using ArtCollection.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArtCollection;

public partial class MainWindow : Window
{
    private readonly List<Artwork> _artworks = new();
    private readonly CultureInfo _ua = CultureInfo.GetCultureInfo("uk-UA");

    public MainWindow()
    {
        InitializeComponent();
        UpdateList();
    }

    private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PaintingFields is null || SculptureFields is null)
            return;

        bool painting = TypeComboBox.SelectedIndex == 0;

        PaintingFields.Visibility = painting
            ? Visibility.Visible : Visibility.Collapsed;

        SculptureFields.Visibility = painting
            ? Visibility.Collapsed : Visibility.Visible;

        if (painting)
        {
            TitleTextBox.Text = "Зоряна ніч";
            YearTextBox.Text = "1889";
            AuthorTextBox.Text = "Вінсент ван Гог";
            BaseValueTextBox.Text = "10000";

            TechniqueTextBox.Text = "Олія";
            WidthTextBox.Text = "73.7";
            HeightTextBox.Text = "92.1";
        }
        else
        {
            TitleTextBox.Text = "Мислитель";
            YearTextBox.Text = "1904";
            AuthorTextBox.Text = "Огюст Роден";
            BaseValueTextBox.Text = "15000";

            MaterialTextBox.Text = "Бронза";
        }
    }

    private static decimal ParseMoney(string input)
    {
        string normalized = input.Trim().Replace(',', '.');
        if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            throw new FormatException("Введіть коректну базову вартість.");
        return result;
    }

    private static double ParseSize(string input)
    {
        string normalized = input.Trim().Replace(',', '.');
        if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            throw new FormatException("Введіть коректний розмір полотна.");
        return result;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(YearTextBox.Text, out int year))
                throw new FormatException("Рік створення повинен бути цілим числом.");

            decimal value = ParseMoney(BaseValueTextBox.Text);
            Artwork artwork = TypeComboBox.SelectedIndex == 0
                ? new Painting(TitleTextBox.Text, year, AuthorTextBox.Text, value,
                    TechniqueTextBox.Text, ParseSize(WidthTextBox.Text), ParseSize(HeightTextBox.Text))
                : new Sculpture(TitleTextBox.Text, year, AuthorTextBox.Text, value, MaterialTextBox.Text);

            _artworks.Add(artwork);
            UpdateList();
            ArtworkListBox.SelectedIndex = _artworks.Count - 1;
            StatusTextBlock.Text = $"Додано: «{artwork.Title}».";
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
        {
            MessageBox.Show(ex.Message, "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private Artwork? SelectedArtwork => ArtworkListBox.SelectedIndex is int i && i >= 0 && i < _artworks.Count
        ? _artworks[i] : null;

    private void ArtworkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Artwork? artwork = SelectedArtwork;
        SelectedInfoTextBlock.Text = artwork is null
            ? "Оберіть твір, щоб переглянути його характеристики."
            : $"{artwork.Title} ({artwork.CreationYear}), {artwork.Author}\n{artwork.Details()}\nРеставрація: {(artwork.IsRestored ? "так" : "ні")}";
    }

    private void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        StatusTextBlock.Text = $"Оцінка «{artwork.Title}»: {artwork.Evaluate().ToString("N2", _ua)} грн.";
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        string message = artwork.Restore();
        UpdateList();
        ArtworkListBox.SelectedIndex = _artworks.IndexOf(artwork);
        StatusTextBlock.Text = message;
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        _artworks.Remove(artwork);
        UpdateList();
        StatusTextBlock.Text = $"Видалено: «{artwork.Title}».";
    }

    private void ShowSelectionMessage() => MessageBox.Show("Спочатку оберіть твір у списку.",
        "Не вибрано твір", MessageBoxButton.OK, MessageBoxImage.Information);

    private void UpdateList()
    {
        ArtworkListBox.ItemsSource = null;
        ArtworkListBox.ItemsSource = _artworks.Select(a => new ArtworkItem(a)).ToList();
        TotalValueTextBlock.Text = CollectionAnalysis.TotalValueBefore1900(_artworks).ToString("N2", _ua) + " грн";
        SelectedInfoTextBlock.Text = "Оберіть твір, щоб переглянути його характеристики.";
    }

    private sealed class ArtworkItem
    {
        public Artwork Artwork { get; }
        public ArtworkItem(Artwork artwork) => Artwork = artwork;
        public override string ToString() => $"{(Artwork is Painting ? "Картина" : "Скульптура")}: {Artwork.Title} ({Artwork.CreationYear}) — {Artwork.Evaluate():N2} грн";
    }
}