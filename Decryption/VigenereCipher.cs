/*
    Authors: Brandon (bo206), Emmanuel (es555)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    /// <summary>
    /// This represents a sequence of three characters in a text.
    /// It holds the trigram as a string, the individual characters and
    /// the position in the original text that the trigram starts. This
    /// is used when finding the spacing between duplicates.
    /// </summary>
    public class Trigram
    {
        public string Text { get;}
        
        public char[] Characters { get; }
        
        public int StartingIndex { get; }
        
        public Trigram(char[] characters, int startingIndex)
        {
            Characters = characters;
            Text = new string(characters);
            StartingIndex = startingIndex;
        }
    }
    
    public static class VigenereCipher
    {

        public static string DecryptWithKey(Text plainText)
        {
            const string key = "TESSOFTHEDURBERVILLES";
            var cipherTextString = File.ReadAllText("cexercise2.txt");
            var cipherText = new Text(cipherTextString);

            return DecryptWithKey(plainText, cipherText, key);
        }

        public static string DecryptWithoutKey(Text plainText, string file)
        {
            var cipherTextString = File.ReadAllText(file);
            var cipherText = new Text(cipherTextString);

            // Generate all the possible trigrams.
            var allTrigrams = GenerateTrigrams(cipherText);
            
            // Create a list of groups. The list contains trigrams groupings of trigram objects which appear more than once.
            // The list is ordered by the most occurrences.
            var groupedTrigrams = allTrigrams.GroupBy(x => x.Text).Where(x => x.Count() > 1).OrderByDescending(x => x.Count()).ToList();
             
            // Find the spaces between duplicate trigrams.
            var spaces = GetTrigramSpaces(groupedTrigrams);
            
            // Use the spaces to figure out the possible key length.
            var keyLength = FindPossibleKeyLength(spaces);

            // Generate all the possible keys.
            var possibleKeys = FindPossibleKeys(groupedTrigrams, keyLength);

            // Attempt to decrypt the cipher keys which each of the generated keys.
            foreach (var possibleKey in possibleKeys)
            {
                var decryptedText = DecryptWithKey(plainText, cipherText, possibleKey);
                if (!decryptedText.Equals("Failed"))
                    return decryptedText;
            }
            
            return "Failed";
        }

        private static string DecryptWithKey(Text plainText, Text cipherText, string key)
        {
            // Separate the key into individual characters.
            var keyArray = key.ToCharArray();

            var decryptedCharArray = new char[cipherText.OriginalText.Length];
            for (var i = 0; i < cipherText.OriginalText.Length; i++)
            {
                // Using modulo, wrap the index of the key character between 0 and the length of the key
                // Equivalent to producing:
                // Text: ABCDEFGHIJ
                // Key:  HELLOHELLO
                var keyIndex = i % (key.Length);

                decryptedCharArray[i] = GetPlainTextCharacter(cipherText.Characters[i], keyArray[keyIndex]);
            }
            
            var decryptedString = new string(decryptedCharArray);

            return Helper.IsInPlainText(plainText, decryptedString);
        }

        /// <summary>
        /// This generates all the possible three character sequences in a text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static List<Trigram> GenerateTrigrams(Text text)
        {
            var triads = new List<Trigram>();
            
            // Loop through each character in the text.
            for (var i = 0; i < text.Characters.Length; i++)
            {
                // If there are no more sequences of three characters then stop.
                if (i + 3 > text.Characters.Length - 1)
                    break;

                // Generate trigram by selecting the current character and the two characters ahead.
                var triadCharacters = new char[3];
                triadCharacters[0] = text.Characters[i];
                triadCharacters[1] = text.Characters[i + 1];
                triadCharacters[2] = text.Characters[i + 2];

                // Add the trigram to the list.
                triads.Add(new Trigram(triadCharacters, i));
            }

            return triads;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupedTrigram"></param>
        /// <returns></returns>
        private static List<int> GetTrigramSpaces(List<IGrouping<string, Trigram>> groupedTrigram)
        {
            var spaces = new List<int>();

            // Find all the possible spacings for each trigram
            foreach (var group in groupedTrigram)
            {
                // Get all of the trigrams in the current group.
                var trigrams = group.Select(x => x).ToList();

                // Find spaces between the first trigram and all other duplicate trigrams by subtracting the starting index.
                // An index of 1 is used to ensure we don't compare the first trigram with itself.
                for (var i = 1; i < trigrams.Count; i++)
                    spaces.Add(trigrams[i].StartingIndex - trigrams[0].StartingIndex);
            }

            return spaces;
        }

        /// <summary>
        /// The possible key length is found by determining the most common divisor (out of 4, 5 and 6)
        /// of the trigrams' spaces. The most common divisor is used as a probable key length.
        /// </summary>
        /// <param name="spaces"></param>
        /// <returns></returns>
        private static int FindPossibleKeyLength(List<int>spaces)
        {
            var modFourOccurrences = 0;
            var modFiveOccurrences = 0;
            var modSixOccurrences = 0;

            // Determine if 4, 5 or 6 is a divisor of the space.
            // If so we increment the required counter.
            foreach (var space in spaces)
            {
                if (space % 4 == 0)
                    modFourOccurrences++;
                if (space % 5 == 0)
                    modFiveOccurrences++;
                if (space % 6 == 0)
                    modSixOccurrences++;
            }

            // Find which occured the most.
            var mostOccurred = Math.Max(modFourOccurrences, Math.Max(modFiveOccurrences, modSixOccurrences));
            
            if (mostOccurred == modFourOccurrences)
                return 4;

            return mostOccurred == modFiveOccurrences ? 5 : 6;
        }

        /// <summary>
        /// This finds all of the possible keys.
        /// Two common words are selected. In this case THE and AND
        /// as they are the most common trigrams in the english language.
        /// Each trigram is used to find out the key characters used to form the word THE
        /// and then the word AND. These are placed into two separate lists.
        /// The idea is that when combining the two key sections, it will
        /// account for the spacing of the trigrams in such a way that
        /// the trigrams for THE and AND appear spaces in which they key size is a divisor for.
        /// </summary>
        /// <param name="cipherTextTrigrams"></param>
        /// <param name="keyLength"></param>
        /// <returns></returns>
        private static List<string> FindPossibleKeys(List<IGrouping<string, Trigram>> cipherTextTrigrams, int keyLength)
        {
            var possibleKeysFirstSection = new List<string>();
            var possibleKeysSecondSection = new List<string>();

            var trigrams = cipherTextTrigrams.SelectMany(x => x).ToList();

            foreach (var trigram in trigrams)
            {
                var keySectionOne = GetKeySection(trigram, 3, "THE");
                var keySectionTwo = GetKeySection(trigram, Math.Abs(3 - keyLength), "AND");
            
                possibleKeysFirstSection.Add(keySectionOne);
                possibleKeysSecondSection.Add(keySectionTwo);
            }

            // Combine the two sections to create a list of all of the possible keys with those two common words.
            var possibleKeys = (from theKey in possibleKeysFirstSection from andKey in possibleKeysSecondSection select theKey + andKey).ToList();
            possibleKeys.AddRange((from theKey in possibleKeysFirstSection from andKey in possibleKeysSecondSection select andKey + theKey).ToList());

            return possibleKeys;
        }

        /// <summary>
        /// Get a section of the key.
        /// This is used to determine how many characters of the common
        /// word was used, depending on the key size.
        /// </summary>
        /// <param name="cipherTextTrigram"></param>
        /// <param name="sectionLength"></param>
        /// <param name="commonWord"></param>
        /// <returns></returns>
        private static string GetKeySection(Trigram cipherTextTrigram, int sectionLength, string commonWord)
        {
            var keySection = new char[sectionLength];

            for (int i = 0; i < sectionLength; i++)
            {
                keySection[i] = GetPlainTextCharacter(cipherTextTrigram.Characters[i], commonWord[i]);
            }

            return new string(keySection);

        }

        private static char GetPlainTextCharacter(char encryptedChar, char keyChar)
        {
            var difference = encryptedChar - keyChar;
            return difference < 0 ? Helper.WrapCharacter(91 - Math.Abs(difference)) : Helper.WrapCharacter(65 + difference);
        }
        
    }
}