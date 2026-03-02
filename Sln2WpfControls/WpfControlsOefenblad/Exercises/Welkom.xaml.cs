using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Welkom", Description = "TextBox met eenvoudige validatie.", Order = 2, IsVisible = true)]
    public partial class Welkom : Page
    {
        public Welkom()
        {
            InitializeComponent();
        }

        private void btnHallo_Click(object sender, RoutedEventArgs e)
        {
            string naam1 = txtNaam.Text;
            string naam2 = naam1.Trim();
            txtWelkom.Text = $"Welkom, {naam2}!";

            if (naam1 == "")
            {
                txtFout.Visibility = Visibility.Visible;
                txtWelkom.Visibility = Visibility.Hidden;
            }
        }

    }
}
