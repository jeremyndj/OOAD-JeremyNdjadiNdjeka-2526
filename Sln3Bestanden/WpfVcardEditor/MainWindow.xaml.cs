using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfVcardEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnSave.IsEnabled = false;
        }

        private string huidigBestandPad = null;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();//deze lijn is met AI gedaan om te weten hoe ik de window moest tonen zonder het als popup te hebben
            about.Show();

        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ben je zeker dat je de applicatie wilt afsluiten?", "Bevestigen", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Filter = "Tekstbestanden|*.vcf;*.vcf";
            bool? dialogResult = dialog.ShowDialog();
            //Dit gedeelde hieronder is met AI gedaan
            if (dialogResult != true)
            {
                return;
            }

            string bestandspad = dialog.FileName;

            string[] regels;
            try
            {
                regels = File.ReadAllLines(bestandspad);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Kan het bestand niet lezen:\n{ex.Message}",
                                "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string voornaam = "";
            string achternaam = "";
            string bday = "";
            string gender = "";
            string emailPrive = "";
            string telPrive = "";

            foreach (string regel in regels)
            {
                string lijn = regel.Trim();
                if (string.IsNullOrWhiteSpace(lijn)) continue;

                int dubbelePuntPos = lijn.IndexOf(':');
                if (dubbelePuntPos < 0) continue;

                string sleutel = lijn.Substring(0, dubbelePuntPos).ToUpper().Trim();
                string waarde = lijn.Substring(dubbelePuntPos + 1).Trim();

                // Parameters verwijderen (CHARSET, TYPE, etc.)
                int puntKommaPos = sleutel.IndexOf(';');
                if (puntKommaPos >= 0)
                {
                    sleutel = sleutel.Substring(0, puntKommaPos).Trim();
                }

                switch (sleutel)
                {
                    case "FN":
                        string[] delen = waarde.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (delen.Length >= 2)
                        {
                            voornaam = delen[0];
                            achternaam = string.Join(" ", delen, 1, delen.Length - 1);
                        }
                        break;

                    case "N":
                        string[] nDelen = waarde.Split(';');
                        if (nDelen.Length >= 2)
                        {
                            achternaam = nDelen[0].Trim();
                            voornaam = nDelen[1].Trim();
                        }
                        break;

                    case "BDAY":
                        if (waarde.Length == 8 && waarde.All(char.IsDigit))
                        {
                            bday = $"{waarde.Substring(0, 4)}-{waarde.Substring(4, 2)}-{waarde.Substring(6, 2)}";
                        }
                        else
                        {
                            bday = waarde;
                        }
                        break;

                    case "GENDER":
                        string g = waarde.Trim().ToUpper();
                        if (g == "M") gender = "man";
                        else if (g == "F") gender = "vrouw";
                        else gender = "onbekend";
                        break;

                    case "EMAIL":
                        if (lijn.Contains("HOME", StringComparison.OrdinalIgnoreCase) ||
                            !lijn.Contains("WORK", StringComparison.OrdinalIgnoreCase))
                        {
                            emailPrive = waarde;
                        }
                        break;

                    case "TEL":
                        if (lijn.Contains("HOME", StringComparison.OrdinalIgnoreCase) ||
                            lijn.Contains("VOICE", StringComparison.OrdinalIgnoreCase))
                        {
                            telPrive = waarde;
                        }
                        break;
                }
            }

            // ────────────────────────────────────────────────
            // 6. Velden invullen
            // ────────────────────────────────────────────────
            txtVoornaam.Text = voornaam;
            txtAchternaam.Text = achternaam;

            if (DateTime.TryParse(bday, out DateTime geboorteDatum))
            {
                datGeboorte.SelectedDate = geboorteDatum;
            }

            if (gender == "vrouw") rbnVrouw.IsChecked = true;
            else if (gender == "man") rbnMan.IsChecked = true;
            else rbnOnbekend.IsChecked = true;

            txtPrvEmail.Text = emailPrive;
            txtPrvTelefoon.Text = telPrive;

            btnSave.IsEnabled = true;

            MessageBox.Show("vCard ingelezen (alleen Persoonlijk-tabblad).",
                            "Klaar", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private List<string> GenereerVCardInhoud()
        {
            List<string> regels = new List<string>();

            // Eerst de verplichte beginregels
            regels.Add("BEGIN:VCARD");
            regels.Add("VERSION:3.0");

            // Volledige naam (FN) - alleen als er een naam is ingevuld
            string volledigeNaam = txtVoornaam.Text.Trim() + " " + txtAchternaam.Text.Trim();
            volledigeNaam = volledigeNaam.Trim();
            if (volledigeNaam != "")
            {
                regels.Add("FN:" + volledigeNaam);
            }

            // Naam componenten (N) - altijd aanwezig, anders is de vCard ongeldig
            string achternaam = txtAchternaam.Text.Trim();
            string voornaam = txtVoornaam.Text.Trim();
            regels.Add("N:" + achternaam + ";" + voornaam + ";;;");

            // Geboortedatum (BDAY) - alleen als er een datum is gekozen
            if (datGeboorte.SelectedDate.HasValue)
            {
                string geboorteDatum = datGeboorte.SelectedDate.Value.ToString("yyyyMMdd");
                regels.Add("BDAY:" + geboorteDatum);
            }

            // Geslacht (GENDER) - alleen als man of vrouw is aangevinkt
            if (rbnMan.IsChecked == true)
            {
                regels.Add("GENDER:M");
            }
            else if (rbnVrouw.IsChecked == true)
            {
                regels.Add("GENDER:F");
            }
            // Als 'onbekend' is gekozen, voegen we geen GENDER regel toe

            // E-mail (EMAIL) - alleen als er een e-mailadres is ingevuld
            string email = txtPrvEmail.Text.Trim();
            if (email != "")
            {
                regels.Add("EMAIL;HOME:" + email);
            }

            // Telefoon (TEL) - alleen als er een telefoonnummer is ingevuld
            string telefoon = txtPrvTelefoon.Text.Trim();
            if (telefoon != "")
            {
                regels.Add("TEL;HOME;VOICE:" + telefoon);
            }

            // Afsluiten
            regels.Add("END:VCARD");

            return regels;
        }

        private void SaveVCardToFile(string filePath) //AI HULPMETHODE
        {
            List<string> content = GenereerVCardInhoud();
            File.WriteAllLines(filePath, content, Encoding.UTF8);
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialoog = new SaveFileDialog();
            dialoog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialoog.Filter = "vCard-bestanden|*.vcf|Alle bestanden|*.*";
            dialoog.FileName = "contact.vcf";

            if (dialoog.ShowDialog() == true)
            {
                string bestandPad = dialoog.FileName;
                try
                {
                    SaveVCardToFile(bestandPad);
                    // Onthouden dat we dit bestand nu gebruiken voor Save
                    huidigBestandPad = bestandPad;
                    btnSave.IsEnabled = true; // Save knop activeren
                    txtHuidigeKaart.Text = $"huidige kaart: {System.IO.Path.GetFileName(bestandPad)}";
                    // Geen melding (zoals gevraagd)
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij opslaan:\n" + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e) //AI-methode
        {
            if (huidigBestandPad != null)
            {
                try
                {
                    SaveVCardToFile(huidigBestandPad);
                    MessageBox.Show("Bestand opgeslagen.", "Bevestiging", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij opslaan:\n" + ex.Message, "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                SaveAsMenuItem_Click(sender, e);
            }
        }

        // 6. Vervang de bestaande btnSave_Click door deze:
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveMenuItem_Click(sender, e);
        }
    }
}