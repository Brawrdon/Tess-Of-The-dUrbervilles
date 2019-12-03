/*
    Authors: Brandon (bo206), Emmanuel (es555)
*/

using TessOfThedUrbervilles.Analysis;

namespace TessOfThedUrbervilles
{
    public static class Helper
    {
        /// <summary>
        /// This is a helper method that ensures that the characters
        /// stay within the 26 letter alphabets. Because A is 65 in ASCII,
        /// if the character is below, adding 26  (the size of the alphabet)
        /// will "wrap" it around the Z (90 in ASCII) producing the correct character.
        /// If the character is over 90, 26 is subtracted from it giving the correct character.
        /// </summary>
        /// <param name="character">The character to wrap.</param>
        /// <returns></returns>
        public static char WrapCharacter(int character)
        {
            if (character < 65)
                character += 26;

            if (character > 90)
                character -= 26;
        
            return (char) character;
        }

        /// <summary>
        /// Checks if the decrypted string exists in the plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="potentialDecryption">The string to check.</param>
        /// <returns>Substring of the decrypted plain text or failed if it is incorrect.</returns>
        public static string IsInPlainText(Text plainText, string potentialDecryption)
        {
            return plainText.OriginalText.Contains(potentialDecryption) ? potentialDecryption.Substring(0, 30) : "Failed";
        }
        
    }
}