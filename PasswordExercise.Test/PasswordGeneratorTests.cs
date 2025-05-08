using Microsoft.VisualStudio.TestTools.UnitTesting;
using PasswordExcercise;
using System;
using System.Linq;

namespace PasswordExercise.Test
{
    [TestClass]
    public class PasswordGeneratorTests
    {

        IPasswordGenerator generator;

        [TestInitialize]
        public void Initialize()
        {
            generator = new PasswordGenerator();
        }

        [TestMethod]
        public void TestGenerateLengthOnly()
        {
            string result = generator.GeneratePassword(new PasswordRequirements()
            {
                MaxLength = 16,
                MinLength = 8,
            });

            Assert.IsTrue(result.Length >= 8 && result.Length <= 16);
        }

        [TestMethod]
        public void TestGenerateAllRequirements()
        {
            string result = generator.GeneratePassword(new PasswordRequirements()
            {
                MaxLength = 16,
                MinLength = 8,
                MinLowerAlphaChars = 1,
                MinUpperAlphaChars = 1,
                MinNumericChars = 1,
                MinSpecialChars = 1
            });

            Assert.IsTrue(result.Length >= 8 && result.Length <= 16);
            Assert.IsTrue(result.Any(char.IsUpper));
            Assert.IsTrue(result.Any(char.IsLower));
            Assert.IsTrue(result.Any(char.IsNumber));
            Assert.IsTrue(result.Any(char.IsSymbol) || result.Any(char.IsPunctuation));
        }

        [TestMethod]
        public void TestGenerateAllRequirments_Multiple()
        {
            string result = generator.GeneratePassword(new PasswordRequirements()
            {
                MaxLength = 8,
                MinLength = 8,
                MinLowerAlphaChars = 2,
                MinUpperAlphaChars = 2,
                MinNumericChars = 2,
                MinSpecialChars = 2
            });

            Assert.IsTrue(result.Length == 8);
            Assert.IsTrue(result.Where(char.IsUpper).Count() == 2);
            Assert.IsTrue(result.Where(char.IsLower).Count() == 2);
            Assert.IsTrue(result.Where(char.IsNumber).Count() == 2);

            int countSpecial = result.Count(char.IsSymbol) + result.Count(char.IsPunctuation);
            Assert.IsTrue(countSpecial == 2);
        }

        [TestMethod]
        public void MinLengthGreaterThanMaxLength_ShouldThrowException()
        {

            // Act & Assert: generating a password should throw an exception due to invalid configuration
            Assert.ThrowsException<ArgumentException>(() =>
            {
                string password = generator.GeneratePassword(new PasswordRequirements()
                {
                    MaxLength = 10,
                    MinLength = 20,
                });
            }, "Expected an exception when MinLength is greater than MaxLength.");
        }

        [TestMethod]
        public void MinCharacterRequirementsExceedMaxLength_ShouldThrowException()
        {
            // (Here, total required = 2+2+2 = 6, which exceeds MaxLength of 5)

            // Act & Assert: generating a password should throw an exception due to unsatisfiable requirements
            Assert.ThrowsException<ArgumentException>(() =>
            {
                string password = generator.GeneratePassword(new PasswordRequirements()
                {
                    MinLength = 1,
                    MaxLength = 5,
                    MinUpperAlphaChars = 2,
                    MinLowerAlphaChars = 2,
                    MinNumericChars = 2,
                    MinSpecialChars = 0
                });
            }, "Expected an exception when MinLength is greater than MaxLength.");
        }

        [TestMethod]
        public void AllMinimumRequirementsZero_ShouldGenerateValidPassword()
        {
            string password = generator.GeneratePassword(new PasswordRequirements()
            {
                MinLength = 5,
                MaxLength = 10,
                MinUpperAlphaChars = 0,
                MinLowerAlphaChars = 0,
                MinNumericChars = 0,
                MinSpecialChars = 0
            });

            // Assert: password should not be null or empty, and its length should be within the specified range
            Assert.IsNotNull(password, "Password should not be null.");
            Assert.AreNotEqual(string.Empty, password, "Password should not be an empty string.");
            int length = password.Length;
            Assert.IsTrue(length >= 5 && length <= 10,
                $"Password length {length} was outside the expected range 5-10.");

        }

        [TestMethod]
        public void NegativeMinLength_ShouldThrowException()
        {
            // (MinLength is negative, which is invalid)

            // Act & Assert: attempt to generate should throw due to negative MinLength
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                string pwd = generator.GeneratePassword(new PasswordRequirements()
                {
                    MinLength = -1,
                    MaxLength = 10,
                    MinUpperAlphaChars = 1,
                    MinLowerAlphaChars = 1,
                    MinNumericChars = 1,
                    MinSpecialChars = 1
                });
            }, "Expected an exception when one the requirements is negative.");
        }


        [TestMethod]
        public void NegativeMinimumCharacterRequirement_ShouldThrowException()
        {
            // Arrange: set a negative requirement for one character type
            var requirments = new PasswordRequirements();
            requirments.MinLength = 5;
            requirments.MaxLength = 10;
            requirments.MinUpperAlphaChars = -2;   // Invalid negative requirement
            requirments.MinLowerAlphaChars = 1;
            requirments.MinNumericChars = 1;
            requirments.MinSpecialChars = 1;

            // Act & Assert: generating a password should throw due to negative character requirement
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                string pwd = new PasswordGenerator().GeneratePassword(requirments);
            }, "Expected an exception when one the requirements is negative.");
        }

        [TestMethod]
        public void ExtremelyHighMinLength_ShouldGenerateLargePasswordWithinLimits()
        {
            // Arrange: set an extremely high length requirement
            var requirments = new PasswordRequirements();
            requirments.MinLength = 10000;
            requirments.MaxLength = 10000;
            requirments.MinUpperAlphaChars = 0;
            requirments.MinLowerAlphaChars = 0;
            requirments.MinNumericChars = 0;
            requirments.MinSpecialChars = 0;
            // (All composition requirements are 0 to focus on length; any character is allowed)

            // Act: generate a very large password
            string password = new PasswordGenerator().GeneratePassword(requirments);

            // Assert: the generated password meets the length requirement (and generation completed)
            Assert.IsNotNull(password, "Password generation returned null for extremely high length.");
            Assert.AreEqual(10000, password.Length,
                $"Password length should be 10000 for the extreme length test, but was {password.Length}.");
            // (Optionally, we could measure execution time or ensure it's under a threshold if performance is a concern)
        }
    }
}



