using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;
using System.Linq;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Live Form Validation", Description = "TextChanged gebruiken voor live validatie en IsEnabled.", Order = 4, IsVisible = true)]
    public partial class LiveFormValidation : Page
    {
        public LiveFormValidation()
        {
            InitializeComponent();
        }

        private void txtPaswoord_TextChanged(object sender, TextChangedEventArgs e)
        {
            string passwoord = txtPaswoord.Text;
            if (string.IsNullOrWhiteSpace(passwoord))
            {
                txtStatus.Text = string.Empty;
                btnSave.IsEnabled = false;
                return;
            }

            if (passwoord.Length < 8)
            {
                txtStatus.Text = "ongledig passwoord";
                txtStatus.Foreground = Brushes.Red;
            }
        }

      /*  static private bool IsPasswoordGeldig(string passwoord) 
        {
            if (passwoord.Length > 8) 
            {
                return false;
            }

            bool passwoord = passwoord.Any(char.IsDigit);

        }*/
    }
}
