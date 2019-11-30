using System;
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
            var decryptedIntArray = characterFrequency.Characters.Select(x => Modulas(x - (shifts % 26)) ).ToArray();
            var decryptedStringArray = Array.ConvertAll(decryptedIntArray, x => (char) x);
            var decryptedString = string.Join("", decryptedStringArray);
            
            return decryptedString.Substring(0, 30);
        }
        
        private static int Modulas(int input)
        {
            const int aCharValue = 65;
            const int zCharValue = 91; // Z is actually 90 but this modula formula wraps from aCharValue to zCharValue - 1
            return (((input - aCharValue) % (zCharValue - aCharValue)) + (zCharValue - aCharValue)) % (zCharValue - aCharValue) + aCharValue;

        }
        
        

    }
}