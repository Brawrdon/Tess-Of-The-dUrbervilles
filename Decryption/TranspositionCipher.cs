/*
    Authors: Brandon (bo206)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class TranspositionCipher
    {
        /// <summary>
        /// Exercise 5.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string DecryptOrderKnown(Text plainText)
        {
            var cipherTextString = File.ReadAllText("cexercise5.txt");
            var cipherText = new Text(cipherTextString);

            var possibleColumnSizes = new [] {4, 5, 6};

            
            // Brute force by attempting to split the cipher text into the possible column sizes.
            // Read horizontally down the grid to generate the decrypted text.
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
        
        /// <summary>
        /// Exercise 6.
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string DecryptOrderUnknown(Text plainText)
        {
            var cipherTextString = File.ReadAllText("cexercise6.txt");
            var cipherText = new Text(cipherTextString);
            
            var decryptedText = "";
            float rowLength = cipherText.OriginalText.Length / 6;
            
            // Generate a grid of 6 columns and x amount of rows.
            var grid = GenerateGrid(cipherText.OriginalText, (int) Math.Ceiling(rowLength));

            // Get all the possible permutations of the grids.
            var gridPermutations = GenerateGridPermutations(grid);

            // Go through each permutation and attempt to decipher.
            // One of the permutations will be in the order of the columns used
            // to encrypt the text.
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

        /// <summary>
        /// Generates a string by reading down the grid at the specified column index.
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static string ReadColumn(int columnIndex, IEnumerable<string> grid)
        {
            var text = "";
            foreach (var row in grid)
            {
                text += row[columnIndex];
            }

            return text;
        }

        /// <summary>
        /// Generate the grid.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rowLength"></param>
        /// <returns></returns>
        private static List<string> GenerateGrid(string text, int rowLength)
        {
            return text.Select((c, i) => new { Char = c, Index = i }).GroupBy(o => o.Index / rowLength).Select(g => new string(g.Select(o => o.Char).ToArray())).ToList();
        }
        
        /// <summary>
        /// Get all the possible permutations of the grid.
        /// This creates all the possible orders (720).
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<string>> GenerateGridPermutations(List<string> grid)
        {
            return GetPermutations(grid, 6).ToList();
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            return length == 1 ? list.Select(x => new [] { x}) : GetPermutations(list, length - 1).SelectMany(x => list.Where(z => !x.Contains(z)),(x1, x2) => x1.Concat(new [] { x2 }));
        }
    }
}