using System;

namespace PIF1006_TP3
{
    class Program
    {
        static void Main(string[] args)
        {
            var chiffrer = Chiffrement.Chiffrer("ce cours de mathématiques est très intéressant", "71452386");
            var dechiffrer = Chiffrement.Dechiffrer(chiffrer, "71452386");
            Console.WriteLine($"Chiffrer: {chiffrer}");
            Console.WriteLine($"Dechiffrer: {dechiffrer}");
        }
    }
}