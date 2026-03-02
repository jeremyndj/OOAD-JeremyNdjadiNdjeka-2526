using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Order Builder", Description = "CheckBox + RadioButton met samenvatting en reset.", Order = 5, IsVisible = true)]
    public partial class OrderBuilder : Page
    {
        public OrderBuilder()
        {
            InitializeComponent();
        }

        private void BtnBevestig_Click(object sender, RoutedEventArgs e)
        {
            string levering = "";
            if (rbnAfhalen.IsChecked == true)
            {
                levering = "Afhalen";
            }
            else if (rbnLevering.IsChecked == true)
            {
                levering = "Levering";
            }
            else if (rbnPlaatse.IsChecked == true)
            {
                levering = "Ter Plaatse";
            }

            if (levering == "")
            {
                txtSamenvatting.Text = "Kies eerst een leveringsmethode";
                return;
            }

            List<string> extras = new List<string>();
            if (chkKaas.IsChecked == true)
            {
                extras.Add("Kaas");
            }
            if (chkSpek.IsChecked == true)
            {
                extras.Add("Spek");
            }
            if (chkSaus.IsChecked == true)
            {
                extras.Add("Extra saus");
            }
            if (chkUi.IsChecked == true)
            {
                extras.Add("Ui");
            }

            string extrasTekst = extras.Count > 0 ? string.Join(", ", extras) : "geen";

            txtSamenvatting.Text = $"Levering: {levering}{Environment.NewLine}Extra's: {extrasTekst}";
        }

        private void btnReset_Click_1(object sender, RoutedEventArgs e)
        {
            rbnAfhalen.IsChecked = false;
            rbnLevering.IsChecked = false;
            rbnPlaatse.IsChecked = false;

            chkKaas.IsChecked = true;
            chkSpek.IsChecked = false;
            chkSaus.IsChecked = true;
            chkUi.IsChecked = false;

            txtSamenvatting.Text = "...";
        }
    }
}