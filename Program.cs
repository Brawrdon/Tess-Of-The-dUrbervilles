using System;
using System.IO;
using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // Read Files 
            string tess26Text;
            string tess27Text;

            try
            {
                tess26Text = File.ReadAllText("tess26.txt");
                tess27Text = File.ReadAllText("tess27.txt");

            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw exception;
            }
            
            
            // Get Character Frequencies
            var tess26CharacterFrequency = new Text(tess26Text);
            var tess27CharacterFrequency = new Text(tess27Text);

            // Decrypt
            Console.WriteLine("Ceasear: " + CaesarCipher.Decrypt(tess26CharacterFrequency) + "\n");
            Console.WriteLine("Vigenere With Key: " + VigenereCipher.DecryptWithKey(tess26CharacterFrequency) + "\n"); 
            Console.WriteLine("Vigenere Without Key (Known Size of 6): " + VigenereCipher.DecryptWithoutKey(tess26CharacterFrequency, "cexercise3.txt") + "\n");
            Console.WriteLine("Vigenere Without Key (Known Size of 4 - 6): " + VigenereCipher.DecryptWithoutKey(tess26CharacterFrequency, "cexercise4.txt") + "\n");
            Console.WriteLine("Transposition (Known Size of 4 - 6, Order Known): " + TranspositionCipher.DecryptOrderKnown(tess26CharacterFrequency) + "\n");
            Console.WriteLine("Transposition (Known Size of 6, Order Unknown): " + TranspositionCipher.DecryptOrderUnknown(tess26CharacterFrequency) + "\n");
            Console.WriteLine("Substitution: " + SubstitutionCipher.Decrypt(tess27CharacterFrequency));
        }
        
    }
}