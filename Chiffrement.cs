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
            Console.WriteLine($"Ciphered message: {cipheredMessage}");
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
            //L'input est un message encodé en base64, on doit le décoder.
            var decodedBytes = Convert.FromBase64String(message); 
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
            
            var transposedArray = GetTransposedArray(decrypted, cle);
            var arr = new char[transposedArray.GetLength(0), transposedArray.GetLength(1)];
            
            // Calculer la longueur de chaque colonne (c'est aussi le nombre de rangées)
            var columnLength = (int)Math.Ceiling((double)decrypted.Length / cle.Length);
            var emptyPositions = cle.Length - (int)Math.Ceiling((double)decrypted.Length % cle.Length);
            var index = 0;
            for (var idxCle = 0; idxCle < cle.Length; idxCle++)
            {
                // Si l'index n'à pas été modifié (sinon, on garde l'index modifié)
                // Voir référence: #modify-index#
                if (index == (idxCle - 1) * columnLength)
                {
                     index = idxCle * columnLength;
                }
                
                // Calculer la prochaine quantité de caractères à placer dans le tableau
                var quantityToGet = columnLength;
                /*
                 * Si nous sommes proches de la fin du string "decrypted"
                 */
                if (columnLength + idxCle * columnLength > decrypted.Length)
                {
                    quantityToGet = decrypted.Length - columnLength * idxCle;
                    
                    if (columnLength + (idxCle * columnLength - 1) > decrypted.Length)
                    {
                        quantityToGet = decrypted.Length - (idxCle * columnLength - 1);
                    }
                }
                else if (columnLength + index > decrypted.Length)
                {
                    quantityToGet = decrypted.Length - index;
                }
                /*
                 *  #modify-index#
                 * 
                 *  Si la colonne contient une case vide à la fin à cause que le texte n'est pas de longueur = "columnLength(rowsCount) * cle.Length(columnCount)"
                 *  Alors on va modifier l'index pour récupérer seulement la quantité nécessaires de lettres (
                 *
                 *  Exemple: Les deux cases blanches des colonnes 8 et 6 dans les notes de cours.
                 *  Ces cases ne doivent pas compter comme des espaces, donc on doit considérer une longueur de colonne columnLength - 1 
                 */
                if (cle.Length - idxCle <= emptyPositions)
                {
                    index = idxCle * columnLength - 1;
                }
                
                var charactersGotten = decrypted.Substring(index, quantityToGet);
                
                var positionOfIdxCleInCle = cle.IndexOf((idxCle + 1).ToString()[0]);    // (idxCle + 1) car les chiffres de la clé commencent à 1 et non à 0 
                /*
                 * Si nous sommes dans une colonne où il devrait y avoir une case vide à la fin
                 * ET QUE
                 * L'index n'a pas été modifié 
                 */
                if (positionOfIdxCleInCle >= cle.Length - emptyPositions && index == idxCle * columnLength)
                {
                    charactersGotten = decrypted.Substring(index, quantityToGet - 1);
                }
                
                // Placer les lettres dans le tableau au bon endroit (où 'i' se trouve dans la clé)
                for (var targetRow = 0; targetRow < arr.GetLength(0); targetRow++)
                {
                    // Si on est à la fin du tableau et qu'il y a moins de lettre que d'espaces à combler, alors on comble avec des espaces
                    arr[targetRow, cle.IndexOf((idxCle + 1).ToString()[0])] = targetRow >= charactersGotten.Length ? ' ' : charactersGotten[targetRow];
                }
            }

            // Construire une string à partir du tableau maintenant bien ordonné
            var result = new StringBuilder();
            for (var col = 0; col < arr.GetLength(0); col++)
            {
                for (var row = 0; row < arr.GetLength(1); row++)
                {
                    result.Append(arr[col, row]);
                }
            }
            
            return result.ToString();
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