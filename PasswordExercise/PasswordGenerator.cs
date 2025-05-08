using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordExcercise
{
	public struct PasswordRequirements
	{
		public int MaxLength { get; set; }
		public int MinLength { get; set; }
		public int MinUpperAlphaChars { get; set; }
		public int MinLowerAlphaChars { get; set; }
		public int MinNumericChars { get; set; }
		public int MinSpecialChars { get; set; }
	}

    public interface IPasswordGenerator
	{
		string GeneratePassword(PasswordRequirements requirements);
	}

	public class PasswordGenerator : IPasswordGenerator
	{
        private static readonly Random _random = new Random();

        private const string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        private const string NumericChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()-_=+[]{};:,.<>?/\\|`~";
        
       

        public string GeneratePassword(PasswordRequirements requirements)
        {
          
            requirements= Valiation(requirements);
            var passwordChars = new List<char>();

            // Add required characters from each category
            passwordChars.AddRange(GetRandomChars(UpperChars, requirements.MinUpperAlphaChars));
            passwordChars.AddRange(GetRandomChars(LowerChars, requirements.MinLowerAlphaChars));
            passwordChars.AddRange(GetRandomChars(NumericChars, requirements.MinNumericChars));
            passwordChars.AddRange(GetRandomChars(SpecialChars, requirements.MinSpecialChars));

            int requiredLength = passwordChars.Count;

            if (requiredLength > requirements.MaxLength)
                throw new ArgumentException("Total minimum required characters exceed maximum password length.");

            // Fill remaining length with a mix of all characters
            int remainingLength = GetRandomNumber(
                Math.Max(0, requirements.MinLength - requiredLength),
                requirements.MaxLength - requiredLength);

            string allChars = UpperChars + LowerChars + NumericChars + SpecialChars;
            passwordChars.AddRange(GetRandomChars(allChars, remainingLength));

            // Shuffle the final password
            return Shuffle(passwordChars);
        }

        private static PasswordRequirements Valiation(PasswordRequirements requirements)
        {
            if (requirements.MinLength > requirements.MaxLength)
                throw new ArgumentException("MinLength cannot be greater than MaxLength");

            if (requirements.MinLength < 0 ||
                requirements.MaxLength < 0 ||
                requirements.MinUpperAlphaChars < 0 ||
                requirements.MinLowerAlphaChars < 0 ||
                requirements.MinSpecialChars < 0 ||
                requirements.MinNumericChars < 0
                )
                throw new ArgumentOutOfRangeException("Expected an exception when one the requirements is negative.");
            return requirements;
        }

        private static List<char> GetRandomChars(string source, int count)
        {
            return Enumerable.Range(0, count)
                             .Select(_ => source[_random.Next(source.Length)])
                             .ToList();
        }

        private static int GetRandomNumber(int min, int max)
        {
            if (max < min) return min; // fallback
            return _random.Next(min, max + 1); // inclusive of max
        }

        private static string Shuffle(List<char> chars)
        {
            return new string(chars.OrderBy(_ => _random.Next()).ToArray());
        }

    }
}
