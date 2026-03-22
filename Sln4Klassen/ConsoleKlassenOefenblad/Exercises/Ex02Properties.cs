using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex02Properties
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: properties, standaardwaarden, object initializer syntax");
        Console.WriteLine("-------------");
        // 1. maak in "Exercises/Classes" een klasse "Recept" met volgende properties:
        //   - Titel
        //   - Rating (int)
        //   - IsVegetarisch (standaardwaarde is false)
        //   - Ingredienten (List van strings, standaard lege lijst)

        // 2. maak volgend recept aan met de lege constructor (... = new Recept()) en stel dan één voor één de properties in:
        //   - Pasta Carbonara (Rating 4, Ingrediënten: Pasta, Eieren, Spek, Parmezaanse kaas)
        // ...
        Recept pastaCarbonara = new Recept();
        pastaCarbonara.Titel = "Pasta Carbonara";
        pastaCarbonara.Rating = 4;
        pastaCarbonara.IsVegetarisch = false;
        pastaCarbonara.Ingrediënten = new List<string> { "Pasta", "Eieren", "Spek", "Parmezaanse kaas" };

            //"Pasta Carbonara", 4, false, "Pasta, Eieren, Spek, Parmezaanse kaas"

        // 3. maak volgende recepten aan met de object initializer syntax:
        //   - Lasagne (Rating 5, IsVegetarisch false, Ingrediënten: Lasagnebladen, Tomatensaus, Courgette, Aubergine, Mozzarella)
        //   - Salade Niçoise (Rating 4, Ingrediënten: Sla, Tonijn, Eieren, Pindakaas, Olijven, Tomaten)
        // ...
        Recept lasagne = new Recept();
        {
            lasagne.Titel = "Lasagne";
            lasagne.Rating = 5;
            lasagne.IsVegetarisch = false;
            lasagne.Ingrediënten = new List<string> { "Lasagnebladen", "Tomatensaus", "Courgette", "Aubergine", "Mozzarella" };
        }

        Recept saladeNicoise = new Recept();
        {
            lasagne.Titel = "Salade Nicoise";
            lasagne.Rating = 4;
            lasagne.IsVegetarisch = true;
            lasagne.Ingrediënten = new List<string> { "Sla", "Tonijn", "Eiren", "Pindakaas", "Olijven", "Tomaten" };
        }

        // 4. pas het recept van de salade niçoise aan:
        //  - verwijder de pindakaas
        //  - zet IsVegetarisch op false
        saladeNicoise.Ingrediënten.Remove("Pindakaas");
        saladeNicoise.IsVegetarisch = false;


        // 5. maak een lijst "kookboek" aan en voeg de drie recepten toe
        // ...
        List<Recept> kookboek = new List<Recept>();
        kookboek.Add(saladeNicoise);
        kookboek.Add(lasagne);
        kookboek.Add(pastaCarbonara);

        // 6. toon het aantal vegetarische recepten (zie screenshot) en de gemiddelde rating
        // ...
        int aantalVegetarisch = kookboek.Count( r => r.IsVegetarisch);
        double gemRating = kookboek.Average( r => r.Rating );
        Console.WriteLine($"Aantal vegetarische recepten: {aantalVegetarisch} ");
        Console.WriteLine($"Gemiddelde rating: {gemRating} ");
    }
}
