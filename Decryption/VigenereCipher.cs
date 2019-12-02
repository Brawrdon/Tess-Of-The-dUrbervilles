using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public class Triad
    {
        public string Text { get;}
        
        public char[] Characters { get; }
        
        public int StartingIndex { get; }
        
        public Triad(char[] characters, int startingIndex)
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

            var allTriads = GenerateTriads(cipherText);
            
            var duplicateTriads = allTriads.GroupBy(x => x.Text).Where(x => x.Count() > 1).OrderByDescending(x => x.Count()).ToList();
             
            var spaces = GetTriadSpaces(duplicateTriads);

            var keyLength = FindPossibleKeyLength(spaces);

            var possibleKeys = FindPossibleKeys(plainText, duplicateTriads, keyLength);

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
            var keyArray = key.ToCharArray();

            var decryptedCharArray = new char[cipherText.OriginalText.Length];
            for (var i = 0; i < cipherText.OriginalText.Length; i++)
            {
                var keyIndex = i % (key.Length);

                decryptedCharArray[i] = GetPossibleCharacter(cipherText.Characters[i], keyArray[keyIndex]);
            }
            
            var decryptedString = new string(decryptedCharArray);

            return plainText.OriginalText.Contains(decryptedString) ? decryptedString.Substring(0, 30) : "Failed";
        }

        private static List<Triad> GenerateTriads(Text text)
        {
            var triads = new List<Triad>();
            
            // Loop through all possible triads in the text
            for (var i = 0; i < text.Characters.Length; i++)
            {
                // If there are no more triads then stop
                if (i + 3 > text.Characters.Length - 1)
                    break;

                // Generate triad
                var triadCharacters = new char[3];
                triadCharacters[0] = text.Characters[i];
                triadCharacters[1] = text.Characters[i + 1];
                triadCharacters[2] = text.Characters[i + 2];

                triads.Add(new Triad(triadCharacters, i));
            }

            return triads;
        }

        private static List<int> GetTriadSpaces(List<IGrouping<string, Triad>> groupedTriads)
        {
            var spaces = new List<int>();

            foreach (var group in groupedTriads)
            {
                var triadsInGroup = group.Select(x => x).ToList();

                // Find spaces between the first triad and all other duplicate triads
                // matches.Count - 1 is used to ensure we stop searching once we reach the last triad.
                for (var i = 1; i < triadsInGroup.Count; i++)
                    spaces.Add(triadsInGroup[i].StartingIndex - triadsInGroup[0].StartingIndex);

            }

            return spaces;
        }

        private static int FindPossibleKeyLength(List<int>spaces)
        {
            var modFourOccurrences = 0;
            var modFiveOccurrences = 0;
            var modSixOccurrences = 0;

            foreach (var space in spaces)
            {
                if (space % 4 == 0)
                    modFourOccurrences++;
                if (space % 5 == 0)
                    modFiveOccurrences++;
                if (space % 6 == 0)
                    modSixOccurrences++;
            }

            var mostOccurred = Math.Max(modFourOccurrences, Math.Max(modFiveOccurrences, modSixOccurrences));
            
            if (mostOccurred == modFourOccurrences)
                return 4;

            return mostOccurred == modFiveOccurrences ? 5 : 6;
        }

        private static List<string> FindPossibleKeys(Text plantText, List<IGrouping<string, Triad>> cipherTextTriads, int keyLength)
        {
            var possibleKeysFirstSection = new List<string>();
            var possibleKeysSecondSection = new List<string>();

            var plainTextTriads = GenerateTriads(plantText);

            var plainTextDuplicateTriads = plainTextTriads.GroupBy(x => x.Text).Where(x => x.Count() > 1).OrderByDescending(x => x.Count()).ToList();

            var commonWordOne = plainTextDuplicateTriads[0].Key;
            var commonWordTwo = plainTextDuplicateTriads[1].Key;

            var triads = cipherTextTriads.SelectMany(x => x).ToList();

            foreach (var triad in triads)
            {
                var keySectionOne = GetKeySection(triad, 3, commonWordOne);
                var keySectionTwo = GetKeySection(triad, Math.Abs(3 - keyLength), commonWordTwo);
            
                possibleKeysFirstSection.Add(keySectionOne);
                possibleKeysSecondSection.Add(keySectionTwo);
            }

            var possibleKeys = (from theKey in possibleKeysFirstSection from andKey in possibleKeysSecondSection select theKey + andKey).ToList();
            possibleKeys.AddRange((from theKey in possibleKeysFirstSection from andKey in possibleKeysSecondSection select andKey + theKey).ToList());

            return possibleKeys;
        }

        private static string GetKeySection(Triad cipherTextTriad, int sectionLength, string commonWord)
        {
            var keySection = new char[sectionLength];

            for (int i = 0; i < sectionLength; i++)
            {
                keySection[i] = GetPossibleCharacter(cipherTextTriad.Characters[i], commonWord[i]);
            }

            return new string(keySection);

        }

        private static char GetPossibleCharacter(char encryptedChar, char keyChar)
        {
            var difference = encryptedChar - keyChar;
            return difference < 0 ? WrapCharacter(91 - Math.Abs(difference)) : WrapCharacter(65 + difference);
        }
        
        private static char WrapCharacter(int character)
        {
            if (character < 65)
                character += 26;

            if (character > 90)
                character -= 26;
            
            return (char) character;
        }

    }
}