using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// Overzicht: contactkaarten met zoekfilter (gegevens uit de database).
public partial class PatientenPage : Page
{
    private readonly PatientService _svcPatient = new PatientService();
    private readonly FontFamily _fontIconen = new FontFamily("Segoe MDL2 Assets");

    public PatientenPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        VernieuwKaarten();
    }

    private void TxtZoek_TextChanged(object sender, TextChangedEventArgs e)
    {
        VernieuwKaarten();
    }

    // Bouwt de kaarten opnieuw op basis van de zoektekst.
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
        btnDetail.Click += (_, _) => NavigationService?.Navigate(new PatientDetailPage(iId));

        Button btnWijzig = MaakIcoonKnop("\uE70F", "Aanpassen");
        btnWijzig.Click += (_, _) => NavigationService?.Navigate(new PatientBewerkPage(iId));

        Button btnVerwijder = MaakIcoonKnop("\uE74D", "Verwijderen");
        btnVerwijder.Click += (_, _) => NavigationService?.Navigate(new PatientVerwijderPage(iId));

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

    private void BtnTerugNaarStart_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new StartPage());
    }
}
