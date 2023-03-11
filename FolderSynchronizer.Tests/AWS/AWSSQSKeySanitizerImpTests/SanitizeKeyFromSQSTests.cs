using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.AWS.AWSSQSKeySanitizerImpTests
{
    [TestFixture]
    public class SanitizeKeyFromSQSTests
    {
        private AWSSQSKeySanitizerImp KeySanitizer { get; set; }

        [SetUp]
        public void Setup()
        {
            KeySanitizer = new AWSSQSKeySanitizerImp();
        }

        [TestCase("%237+Quality+Royale/ChickenSuitGuy+-+%237+Quality+Royale+-+304+Fabio%27s+Singing+Telegram.mp3", "#7 Quality Royale/ChickenSuitGuy - #7 Quality Royale - 304 Fabio's Singing Telegram.mp3")]
        [TestCase("All-Star+Nuclear+Winter+Festival+Collection/BrahaMan+-+Transmission+Archive+%7E+The+SiIvaGunner+A+-+368+THE+END+-+Pok%C3%A9mon+Heartgold+%26+Soulsilver.mp3", "All-Star Nuclear Winter Festival Collection/BrahaMan - Transmission Archive ~ The SiIvaGunner A - 368 THE END - Pokémon Heartgold & Soulsilver.mp3")]
        public void SuccessTest(string input, string expectedOutput)
        {
            var sanitizedKey = KeySanitizer.SanitizeKeyFromSQS(input);
            sanitizedKey.ShouldBe(expectedOutput);
        }
    }
}