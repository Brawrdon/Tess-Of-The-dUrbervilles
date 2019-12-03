/*
    Authors: Brandon (bo206)
*/

using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class CaesarCipher
    {
        public static string Decrypt(Text plainText)
        {
            var text = File.ReadAllText("cexercise1.txt");
            var characterFrequency = new Text(text);

            // Find the most frequent character in the cipher text.
            // Assumed to be E as that is the most common letter in the English language.
            int mostFrequentCharShifted = characterFrequency.MostFrequentCharacter;

            // Keep track of the number of shifts.
            var shifts = 0;

            // Shift the most frequent character in the cipher text by one until it matches the most frequent character
            // in the plain text. The amount of shifts is used as an assumption for the rest of the cipher text.
            do
            {
                mostFrequentCharShifted -= 1;

                // Chars are represented by their ASCII equivalent so they must be wrapped to ensure
                // they stay within 65 - 90 (A - Z)
                mostFrequentCharShifted = Helper.WrapCharacter(mostFrequentCharShifted);

                shifts++;
            } while (plainText.MostFrequentCharacter != mostFrequentCharShifted);


            // Apply the formula to decrypt p = c − k mod 26 to every character in the cipher text
            var decryptedCharArray = characterFrequency.Characters.Select(x => Helper.WrapCharacter(x - (shifts % 26))).ToArray();
            var decryptedString = new string(decryptedCharArray);

            return Helper.IsInPlainText(plainText, decryptedString);
        }
    }
}