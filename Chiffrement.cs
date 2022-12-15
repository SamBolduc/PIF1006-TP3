using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIF1006_TP3
{
    public static class Chiffrement
    {
        public static string Chiffrer(string message, List<int> cle, byte iv)
        {
            var transposed = GetTransposedArray(message, cle);

            var cipheredMessage = CipherMessage(transposed, cle);
            Console.WriteLine($"Ciphered message: {cipheredMessage}");
            var cipheredBytes = Encoding.UTF8.GetBytes(cipheredMessage);

            var res = new byte[cipheredBytes.Length];

            for (var i = 0; i < cipheredBytes.Length; i++)
            {
                if (i == 0)
                {
                    res[i] = (byte)(cipheredBytes[i] ^ iv);
                }
                else
                {
                    res[i] = (byte)(cipheredBytes[i] ^ res[i - 1]);
                }
            }

            var encoded =
                Convert.ToBase64String(
                    res); // Pour que l'algorithme fonctionne avec les caractères spéciaux (UTF-8), on doit coder le message en base64.

            return encoded;
        }

        public static string Dechiffrer(string message, List<int> cle,byte iv)
        {
            //L'input est un message encodé en base64, on doit le décoder.
            var decodedBytes = Convert.FromBase64String(message); 

            var decryptedBytes = new byte[decodedBytes.Length];

            for (var i = 0; i < decodedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(
                    i == 0 
                    ? decodedBytes[i] ^ iv 
                    : decodedBytes[i] ^ decodedBytes[i - 1]
                );
            }

            var decrypted = Encoding.UTF8.GetString(decryptedBytes);
            var decryptedStack = new Stack<char>();
            decrypted.Reverse().ToList().ForEach(c => decryptedStack.Push(c));
            
            var transposedArray = GetTransposedArray(decrypted, cle);
            var arr = new char[transposedArray.GetLength(0), transposedArray.GetLength(1)];
            
            // Calculer la longueur de chaque colonne (c'est aussi le nombre de rangées)
            var columnLength = (int)Math.Ceiling((double)decrypted.Length / cle.Count);
            
            // Si la longueur du message est un multiple de la longueur de la clé, alors il n'y a aucune position vide 
            var emptyPositions = 
                decrypted.Length % cle.Count == 0 
                ? 0 
                : cle.Count - (int)Math.Ceiling((double)decrypted.Length % cle.Count);
            
            for (var idxCle = 0; idxCle < cle.Count; idxCle++)
            {
                // Calculer la prochaine quantité de caractères à placer dans le tableau
                var quantityToGet = columnLength;

                if (cle.IndexOf(idxCle + 1) >= cle.Count - emptyPositions)
                    quantityToGet--;

                var charactersGotten = "";
                for (var i = 0; i < quantityToGet; i++)
                    charactersGotten += decryptedStack.Pop();
                
                // Placer les lettres dans le tableau au bon endroit (où 'i' se trouve dans la clé)
                // Si on est à la fin du tableau et qu'il y a moins de lettre que d'espaces à combler, alors on comble avec des espaces
                for (var targetRow = 0; targetRow < arr.GetLength(0); targetRow++)
                    arr[targetRow, cle.IndexOf((idxCle + 1))] = targetRow >= charactersGotten.Length ? ' ' : charactersGotten[targetRow];
            }

            // Construire une string à partir du tableau maintenant bien ordonné
            var result = new StringBuilder();
            for (var col = 0; col < arr.GetLength(0); col++)
                for (var row = 0; row < arr.GetLength(1); row++)
                    result.Append(arr[col, row]);
            
            return result.ToString();
        }

        private static char[,] GetTransposedArray(string message, List<int>  cle)
        {
            var colCount = cle.Count;
            var rowCount = (int)Math.Ceiling((double)message.Length / colCount);
            var transposed = new char[rowCount, colCount];

            for (var i = 0; i < message.Length; i++)
            {
                var curRow = i / colCount;
                var curCol = i % colCount;
                transposed[curRow, curCol] = message[i];
            }

            return transposed;
        }

        private static string CipherMessage(char[,] transposedArray, List<int> cle)
        {
            var sortedKey = cle.OrderBy(x => x - '0').ToList();
            var res = new List<char>();
            foreach (var col in sortedKey.Select(c => cle.IndexOf(c)))
            {
                for (var row = 0; row < transposedArray.GetLength(0); row++)
                {
                    var val = transposedArray[row, col];
                    if (val != '\0')
                    {
                        res.Add(val);
                    }
                }
            }

            return string.Join("", res);
        }

        // Pour debug
        private static void PrintMatrix(char[,] matrix)
        {
            var ret = new List<string>();

            for (var row = 0; row < matrix.GetLength(0); row++)
            {
                var line = new StringBuilder();
                for (var col = 0; col < matrix.GetLength(1); col++)
                {
                    var val = matrix[row, col];
                    line.Append(val);
                }

                ret.Add(line.ToString());
                ret.Add("");
            }

            var lines = string.Join("\n", ret);
            Console.WriteLine(lines);
        }
    }
}