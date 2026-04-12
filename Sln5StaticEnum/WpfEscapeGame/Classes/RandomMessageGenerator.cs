using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfEscapeGame.Enums;

namespace WpfEscapeGame.Classes
{
    internal static class RandomMessageGenerator
    {
        private static Random _random = new Random();

        private static string[] PastNiet = { "DThis is not the right size.", "Maybe i should look for something.", "I should look for something smaller." };

        private static string[] KanNiet = { "You can't do this.", "Try something else.", "Not possible." };

        private static string[] Gelukt = { "Great success", "Succeeded", "Victoryyyy!" };

        public static string GetRandomMessage(MessageType bericht)
        {
            try
            {
                string[] berichten;

                switch (bericht)
                {
                    case MessageType.PastNiet:
                        berichten = PastNiet;
                        break;

                    case MessageType.KanNiet:
                        berichten = KanNiet;
                        break;

                    case MessageType.Gelukt:
                        berichten = Gelukt;
                        break;

                    default: throw new Exception("Iets is fout");
                }
                return berichten[_random.Next(berichten.Length)];
            }
            catch (Exception e)
            {
                throw new Exception("Methode is niet gelukt");
            }
        }
    }
}
