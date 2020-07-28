using Xunit;
using Core.Models;
using AI_lib;
using System.Collections.Generic;
using System.IO;

namespace AI_lib_test
{
    public class UtilityTest
    {
        public UtilityTest()
        {
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(1, 100)]
        [InlineData(1, 99)]
        [InlineData(1, 2)]
        [InlineData(1, 1)]
        public void Should_GetRandomInt_BetweenMinAndMax(int min, int max)
        {
            var randomizer = new CryptoRandomizer();
            var randomInt = randomizer.Next(min, max);
            Assert.True(randomInt >= min);
            Assert.True(randomInt <= max);
        }

        [Fact(Skip = "Should only run in debug mode since write to file")]
        public void Ensure_ThatCryptoRandomizer_IsMaxInclusive()
        {
            File.Delete("randomtest.txt");

            var randomizer = new CryptoRandomizer();
            for (int i = 0; i < 20; i++)
            {
                var randomInt = randomizer.Next(0, 1);
                File.AppendAllText("randomtest.txt", $"\n{randomInt}");
            }
        }

        [Fact(Skip = "Should only run in debug mode since write to file")]
        public void Ensure_ThatShuffle_Works()
        {
            var list = new List<string>() { "1", "2", "3", "4", "5" };
            for (var i = 0; i < 10; i++)
            {
                list.Shuffle();
                File.AppendAllText("shuffletest.txt", $"\n{string.Join(' ', list)}");
            }
        }
    }
}
