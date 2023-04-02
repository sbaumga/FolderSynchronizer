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

        // TODO: test with FrontierJustice6000 - #7 Quality Royale - 16 (2A03+VRC6) PICTIONARY SONG!!! BY TIM FOLLIN.mp3
        // Cryptrik - GilvaSunner's Highest Quality Video Game - 273 the song's title is 'the swamp', guess what's about to happen.mp3
        // Poopy Joe - DJ Professor K Presents- 24-7 FUNKY FRESH BEATS FROM TOKYO-TO - 115 Everybody Jump Around (PRøF1Zø®️K∆YMIX).mp3
        // _rats - FIRST STRIKE ~ SiIvaGunner- King for Another Day To - 182 MyonMyon(myon)~Myon...Myon!Myon!.mp3
        // Wobble & Niko+ - PC Master Rips - 02 PC Master Rips Anthem.mp3
        // Jiko Music - SiIvaGunner's Highest Quality Rips- Volu - 347 I Forgive µ's (Inverse).mp3
        // Myeauxyoozi - The Inevitable Holiday Album - 183 BOYFRIEND (BF) FROM FRIDAY NIGHT FUNKIN' DOES THE DEFAULT DANCE (FROM FORTNITE!!!!).mp3
        // All-Star+Nuclear+Winter+Festival+Collection/Memesauce+-+Transmission+Archive+%7E+The+SiIvaGunner+A+-+341+Foghorn+of+Time.mp3
        // All-Star+Nuclear+Winter+Festival+Collection/DonnieTheGuy+-+Transmission+Archive+%7E+The+SiIvaGunner+A+-+281+Super+Paper+Mario+if+it+was+a+rhythm+game+for+mobile.mp3
        [TestCase("%237+Quality+Royale/ChickenSuitGuy+-+%237+Quality+Royale+-+304+Fabio%27s+Singing+Telegram.mp3", "#7 Quality Royale/ChickenSuitGuy - #7 Quality Royale - 304 Fabio's Singing Telegram.mp3")]
        [TestCase("All-Star+Nuclear+Winter+Festival+Collection/BrahaMan+-+Transmission+Archive+%7E+The+SiIvaGunner+A+-+368+THE+END+-+Pok%C3%A9mon+Heartgold+%26+Soulsilver.mp3", "All-Star Nuclear Winter Festival Collection/BrahaMan - Transmission Archive ~ The SiIvaGunner A - 368 THE END - Pokémon Heartgold & Soulsilver.mp3")]
        public void SuccessTest(string input, string expectedOutput)
        {
            var sanitizedKey = KeySanitizer.SanitizeKeyFromSQS(input);
            sanitizedKey.ShouldBe(expectedOutput);
        }
    }
}