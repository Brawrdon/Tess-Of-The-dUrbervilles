/*
    Authors: Brandon (bo206), Emmanuel (es555)
*/

using System.Collections.Generic;
using System.Linq;

namespace TessOfThedUrbervilles.Analysis
{
    /// <summary>
    /// The class to represent inputs from text files.
    /// The class parses text files to produce an object that contains the original text,
    /// the individual characters of the text and the amount of times characters appear.
    /// </summary>
    public class Text
    {
        public string OriginalText { get; }
        
        public char[] Characters { get; }
        
        public Dictionary<char, int> CharacterFrequencies { get; }
        
        public char MostFrequentCharacter { get; }


        public Text(string text)
        {
            OriginalText = text;
            Characters = text.ToCharArray();
            CharacterFrequencies = GetFrequencies();
            MostFrequentCharacter = GetMostFrequentCharacter();
        }
        
        private Dictionary<char, int> GetFrequencies()
        {
            return OriginalText.GroupBy(c => c).Select(c => new {Char = c.Key, Count = c.Count()}).ToDictionary(x => x.Char, x => x.Count);
        }

        private char GetMostFrequentCharacter()
        {
            return CharacterFrequencies.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        }
    }
}