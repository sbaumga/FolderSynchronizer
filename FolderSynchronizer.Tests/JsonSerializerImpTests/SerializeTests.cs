using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.JsonSerializerImpTests
{
    public class SerializeTests : JsonSerializerImpTestBase
    {
        [Test]
        public void SuccessTest()
        {
            var data = new SerializationTestClass();

            var result = Serializer.Serialize(data);

            result.ShouldBe(DefaultSerializedData);
        }
    }
}