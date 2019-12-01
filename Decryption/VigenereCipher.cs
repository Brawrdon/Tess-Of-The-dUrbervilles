using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public class Sequence
    {

        public string Triad { get; }
        public List<Spacing> Spaces { get; }
        
        public Sequence(string triad)
        {
            Triad = triad;
            Spaces = new List<Spacing>();
        }
        
    }
    
    public class Spacing
    {
        public int Space { get; }
        public bool Mod4 { get; set; }
        public bool Mod5 { get; set; }
        public bool Mod6 { get; set; }

        public Spacing(int space)
        {
            Space = space;
        }
            
    }
    
    public static class VigenereCipher
    {
        public static string DecryptWithKey(CharacterFrequency originalCharacterFrequency)
        {
            const string key = "TESSOFTHEDURBERVILLES";
            var text = File.ReadAllText("cexercise2.txt");
            var characterFrequency = new CharacterFrequency(text);
            
            var keyArray = key.ToCharArray();
            var offsetKeyArray = keyArray.Select(x => x - 65).ToArray();

            var decryptedCharArray = new char[characterFrequency.OriginalText.Length];
            for (var i = 0; i < characterFrequency.OriginalText.Length; i++)
            {
                var keyIndex = i % (key.Length);

                var decryptedChar = characterFrequency.Characters[i] - offsetKeyArray[keyIndex];
                
                decryptedCharArray[i] = WrapCharacter(decryptedChar);
            }
            
            var decryptedString = new string(decryptedCharArray);

            return originalCharacterFrequency.OriginalText.Contains(decryptedString) ? decryptedString.Substring(0, 30) : "Failed";
        }

        public static string DecryptWithoutKey(CharacterFrequency originalCharacterFrequency, string file)
        {
            var text = File.ReadAllText(file);
            var characterFrequency = new CharacterFrequency(text);
            
            var sequences = new List<Sequence>();

            // Loops through all possible triad in the text
            for (var i = 0; i < characterFrequency.Characters.Length; i++)
            {
                // If there are no more triads then stop.
                if (i + 3 > characterFrequency.Characters.Length - 1)
                    break;

                // Generate triad
                var sequenceChars = new char[3];
                sequenceChars[0] = characterFrequency.Characters[i];
                sequenceChars[1] = characterFrequency.Characters[i + 1];
                sequenceChars[2] = characterFrequency.Characters[i + 2];
                
                var sequenceString = new string(sequenceChars);
                
                // Generate a list of all of the triads that have been repeated.
                // sequences.All... checks that we don't loop through triads spacings if it's already been done
                if (sequences.All(x => x.Triad != sequenceString))
                {
                    // Find all the triads in the text 
                    var matches = Regex.Matches(characterFrequency.OriginalText, sequenceString);
                    
                    // Only carry on if duplicates are found
                    if (matches.Count > 1)
                    {
                        var sequence = new Sequence(sequenceString);
                        sequences.Add(sequence);
                        
                        // Get the index of the first letter of the first triad duplicate
                        var firstMatchIndex = matches[0].Index;

                        // Find spaces between the first triad and all other duplicate triads
                        // matches.Count - 1 is used to ensure we stop searching once we reach the last triad.
                        for (var j = 0; j < matches.Count - 1; j++)
                        {
                            var secondMatchIndex = matches[j + 1].Index;
                            
                            var spacing = secondMatchIndex - firstMatchIndex;

                            var space = new Spacing(spacing);
                            sequence.Spaces.Add(space);

                        }
                    }

                }
            }


            // Find possible key length with modulo
            foreach (var sequenceSpace in sequences.SelectMany(sequence => sequence.Spaces))
            {
                sequenceSpace.Mod4 = sequenceSpace.Space % 4 == 0;
                sequenceSpace.Mod5 = sequenceSpace.Space % 5 == 0;
                sequenceSpace.Mod6 = sequenceSpace.Space % 6 == 0;
            }

            // Find highest frequency
            var frequencies = new Dictionary<int, int> {{4, 0}, {5, 0}, {6, 0}};

            foreach (var sequenceSpace in sequences.SelectMany(sequence => sequence.Spaces))
            {
                if (sequenceSpace.Mod4)
                    frequencies[4] = frequencies[4] + 1;                
                if (sequenceSpace.Mod5)
                    frequencies[5] = frequencies[5] + 1;                
                if (sequenceSpace.Mod6)
                    frequencies[6] = frequencies[6] + 1;                
            }
            
            var keySize = frequencies.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;           

            // Split text into segments of the size of the key
            var sequencesInOrder = sequences.OrderByDescending(x => x.Spaces.Count);

            var keysForThe = new List<string>();
            var keysForAnd = new List<string>();

            foreach (var sequence in sequencesInOrder)
            {
                var sequenceCharArray = sequence.Triad.ToCharArray();

                var firstChar = sequenceCharArray[0];
                var secondChar = sequenceCharArray[1];
                var thirdChar = sequenceCharArray[2];
                
                var possibleKeyOne = GetPossibleCharacter(firstChar, 'T');
                var possibleKeyTwo =  GetPossibleCharacter(secondChar, 'H');
                var possibleKeyThree = GetPossibleCharacter(thirdChar, 'E');
                
                var keyCharArray = new [] {possibleKeyOne, possibleKeyTwo, possibleKeyThree};
                keysForThe.Add(new string(keyCharArray));
                
                possibleKeyOne = GetPossibleCharacter(firstChar, 'A');
                possibleKeyTwo =  GetPossibleCharacter(secondChar, 'N');
                possibleKeyThree = GetPossibleCharacter(thirdChar, 'D');

                switch (keySize)
                {
                    case 4:
                        keyCharArray = new [] {possibleKeyOne};
                        break;
                    case 5:
                        keyCharArray = new [] {possibleKeyOne, possibleKeyTwo};
                        break;
                    default:
                        keyCharArray = new [] {possibleKeyOne, possibleKeyTwo, possibleKeyThree};
                        break;
                }
                
                keysForAnd.Add(new string(keyCharArray));


            }

   
            var possibleKeys = (from theKey in keysForThe from andKey in keysForAnd select theKey + andKey).ToList();
            possibleKeys.AddRange((from theKey in keysForThe from andKey in keysForAnd select  andKey + theKey).ToList());
            
            var segments = GetSegments(characterFrequency.OriginalText, keySize);


            foreach (var possibleKey in possibleKeys)
            {
                // Segment original text into key size.

                var decrypted = "";
                var keyIndex = 0;
                foreach (var originalInSegment in segments)
                {
                    
                    // Get array of characters
                    decrypted += new string(GetPossibleCharacters(originalInSegment.ToCharArray(), possibleKey.ToCharArray()));
                }

                if (originalCharacterFrequency.OriginalText.Contains(decrypted))
                    return decrypted.Substring(0, 30);

            }

            return "Failed";
        }

        private static List<string> GetSegments(string text, int keySize)
        {
            return text.Select((c, i) => new { Char = c, Index = i }).GroupBy(o => o.Index / keySize).Select(g => new string(g.Select(o => o.Char).ToArray())).ToList();
        }


        private static char[] GetPossibleCharacters(char[] encryptedCharArray, char[] keyCharArray)
        {
            var translatedBlock = new char[encryptedCharArray.Length];
            for (var i = 0; i < encryptedCharArray.Length; i++)
            {
                translatedBlock[i] = GetPossibleCharacter(encryptedCharArray[i], keyCharArray[i]);
            }

            return translatedBlock;
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