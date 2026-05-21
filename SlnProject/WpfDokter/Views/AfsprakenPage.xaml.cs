using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfDokter.Views;

// =============================================================================
// AfsprakenPage — dagoverzicht afspraken van de ingelogde dokter
// =============================================================================
// Filter: Session.GebruikerId (dokter_id) + gekozen datum in calDatum.
// Lijst: handmatig ListBoxItem per afspraak; volledig AfspraakWeergave-object in Tag.
// Detail: txtDetail + annuleren-knop; annuleren = DELETE in DB na MessageBox-bevestiging.
// Geen data binding op ItemsSource (projectafspraak).
// =============================================================================
public partial class AfsprakenPage : Page
{
    private readonly AfspraakService _svcAfspraak = new AfspraakService();
    // Onthoudt de laatst geselecteerde afspraak voor BtnAnnuleren_Click (id + gegevens).
    private AfspraakWeergave? _afspraakGeselecteerd;
    // Voorkomt dubbele LaadAfspraken tijdens Page_Loaded wanneer SelectedDate programmatisch wordt gezet.
    private bool _bInitialiseert;

    public AfsprakenPage()
    {
        InitializeComponent();
    }

    // -------------------------------------------------------------------------
    // Page_Loaded — start op vandaag
    // -------------------------------------------------------------------------
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        _bInitialiseert = true;
        calDatum.SelectedDate = DateTime.Today;
        _bInitialiseert = false;
        LaadAfsprakenVoorGeselecteerdeDatum();
        WisDetail();
    }

    // -------------------------------------------------------------------------
    // CalDatum_SelectedDatesChanged — andere dag gekozen in kalender
    // -------------------------------------------------------------------------
    private void CalDatum_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_bInitialiseert || calDatum.SelectedDate == null)
        {
            return;
        }

        LaadAfsprakenVoorGeselecteerdeDatum();
        WisDetail();
    }

    // -------------------------------------------------------------------------
    // LaadAfsprakenVoorGeselecteerdeDatum — SQL via AfspraakService.HaalAfsprakenOpDag
    // -------------------------------------------------------------------------
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

    // Vult txtAfsprakenDatum met een volledige datum in het Nederlands (nl-BE).
    private void WerkDatumTitelBij(DateTime datum)
    {
        string strDatum = datum.ToString("dddd d MMMM yyyy", new CultureInfo("nl-BE"));
        txtAfsprakenDatum.Text = "Afspraken voor " + strDatum;
    }

    // -------------------------------------------------------------------------
    // LstAfspraken_SelectionChanged — selectie koppelen aan detailpaneel
    // -------------------------------------------------------------------------
    private void LstAfspraken_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListBoxItem? item = lstAfspraken.SelectedItem as ListBoxItem;
        AfspraakWeergave? afspraak = null;
        if (item != null && item.Tag != null)
        {
            afspraak = item.Tag as AfspraakWeergave;
        }

        if (afspraak != null)
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

    // Reset selectie en detailtekst; annuleren-knop uit tot er opnieuw een rij wordt gekozen.
    private void WisDetail()
    {
        _afspraakGeselecteerd = null;
        lstAfspraken.SelectedItem = null;
        txtDetail.Text = "Selecteer een afspraak in de lijst.";
        btnAnnuleren.IsEnabled = false;
    }

    // -------------------------------------------------------------------------
    // BtnAnnuleren_Click — afspraak verwijderen na bevestiging (MessageBox)
    // -------------------------------------------------------------------------
    // DELETE alleen als dokter_id overeenkomt met Session.GebruikerId (in repository).
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

}
