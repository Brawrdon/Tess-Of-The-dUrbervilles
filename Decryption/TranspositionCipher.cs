using System;
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
                float rowLength = cipherText.OriginalText.Length / possibleColumnSize;
                var grid = GenerateGrid(cipherText.OriginalText, (int) Math.Ceiling(rowLength));

                for (int i = 0; i < grid.Count(); i++)
                {
                    var columnIndex = i % (possibleColumnSize + 1);
                    decryptedText += ReadColumn(columnIndex, grid);
                }

                if (plainText.OriginalText.Contains(decryptedText)) 
                    return decryptedText;

            }

            return "Failed";

        }

        private static string ReadColumn(int columnIndex, List<string> grid)
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

        
        
        // If you don't know the order and know the keysize
        // Divide by the cipher text by the keysize
        // 

        
    }
}