using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CLDokterspraktijk.Models;
using CLDokterspraktijk.Services;

namespace WpfPatiënt.Views;

// =============================================================================
// AfsprakenPage — overzicht eigen afspraken met filter, detail en annuleren
// =============================================================================
// Data: HaalAfsprakenVoorPatiënt(Session.GebruikerId); filter alle/toekomst in UI (foreach).
// Lijst: tijdstip + dokternaam; klacht in txtDetail; Tag = AfspraakWeergave.
// Annuleren: alleen toekomstige afspraak, MessageBox-bevestiging, AnnuleerDoorPatiënt.
// =============================================================================
public partial class AfsprakenPage : Page
{
    private readonly AfspraakService _svcAfspraak = new AfspraakService();
    private List<AfspraakWeergave> _lijstAlleAfspraken = new List<AfspraakWeergave>();
    private AfspraakWeergave? _afspraakGeselecteerd;

    public AfsprakenPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        LaadAfsprakenUitDatabase();
    }

    private void BtnNieuweAfspraak_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService != null)
        {
            NavigationService.Navigate(new AfspraakMakenPage());
        }
    }

    private void RbFilter_Checked(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        VulLijstWeergave();
        WisDetail();
    }

    private void LaadAfsprakenUitDatabase()
    {
        VerbergFout();
        _lijstAlleAfspraken = new List<AfspraakWeergave>();

        if (Session.GebruikerId <= 0)
        {
            ToonFout("U bent niet ingelogd. Log opnieuw in.");
            return;
        }

        try
        {
            _lijstAlleAfspraken = _svcAfspraak.HaalAfsprakenVoorPatiënt(Session.GebruikerId);
            VulLijstWeergave();
            WisDetail();
        }
        catch (Exception ex)
        {
            ToonFout("Afspraken laden is mislukt: " + ex.Message);
        }
    }

    private void VulLijstWeergave()
    {
        lstAfspraken.Items.Clear();
        bool bAlleenToekomst = rbToekomst.IsChecked == true;
        DateTime nu = DateTime.Now;
        int iGetoond = 0;

        foreach (AfspraakWeergave afspraak in _lijstAlleAfspraken)
        {
            if (bAlleenToekomst && afspraak.Moment <= nu)
            {
                continue;
            }

            string strDatumTijd = afspraak.Moment.ToString("d MMMM yyyy HH:mm", new CultureInfo("nl-BE"));
            string strRegel = strDatumTijd + " — " + afspraak.DokterNaam;
            ListBoxItem item = new ListBoxItem
            {
                Content = strRegel,
                Tag = afspraak
            };
            lstAfspraken.Items.Add(item);
            iGetoond = iGetoond + 1;
        }

        if (iGetoond == 0)
        {
            string strLeeg = bAlleenToekomst
                ? "U heeft geen toekomstige afspraken."
                : "U heeft nog geen afspraken. Maak een afspraak via Nieuwe afspraak.";
            ListBoxItem itemLeeg = new ListBoxItem
            {
                Content = strLeeg,
                IsEnabled = false
            };
            lstAfspraken.Items.Add(itemLeeg);
        }
    }

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
            btnAnnuleren.IsEnabled = afspraak.Moment > DateTime.Now;
        }
        else
        {
            WisDetail();
        }
    }

    private void ToonDetail(AfspraakWeergave afspraak)
    {
        string strDatumTijd = afspraak.Moment.ToString(
            "dddd d MMMM yyyy HH:mm",
            new CultureInfo("nl-BE"));
        string strKlacht = afspraak.Klacht;
        if (string.IsNullOrWhiteSpace(strKlacht))
        {
            strKlacht = "(geen reden opgegeven)";
        }

        txtDetail.Text =
            "Dokter: " + afspraak.DokterNaam + Environment.NewLine +
            "Datum en tijd: " + strDatumTijd + Environment.NewLine +
            "Reden van consultatie: " + strKlacht;
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

        if (_afspraakGeselecteerd.Moment <= DateTime.Now)
        {
            ToonFout("Alleen toekomstige afspraken kunnen worden geannuleerd.");
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
            bool bGelukt = _svcAfspraak.AnnuleerDoorPatiënt(_afspraakGeselecteerd.Id, Session.GebruikerId);
            if (!bGelukt)
            {
                ToonFout("De afspraak kon niet worden geannuleerd.");
                return;
            }

            VerbergFout();
            LaadAfsprakenUitDatabase();
        }
        catch (Exception ex)
        {
            ToonFout("Annuleren is mislukt: " + ex.Message);
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
