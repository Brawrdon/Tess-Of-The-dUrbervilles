/*
    Authors: Brandon (bo206), Emmanuel (es555)
*/

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class Helper
    {
        /// <summary>
        /// This is a helper method that ensures that the characters
        /// stay within the 26 letter alphabets. Because A is 65 in ASCII,
        /// if the character is below, adding 26  (the size of the alphabet)
        /// will "wrap" it around the Z (90 in ASCII) producing the correct character.
        /// If the character is over 90, 26 is subtracted from it giving the correct character.
        /// </summary>
        /// <param name="character">The character to wrap.</param>
        /// <returns></returns>
        public static char WrapCharacter(int character)
        {
            if (character < 65)
                character += 26;

            if (character > 90)
                character -= 26;
        
            return (char) character;
        }

        public static Dictionary<string, int> FindBasedOnWordOccurrence(List<string> texts, string wordToFind, int minOccurrence)
        {
            var frequencies = new Dictionary<string, int>();
            for (var index = 0; index < texts.Count; index++)
            {
                var text = texts[index];
                var count = Regex.Matches(text, wordToFind).Count;
                if (count >= minOccurrence)
                {
                    frequencies.TryAdd(text, count);
                }
            }

            frequencies = frequencies.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            return frequencies;

        }

        /// <summary>
        /// Helper to confirm the decrypted text found exists in the plain text.
        /// This isn't used to just to search the text during decryption, it's used
        /// to make it faster than continuously pressing CTRL+F so that the program can be run
        /// automatically.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="expectedDecryption">The string to check.</param>
        /// <returns>Substring of the decrypted plain text or failed if it is incorrect.</returns>
        public static string IsInPlainText(Text plainText, string expectedDecryption)
        {
            return plainText.OriginalText.Contains(expectedDecryption) ? expectedDecryption.Substring(0, 30) : "Failed";
        }
        
    }
}