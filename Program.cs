﻿using System;
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
            var tess26CharacterFrequency = new CharacterFrequency(tess26Text);
            var tess27CharacterFrequency = new CharacterFrequency(tess27Text);

            // Decrypt
            Console.WriteLine("Ceasear: " + CaesarCipher.Decrypt(tess26CharacterFrequency) + "\n");
            Console.WriteLine("Vigenere With Key: " + VigenereCipher.DecryptWithKey(tess26CharacterFrequency) + "\n");
            Console.WriteLine("Vigenere Without Key: " + VigenereCipher.DecryptWithoutKey(tess26CharacterFrequency) + "\n");

        }
        
    }
}