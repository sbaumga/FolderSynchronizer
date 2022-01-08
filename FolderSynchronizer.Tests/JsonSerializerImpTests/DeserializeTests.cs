using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.JsonSerializerImpTests
{
    public class DeserializeTests : JsonSerializerImpTestBase
    {
        [Test]
        public void SuccessTest()
        {
            var str = DefaultSerializedData;

            var result = Serializer.Deserialize<SerializationTestClass>(str);

            result.ShouldBeEquivalentTo(new SerializationTestClass());
        }
    }
}