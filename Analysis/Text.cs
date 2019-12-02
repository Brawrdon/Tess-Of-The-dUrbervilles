using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TessOfThedUrbervilles.Analysis
{
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