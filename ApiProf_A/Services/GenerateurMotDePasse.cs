using System;
using System.Linq;
namespace ApiProf_A.Services
{
    public class GenerateurMotDePasse
    {
        private static readonly string[] symbols = { "!", "@", "#", "$", "%", "^", "&", "*" };
        public static string generationMotDePasse() {

            string Prefix = "Tecc";
            string quatreNumRandom = generer4NumAleatoire();
            string symbol = genererSymboleAleatoire();
            string motDePasseSansPrefix = placerSymboleAvantOuApresChiffres(quatreNumRandom, symbol);
            string mdpFinal = $"{Prefix}{motDePasseSansPrefix}";
            return symbols[0];
        }

        private static string generer4NumAleatoire() { 
            Random rand = new Random();
            return new string(Enumerable.Range(0, 4).Select(_ => (char)('0' + rand.Next(10))).ToArray());
        }

        private static string genererSymboleAleatoire() {
            Random random = new Random();
            int index = random.Next(symbols.Length);
            return symbols[index];
        }


        private static string placerSymboleAvantOuApresChiffres(string quatreNumRandom, string symbol) {
            Random random = new Random();
            bool k = random.Next(2) == 0;
            //si k alors devant et inverse
            if (k)
            {
                return $"{symbol}{quatreNumRandom}";
            }
            else
            {
                return $"{quatreNumRandom}{symbol}";
            }
        }
    }
}
