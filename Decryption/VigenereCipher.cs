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

        public static string DecryptWithoutKey(CharacterFrequency originalCharacterFrequency)
        {
            var text = File.ReadAllText("cexercise3.txt");
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


            return "";

        }

        
        
        private static char WrapCharacter(int character)
        {
            if (character < 65)
                character += 26;

            return (char) character;
        }

    }
}