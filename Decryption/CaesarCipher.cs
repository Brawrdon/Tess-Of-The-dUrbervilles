using System.IO;
using System.Linq;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class CaesarCipher
    {
        // ASCII A = 65, Z = 90
        public static string Decrypt(CharacterFrequency originalCharacterFrequency)
        {
            var text = File.ReadAllText("cexercise1.txt");
            var characterFrequency = new CharacterFrequency(text);

            int mostFrequentCharShifted = characterFrequency.MostFrequentCharacter;
            
            // Shift minus one letter until they are equal
            var shifts = 0;
            do
            {
                mostFrequentCharShifted -= 1;

                if (mostFrequentCharShifted == 64)
                    mostFrequentCharShifted = 90;

                shifts++;
            } while (originalCharacterFrequency.MostFrequentCharacter != mostFrequentCharShifted);

            
            // Apply decrypt formula
            var decryptedCharArray = characterFrequency.Characters.Select(x => WrapCharacter(x - (shifts % 26)) ).ToArray();
            var decryptedString = new string(decryptedCharArray);

            return originalCharacterFrequency.OriginalText.Contains(decryptedString) ? decryptedString.Substring(0, 30) : "Failed";
        }
        
        private static char WrapCharacter(int character)
        {
            if (character < 65)
                character += 26;

            return (char) character;
        }
        
        

    }
}