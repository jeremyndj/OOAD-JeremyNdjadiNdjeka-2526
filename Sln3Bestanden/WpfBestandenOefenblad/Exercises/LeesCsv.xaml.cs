using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WpfBestandenOefenblad.Helpers;

namespace WpfBestandenOefenblad.Exercises;

[NavPage(Title = "Lees CSV", Description = "CSV inlezen (Product;Quantity;Price), regels tonen en totaal verkoopbedrag", Order = 2, IsVisible = true)]
public partial class LeesCsv : Page
{
    public LeesCsv()
    {
        InitializeComponent();
    }

    private void btnOutput_Click(object sender, RoutedEventArgs e)
    {
        string[] regels = File.ReadAllLines("Exercises/Files/verkoop.csv");
        double totaal = 0;
        for (int i = 0; i < regels.Length; i++) 
        { 
            string regel = regels[i];
            string[] delen = regel.Split(';');

            string product = delen[0];
            int aantal = int.Parse(delen[1]);
            double prijs = double.Parse(delen[2]);

            double bedrag = prijs * aantal;
            totaal += bedrag;

            lstOutput.Items.Add(product + " x" + aantal + " aan €" + prijs.ToString("0,00") + " = €" + bedrag.ToString("0.00"));
        }

        txtOutput.Text = "Totaal verkoopbedrag: €" + totaal.ToString("0.00");
    }
}
