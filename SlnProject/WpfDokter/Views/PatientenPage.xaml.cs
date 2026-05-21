using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// =============================================================================
// PatientenPage — overzicht van alle patiënten als contactkaarten
// =============================================================================
// Data-flow: txtZoek → PatientService.HaalVoorOverzicht → PatientRepository (SQL LIKE op naam).
// UI wordt volledig in code opgebouwd (geen ItemsSource/data binding per projectafspraak).
// Elke kaart heeft drie icoonknoppen; het patiënt-id zit in Button.Tag voor navigatie.
// =============================================================================
public partial class PatientenPage : Page
{
    private readonly PatientService _svcPatient = new PatientService();
    // Lettertype voor Segoe MDL2-glyphen op detail/wijzig/verwijder-knoppen.
    private readonly FontFamily _fontIconen = new FontFamily("Segoe MDL2 Assets");

    public PatientenPage()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — eerste vulling van pnlKaarten (WrapPanel in XAML)
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        VernieuwKaarten();
    }

    // -------------------------------------------------------------------------
    // TxtZoek_TextChanged — live filter bij elke toetsaanslag
    // -------------------------------------------------------------------------
    // Roept VernieuwKaarten opnieuw aan; lege zoektekst = alle patiënten uit de database.
    private void TxtZoek_TextChanged(object sender, TextChangedEventArgs e)
    {
        VernieuwKaarten();
    }

    // -------------------------------------------------------------------------
    // VernieuwKaarten — kern: lijst ophalen en kaarten opnieuw tekenen
    // -------------------------------------------------------------------------
    // Stappen: Children.Clear → service aanroepen → per Patient MaakContactKaart → toevoegen.
    // Bij SQL-fout: MessageBox (technische fout laden, geen formulier-validatie).
    private void VernieuwKaarten()
    {
        pnlKaarten.Children.Clear();
        string strFilter = txtZoek.Text;

        try
        {
            List<Patient> lijstPatienten = _svcPatient.HaalVoorOverzicht(strFilter);
            foreach (Patient patient in lijstPatienten)
            {
                Border brdKaart = MaakContactKaart(patient);
                pnlKaarten.Children.Add(brdKaart);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Patiënten laden is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // -------------------------------------------------------------------------
    // MaakContactKaart — bouwt één visuele kaart (Border + Grid + knoppen)
    // -------------------------------------------------------------------------
    // Layout: links profielfoto (2 rijen hoog), rechtsboven naam/e-mail/gsm, rechtsonder actieknoppen.
    // patient.Id wordt op elke actieknop in Tag gezet (geen closure met discard-parameters).
    private Border MaakContactKaart(Patient patient)
    {
        Border brd = new Border
        {
            Width = 260,
            Margin = new Thickness(0, 0, 12, 12),
            Padding = new Thickness(12),
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Tag = patient
        };

        Grid grd = new Grid();
        grd.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grd.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grd.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grd.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        Image imgProfiel = new Image
        {
            Width = 56,
            Height = 56,
            Stretch = Stretch.UniformToFill,
            Margin = new Thickness(0, 0, 10, 0),
            VerticalAlignment = VerticalAlignment.Top
        };
        ProfielAfbeeldingHelper.LaadProfielAfbeelding(imgProfiel, patient.ProfielData);
        Grid.SetRow(imgProfiel, 0);
        Grid.SetColumn(imgProfiel, 0);
        Grid.SetRowSpan(imgProfiel, 2);

        StackPanel pnlTekst = new StackPanel();
        TextBlock txtNaam = new TextBlock
        {
            Text = patient.Voornaam + " " + patient.Achternaam,
            FontWeight = FontWeights.SemiBold,
            FontSize = 15,
            TextWrapping = TextWrapping.Wrap,
            TextTrimming = TextTrimming.CharacterEllipsis
        };
        TextBlock txtEmail = new TextBlock
        {
            Text = patient.Email,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            FontSize = 13,
            Margin = new Thickness(0, 4, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };
        TextBlock txtGsm = new TextBlock
        {
            Text = patient.Gsm,
            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
            FontSize = 13,
            Margin = new Thickness(0, 2, 0, 0),
            TextWrapping = TextWrapping.Wrap
        };
        pnlTekst.Children.Add(txtNaam);
        pnlTekst.Children.Add(txtEmail);
        pnlTekst.Children.Add(txtGsm);
        Grid.SetRow(pnlTekst, 0);
        Grid.SetColumn(pnlTekst, 1);

        StackPanel pnlKnoppen = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 12, 0, 0)
        };
        int iId = patient.Id;

        Button btnDetail = MaakIcoonKnop("\uE946", "Details");
        btnDetail.Tag = iId;
        btnDetail.Click += BtnKaartDetail_Click;

        Button btnWijzig = MaakIcoonKnop("\uE70F", "Aanpassen");
        btnWijzig.Tag = iId;
        btnWijzig.Click += BtnKaartWijzig_Click;

        Button btnVerwijder = MaakIcoonKnop("\uE74D", "Verwijderen");
        btnVerwijder.Tag = iId;
        btnVerwijder.Click += BtnKaartVerwijder_Click;

        pnlKnoppen.Children.Add(btnDetail);
        pnlKnoppen.Children.Add(btnWijzig);
        pnlKnoppen.Children.Add(btnVerwijder);
        Grid.SetRow(pnlKnoppen, 1);
        Grid.SetColumn(pnlKnoppen, 1);

        grd.Children.Add(imgProfiel);
        grd.Children.Add(pnlTekst);
        grd.Children.Add(pnlKnoppen);
        brd.Child = grd;
        return brd;
    }

    // Maakt een kleine transparante knop met één MDL2-glyph en tooltip.
    private Button MaakIcoonKnop(string strGlyph, string strTooltip)
    {
        Button btn = new Button
        {
            FontFamily = _fontIconen,
            Content = strGlyph,
            FontSize = 16,
            Width = 36,
            Height = 36,
            Margin = new Thickness(0, 0, 8, 0),
            Padding = new Thickness(0),
            ToolTip = strTooltip,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0)
        };
        return btn;
    }

    // Leest het integer-id uit de Tag van de aangeklikte knop; 0 = ongeldig (geen navigatie).
    private int HaalPatientIdUitKnop(object sender)
    {
        Button? btn = sender as Button;
        if (btn == null || btn.Tag == null)
        {
            return 0;
        }

        return (int)btn.Tag;
    }

    private void BtnKaartDetail_Click(object sender, RoutedEventArgs e)
    {
        int iId = HaalPatientIdUitKnop(sender);
        if (iId > 0 && NavigationService != null)
        {
            NavigationService.Navigate(new PatientDetailPage(iId));
        }
    }

    // Opent PatientBewerkPage met id 0 → INSERT in database bij Opslaan.
    private void BtnNieuwPatient_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new PatientBewerkPage(0));
        }
    }

    private void BtnKaartWijzig_Click(object sender, RoutedEventArgs e)
    {
        int iId = HaalPatientIdUitKnop(sender);
        if (iId > 0 && NavigationService != null)
        {
            NavigationService.Navigate(new PatientBewerkPage(iId));
        }
    }

    // Opent PatientVerwijderPage met bevestiging en DELETE (inclusief gekoppelde afspraken).
    private void BtnKaartVerwijder_Click(object sender, RoutedEventArgs e)
    {
        int iId = HaalPatientIdUitKnop(sender);
        if (iId > 0 && NavigationService != null)
        {
            NavigationService.Navigate(new PatientVerwijderPage(iId));
        }
    }

}
