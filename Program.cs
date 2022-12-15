using System;
using System.Linq;
using System.Text;

namespace PIF1006_TP3
{
    class Program
    {
        public const bool IsDev = true;

        static void Main(string[] args)
        {
            var message = "ce cours de mathematiques est tres interessant";

            Console.WriteLine("Veuillez entrer le message à chiffrer: ");
            message = Console.ReadLine();

            
            var key = "7 1 4 5 2 3 8 6 9 15 13 11 14 12 10";

            Console.WriteLine("Veuillez entrer la clé de chiffrement avec un espace entre chaque nombre (les nombres doivent se suivre, n'avoir qu'un incrément de 1 et commencer à 1 (inclusivement): ");
            key = Console.ReadLine();




            Console.WriteLine("Veuillez entrer le vecteur d'initialisation : ");
            var vecteurR = Console.ReadLine();
            var vecteurI = (byte)Encoding.UTF8.GetBytes(vecteurR.ToString()!).GetValue(0)!;


            var keyList = key!.Split().Select(int.Parse).ToList();
            
            var chiffrer = Chiffrement.Chiffrer(message, keyList, (byte)vecteurI);
            Console.WriteLine($"Chiffrer: {chiffrer}");
            var dechiffrer = Chiffrement.Dechiffrer(chiffrer, keyList, (byte)vecteurI);
            Console.WriteLine($"Dechiffrer: {dechiffrer}");
        }
    }
}