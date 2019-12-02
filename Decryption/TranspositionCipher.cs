using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class TranspositionCipher
    {

        // If you don't the keysize and know the order, put in the right coloumn size then
        // Know numb of chars in cipher text
        // Divide by possible length of keysizes to get columns
        // Read column downs to form english 
        public static string DecryptOrderKnown(Text plainText)
        {
            var cipherTextString = File.ReadAllText("cexercise5.txt");
            var cipherText = new Text(cipherTextString);

            var possibleColumnSizes = new [] {4, 5, 6};

            foreach (var possibleColumnSize in possibleColumnSizes)
            {
                var decryptedText = "";
                float rowLengthFloat = cipherText.OriginalText.Length / possibleColumnSize;
                var rowLength = (int) Math.Ceiling(rowLengthFloat);
                var grid = GenerateGrid(cipherText.OriginalText, rowLength);

                for (int i = 0; i < rowLength; i++)
                {
                    decryptedText += ReadColumn(i, grid);
                }

                if (plainText.OriginalText.Contains(decryptedText))
                    return decryptedText.Substring(0, 30);;

            }

            return "Failed";

        }
        
        public static string DecryptOrderUnknown(Text plainText)
        {
            var cipherTextString = File.ReadAllText("cexercise6.txt");
            var cipherText = new Text(cipherTextString);
            
            var decryptedText = "";
            float rowLength = cipherText.OriginalText.Length / 6;
            var grid = GenerateGrid(cipherText.OriginalText, (int) Math.Ceiling(rowLength));

            var gridPermutations = GenerateGridPermutations(grid);

            foreach (var gridPermutation in gridPermutations)
            {
                for (int i = 0; i < rowLength; i++)
                {
                    decryptedText += ReadColumn(i, gridPermutation);
                }

                if (plainText.OriginalText.Contains(decryptedText))
                    return decryptedText.Substring(0, 30);;
                
                decryptedText = "";
            }
            
            return "Failed";
        }

        private static string ReadColumn(int columnIndex, IEnumerable<string> grid)
        {
            var text = "";
            foreach (var row in grid)
            {
                text += row[columnIndex];
            }

            return text;
        }

        private static List<string> GenerateGrid(string text, int rowLength)
        {
            return text.Select((c, i) => new { Char = c, Index = i }).GroupBy(o => o.Index / rowLength).Select(g => new string(g.Select(o => o.Char).ToArray())).ToList();
        }
        
        private static IEnumerable<IEnumerable<string>> GenerateGridPermutations(List<string> grid)
        {
            return GetPermutations(grid, 6).ToList();
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            return length == 1 ? list.Select(t => new T[] { t }) : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)),(t1, t2) => t1.Concat(new T[] { t2 }));
        }


        
    }
}