using System;
using System.Linq;

namespace PIF1006_TP3
{
    class Program
    {
        public const bool IsDev = true;

        static void Main(string[] args)
        {
            var message = "ce cours de mathematiques est tres interessant";
            if (!IsDev)
            {
                Console.WriteLine("Veuillez entrer le message à chiffrer: ");
                message = Console.ReadLine();
            }
            
            var key = "7 1 4 2 3 5 8 6";
            if (!IsDev)
            {
                Console.WriteLine("Veuillez entrer la clé de chiffrement avec un espace entre chaque nombre (les nombres doivent se suivre et n'avoir qu'un incrément de 1): ");
                key = Console.ReadLine();
            }

            var keyList = key!.Split().Select(int.Parse).ToList();
            
            var chiffrer = Chiffrement.Chiffrer(message, keyList);
            Console.WriteLine($"Chiffrer: {chiffrer}");
            var dechiffrer = Chiffrement.Dechiffrer(chiffrer, keyList);
            Console.WriteLine($"Dechiffrer: {dechiffrer}");
        }
    }
}