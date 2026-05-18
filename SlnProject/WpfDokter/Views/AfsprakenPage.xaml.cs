using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// Afspraken per dag: kalender, lijst, detail en annuleren.
public partial class AfsprakenPage : Page
{
    private readonly AfspraakService _svcAfspraak = new AfspraakService();
    private AfspraakWeergave? _afspraakGeselecteerd;
    private bool _bInitialiseert;

    public AfsprakenPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        _bInitialiseert = true;
        calDatum.SelectedDate = DateTime.Today;
        _bInitialiseert = false;
        LaadAfsprakenVoorGeselecteerdeDatum();
        WisDetail();
    }

    private void CalDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_bInitialiseert || calDatum.SelectedDate == null)
        {
            return;
        }

        LaadAfsprakenVoorGeselecteerdeDatum();
        WisDetail();
    }

    // Haalt afspraken op voor de ingelogde dokter en de gekozen dag.
    private void LaadAfsprakenVoorGeselecteerdeDatum()
    {
        if (calDatum.SelectedDate == null)
        {
            return;
        }

        DateTime datum = calDatum.SelectedDate.Value;
        WerkDatumTitelBij(datum);

        lstAfspraken.Items.Clear();

        try
        {
            List<AfspraakWeergave> lijstAfspraken =
                _svcAfspraak.HaalAfsprakenOpDag(Session.GebruikerId, datum);

            if (lijstAfspraken.Count == 0)
            {
                ListBoxItem itemLeeg = new ListBoxItem
                {
                    Content = "Geen afspraken op deze dag.",
                    IsEnabled = false
                };
                lstAfspraken.Items.Add(itemLeeg);
                return;
            }

            foreach (AfspraakWeergave afspraak in lijstAfspraken)
            {
                string strTijd = afspraak.Moment.ToString("HH:mm", CultureInfo.CurrentCulture);
                string strRegel = strTijd + " - " + afspraak.PatientNaam;
                ListBoxItem item = new ListBoxItem
                {
                    Content = strRegel,
                    Tag = afspraak
                };
                lstAfspraken.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Afspraken laden is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void WerkDatumTitelBij(DateTime datum)
    {
        string strDatum = datum.ToString("dddd d MMMM yyyy", new CultureInfo("nl-BE"));
        txtAfsprakenDatum.Text = "Afspraken voor " + strDatum;
    }

    private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (lstAfspraken.SelectedItem is ListBoxItem item && item.Tag is AfspraakWeergave afspraak)
        {
            _afspraakGeselecteerd = afspraak;
            ToonDetail(afspraak);
            btnAnnuleren.IsEnabled = true;
        }
        else
        {
            WisDetail();
        }
    }

    private void ToonDetail(AfspraakWeergave afspraak)
    {
        string strTijd = afspraak.Moment.ToString("HH:mm", CultureInfo.CurrentCulture);
        txtDetail.Text =
            "Tijd: " + strTijd + Environment.NewLine +
            "Patiënt: " + afspraak.PatientNaam + Environment.NewLine +
            "Klacht: " + afspraak.Klacht;
    }

    private void WisDetail()
    {
        _afspraakGeselecteerd = null;
        lstAfspraken.SelectedItem = null;
        txtDetail.Text = "Selecteer een afspraak in de lijst.";
        btnAnnuleren.IsEnabled = false;
    }

    private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
    {
        if (_afspraakGeselecteerd == null)
        {
            return;
        }

        MessageBoxResult resultaat = MessageBox.Show(
            "Weet u zeker dat u deze afspraak wilt annuleren?",
            "Afspraak annuleren",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultaat != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            bool bGelukt = _svcAfspraak.Annuleer(_afspraakGeselecteerd.Id, Session.GebruikerId);
            if (!bGelukt)
            {
                MessageBox.Show(
                    "De afspraak kon niet worden geannuleerd.",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            LaadAfsprakenVoorGeselecteerdeDatum();
            WisDetail();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Annuleren is mislukt: " + ex.Message,
                "Fout",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void BtnTerugNaarStart_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new StartPage());
    }
}
