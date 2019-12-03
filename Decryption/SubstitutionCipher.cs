using System;
using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class SubstitutionCipher
    {
        public static string Decrypt(Text plainText)
        {
            var cipherTextString = File.ReadAllText("cexercise7.txt");
            var cipherText = new Text(cipherTextString);

            var decryptedText = cipherText.OriginalText.ToCharArray();

            // Assume character that occurs the most is |, replace with a space for now
            decryptedText = SubstituteCharacter(decryptedText, cipherText.MostFrequentCharacter, ' ');

            // Assume the most common trigram is THE
            var decryptedSegments = new string(decryptedText).Split(' ');
            var trigrams = decryptedSegments
                .Where(c => c.Length == 3)
                .GroupBy(c => c)
                .Select(c => new {Char = c.Key, Count = c.Count()})
                .ToDictionary(x => x.Char, x => x.Count)
                .OrderByDescending(x => x.Value)
                .Where(x => x.Value > 1)
                .Select(x => x.Key).ToList();
            
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][0], 't');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][1], 'h');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][2], 'e');
            
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][0], 'a');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][1], 'n');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][2], 'd');
            
            decryptedText = SubstituteCharacter(decryptedText, '|', 'r');
            decryptedText = SubstituteCharacter(decryptedText, 'A', 'i');
            decryptedText = SubstituteCharacter(decryptedText, 'B', 'c');
            decryptedText = SubstituteCharacter(decryptedText, 'C', 'x');
            decryptedText = SubstituteCharacter(decryptedText, 'D', 'g');
            decryptedText = SubstituteCharacter(decryptedText, 'E', 'f');
            decryptedText = SubstituteCharacter(decryptedText, 'F', 's');
            decryptedText = SubstituteCharacter(decryptedText, 'G', 'y');
            decryptedText = SubstituteCharacter(decryptedText, 'I', 'u');
            decryptedText = SubstituteCharacter(decryptedText, 'L', 'p');
            decryptedText = SubstituteCharacter(decryptedText, 'N', 'b');
            decryptedText = SubstituteCharacter(decryptedText, 'O', 'w');
            decryptedText = SubstituteCharacter(decryptedText, 'P', 'v');
            decryptedText = SubstituteCharacter(decryptedText, 'R', 'o');
            decryptedText = SubstituteCharacter(decryptedText, 'S', 'k');
            decryptedText = SubstituteCharacter(decryptedText, 'T', 'q');
            decryptedText = SubstituteCharacter(decryptedText, 'U', 'l');
            decryptedText = SubstituteCharacter(decryptedText, 'V', 'm');
            decryptedText = SubstituteCharacter(decryptedText, 'X', 'j');
            
            var decryptedString = new string(decryptedText).Replace(' ', '|').ToUpper();
            return plainText.OriginalText.Contains(decryptedString) ? decryptedString.Substring(0, 30) : "Failed";
        }
        
        
        private static char[] SubstituteCharacter(char[] textCharArray, char toSubstitute, char newChar)
        {
            for (int i = 0; i < textCharArray.Length; i++)
            {
                if (textCharArray[i] == toSubstitute)
                {
                    textCharArray[i] = newChar;
                }
            }

            return textCharArray;
        }
    }
    

}