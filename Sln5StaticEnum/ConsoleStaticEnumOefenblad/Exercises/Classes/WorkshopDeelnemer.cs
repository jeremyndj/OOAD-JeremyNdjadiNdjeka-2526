namespace ConsoleStaticEnumOefenblad.Exercises.Classes;

// Gegeven in "Exercises/Classes" is een klasse "WorkshopDeelnemer" met volgende properties:
//  - Naam (private setter)
//  - IsAanwezig (private setter)
//  - een constructor met parameters voor naam en aanwezigheidsstatus
//    
// 1. Voeg volgende static properties toe aan die klasse:
//  - een static property AantalAangemaakt (private setter)
//  - een static property AantalAanwezig (private setter)
//
// 2. Pas de constructor aan zodat de statische properties aangepast worden.
//
// 3. Voeg daarna een (object)methode ZetAfwezig() toe.
//   - die methode moet de aanwezigheidsstatus aanpassen én het globale aantal aanwezigen correct houden
internal class WorkshopDeelnemer
{
    public string Naam { get; set; }
    public bool IsAanwezig { get; private set; }

    public static int AantalAangemaakt { get; private set; }
    public static int AantalAanwezig { get; private set; }

    public WorkshopDeelnemer(string naam, bool isAanwezig)
    {
        Naam = naam;
        IsAanwezig = isAanwezig;
        AantalAangemaakt++;
        if (isAanwezig) 
        { 
            AantalAanwezig++;
        }

    }

    public void ZetAfwezig()
    {
        if (IsAanwezig)
        {
            AantalAanwezig--;
        }
        IsAanwezig = false;
    }
}