using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIF1006_TP3
{
    public static class Chiffrement
    {
        public static string Chiffrer(string message, string cle)
        {
            var transposed = GetTransposedArray(message, cle);

            var cipheredMessage = CipherMessage(transposed, cle);
            Console.WriteLine(cipheredMessage);
            var cipheredBytes = Encoding.UTF8.GetBytes(cipheredMessage);

            var iv = (byte)Encoding.UTF8.GetBytes(cle).GetValue(0)!;
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

        public static string Dechiffrer(string message, string cle)
        {
            var decodedBytes =
                Convert.FromBase64String(message); //L'input est un message encodé en base64, on doit le décoder.
            var iv = (byte)Encoding.UTF8.GetBytes(cle).GetValue(0)!;
            var decryptedBytes = new byte[decodedBytes.Length];

            for (var i = 0; i < decodedBytes.Length; i++)
            {
                if (i == 0)
                {
                    decryptedBytes[i] = (byte)(decodedBytes[i] ^ iv);
                }
                else
                {
                    decryptedBytes[i] = (byte)(decodedBytes[i] ^ decodedBytes[i - 1]);
                }
            }

            var decrypted = Encoding.UTF8.GetString(decryptedBytes);

            
            //WIP - Sam
            /*
            var transposedArray = GetTransposedArray(decrypted, cle);

            var arr = new char[transposedArray.GetLength(0), transposedArray.GetLength(1)];
            for (var i = 0; i < cle.Length; i++)
            {
                var c = cle[i];
                var targetCol = i;
                var fromCol = c - '0' - 1;

                for (int targetRow = 0; targetRow < arr.GetLength(0); targetRow++)
                {
                    arr[targetRow, targetCol] = transposedArray[targetRow, fromCol];

                }
            }

            PrintMatrix(arr);*/
            
            return decrypted;
        }

        private static char[,] GetTransposedArray(string message, string cle)
        {
            var colCount = cle.Length;
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

        private static string CipherMessage(char[,] transposedArray, string cle)
        {
            var sortedKey = cle.OrderBy(x => x - '0').ToList();
            var res = new List<char>();
            foreach (var c in sortedKey)
            {
                var col = cle.IndexOf(c);
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