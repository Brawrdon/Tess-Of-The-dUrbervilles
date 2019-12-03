/*
    Authors: Brandon (bo206), Emmanuel (es555)
*/

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

            // Assume character that occurs the most is | as this represents punctuation / spaces, replace with a space for now.
            decryptedText = SubstituteCharacter(decryptedText, cipherText.MostFrequentCharacter, ' ');

            // Assuming the spacing is correct, use it to find all of the trigrams
            // in the text. Order them by the ones that occur that most.
            var decryptedSegments = new string(decryptedText).Split(' ');
            var trigrams = decryptedSegments
                .Where(c => c.Length == 3)
                .GroupBy(c => c)
                .Select(c => new {Char = c.Key, Count = c.Count()})
                .ToDictionary(x => x.Char, x => x.Count)
                .OrderByDescending(x => x.Value)
                .Where(x => x.Value > 1)
                .Select(x => x.Key).ToList();
            
            // Lowercase characters are used to make it easier when manually deciphering the text.
            
            // Assume the most occuring trigram is THE. 
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][0], 't');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][1], 'h');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[0][2], 'e');
            
            // Assume the previous is correct and assume the next most occuring trigram is AND.
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][0], 'a');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][1], 'n');
            decryptedText = SubstituteCharacter(decryptedText, trigrams[2][2], 'd');
            
            // Previous assumption seems correct. Manually search the text for patters of other common words
            // such as them. Use the fact that certain characters have already been used to guess potential words.
            // Continue until entire text has been deciphered.
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
            
            // Replace the spaces with original | character and make all the characters upercase.
            var decryptedString = new string(decryptedText).Replace(' ', '|').ToUpper();
            return Helper.IsInPlainText(plainText, decryptedString);
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