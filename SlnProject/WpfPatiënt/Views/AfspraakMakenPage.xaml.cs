using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfPatiënt.Views;

// =============================================================================
// AfspraakMakenPage — nieuwe afspraak aanvragen bij gekozen dokter
// =============================================================================
// Dokter: ComboBox via DokterService.HaalVoorKeuzelijst; kaart via HaalOpId na selectie.
// Opslaan: AfspraakFormulierValidatieHelper + AfspraakService.MaakAfspraak; fout in txtFout.
// Geen SQL in WPF; tijdsloten 15 minuten handmatig in cmbTijd (08:00–17:45).
// =============================================================================
public partial class AfspraakMakenPage : Page
{
    private readonly DokterService _svcDokter = new DokterService();
    private readonly AfspraakService _svcAfspraak = new AfspraakService();
    private bool _bInitialiseert;

    public AfspraakMakenPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        _bInitialiseert = true;
        dpDatum.DisplayDateStart = DateTime.Today;
        dpDatum.SelectedDate = DateTime.Today;
        VulTijdsloten();
        LaadDoktersInCombo();
        VerbergDokterKaart();
        _bInitialiseert = false;
    }

    private void VulTijdsloten()
    {
        cmbTijd.Items.Clear();
        for (int iUur = 8; iUur <= 17; iUur++)
        {
            for (int iMinuut = 0; iMinuut < 60; iMinuut = iMinuut + 15)
            {
                if (iUur == 17 && iMinuut > 45)
                {
                    break;
                }

                string strTijd = iUur.ToString("00") + ":" + iMinuut.ToString("00");
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = strTijd
                };
                cmbTijd.Items.Add(item);
            }
        }
    }

    private void LaadDoktersInCombo()
    {
        cmbDokter.Items.Clear();
        ComboBoxItem itemLeeg = new ComboBoxItem
        {
            Content = "— Selecteer een dokter —",
            Tag = 0,
            IsEnabled = false
        };
        cmbDokter.Items.Add(itemLeeg);

        try
        {
            List<Dokter> lijstDokters = _svcDokter.HaalVoorKeuzelijst();
            foreach (Dokter dokter in lijstDokters)
            {
                string strNaam = dokter.Voornaam + " " + dokter.Achternaam;
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = strNaam,
                    Tag = dokter.Id
                };
                cmbDokter.Items.Add(item);
            }

            cmbDokter.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ToonFout("Dokters laden is mislukt: " + ex.Message);
        }
    }

    private void CmbDokter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_bInitialiseert)
        {
            return;
        }

        int iDokterId = HaalGeselecteerdeDokterId();
        if (iDokterId <= 0)
        {
            VerbergDokterKaart();
            return;
        }

        try
        {
            Dokter? dokter = _svcDokter.HaalOpId(iDokterId);
            if (dokter == null)
            {
                VerbergDokterKaart();
                ToonFout("De gekozen dokter kon niet worden geladen.");
                return;
            }

            VerbergFout();
            ToonDokterKaart(dokter);
        }
        catch (Exception ex)
        {
            VerbergDokterKaart();
            ToonFout("Doktergegevens laden is mislukt: " + ex.Message);
        }
    }

    private int HaalGeselecteerdeDokterId()
    {
        ComboBoxItem? item = cmbDokter.SelectedItem as ComboBoxItem;
        if (item == null || item.Tag == null)
        {
            return 0;
        }

        return (int)item.Tag;
    }

    private string? HaalGeselecteerdeTijdTekst()
    {
        ComboBoxItem? item = cmbTijd.SelectedItem as ComboBoxItem;
        if (item == null || item.Content == null)
        {
            return null;
        }

        return item.Content.ToString();
    }

    private void ToonDokterKaart(Dokter dokter)
    {
        pnlDokterKaart.Children.Clear();

        Border brd = new Border
        {
            Padding = new Thickness(12),
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8)
        };

        Grid grd = new Grid();
        grd.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grd.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Image imgProfiel = new Image
        {
            Width = 56,
            Height = 56,
            Stretch = Stretch.UniformToFill,
            Margin = new Thickness(0, 0, 12, 0),
            VerticalAlignment = VerticalAlignment.Top
        };
        try
        {
            ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, dokter.ProfielData);
        }
        catch (Exception)
        {
            // Eén ongeldige doktersfoto mag de dokterslijst niet blokkeren.
            imgProfiel.Source = null;
        }

        Grid.SetColumn(imgProfiel, 0);

        StackPanel pnlTekst = new StackPanel();
        TextBlock txtNaam = new TextBlock
        {
            Text = dokter.Voornaam + " " + dokter.Achternaam,
            FontWeight = FontWeights.SemiBold,
            FontSize = 15
        };
        TextBlock txtEmail = new TextBlock
        {
            Text = dokter.Email,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            FontSize = 13,
            Margin = new Thickness(0, 4, 0, 0)
        };
        TextBlock txtGsm = new TextBlock
        {
            Text = string.IsNullOrEmpty(dokter.Gsm) ? "Geen gsm opgegeven" : dokter.Gsm,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            FontSize = 13,
            Margin = new Thickness(0, 2, 0, 0)
        };
        string strConventioneerd = dokter.IsGeconventioneerd ? "Geconventioneerd" : "Niet geconventioneerd";
        TextBlock txtRiziv = new TextBlock
        {
            Text = "RIZIV: " + dokter.RizivNummer + " — " + strConventioneerd,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            FontSize = 13,
            Margin = new Thickness(0, 2, 0, 0)
        };
        pnlTekst.Children.Add(txtNaam);
        pnlTekst.Children.Add(txtEmail);
        pnlTekst.Children.Add(txtGsm);
        pnlTekst.Children.Add(txtRiziv);
        Grid.SetColumn(pnlTekst, 1);

        grd.Children.Add(imgProfiel);
        grd.Children.Add(pnlTekst);
        brd.Child = grd;
        pnlDokterKaart.Children.Add(brd);
        pnlDokterKaart.Visibility = Visibility.Visible;
    }

    private void VerbergDokterKaart()
    {
        pnlDokterKaart.Children.Clear();
        pnlDokterKaart.Visibility = Visibility.Collapsed;
    }

    private void BtnBevestigen_Click(object sender, RoutedEventArgs e)
    {
        VerbergFout();

        int iDokterId = HaalGeselecteerdeDokterId();
        DateTime? datum = dpDatum.SelectedDate;
        string? strTijd = HaalGeselecteerdeTijdTekst();
        string strKlacht = txtReden.Text;

        string? strValidatieFout = AfspraakFormulierValidatieHelper.ValideerFormulier(
            iDokterId,
            datum,
            strTijd ?? string.Empty,
            strKlacht);

        if (strValidatieFout != null)
        {
            ToonFout(strValidatieFout);
            return;
        }

        DateTime? moment = AfspraakFormulierValidatieHelper.BerekenMoment(datum!.Value, strTijd!);
        if (moment == null)
        {
            ToonFout("Het gekozen tijdstip is ongeldig.");
            return;
        }

        try
        {
            int iNieuwId = _svcAfspraak.MaakAfspraak(Session.GebruikerId, iDokterId, moment.Value, strKlacht);
            if (iNieuwId <= 0)
            {
                ToonFout("De afspraak kon niet worden opgeslagen. Controleer uw gegevens.");
                return;
            }

            if (NavigationService != null)
            {
                NavigationService.Navigate(new AfsprakenPage());
            }
        }
        catch (Exception ex)
        {
            ToonFout("Opslaan is mislukt: " + ex.Message);
        }
    }

    private void ToonFout(string strMelding)
    {
        txtFout.Text = strMelding;
        txtFout.Visibility = Visibility.Visible;
    }

    private void VerbergFout()
    {
        txtFout.Visibility = Visibility.Collapsed;
        txtFout.Text = string.Empty;
    }
}
