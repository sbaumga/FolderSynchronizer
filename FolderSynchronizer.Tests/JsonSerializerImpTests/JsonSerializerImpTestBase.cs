using FolderSynchronizer.Implementations;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FolderSynchronizer.Tests.JsonSerializerImpTests
{
    [TestFixture]
    public abstract class JsonSerializerImpTestBase
    {
        protected JsonSerializerImp Serializer { get; set; }

        [SetUp]
        public void SetUp()
        {
            Serializer = new JsonSerializerImp();
        }

        protected string DefaultSerializedData => "{\"Text\":\"Test\",\"Number\":3,\"Date\":\"2022-01-04T00:00:00\",\"Object\":{\"Text\":\"Test 2\"},\"List\":[\"A\",\"B\",\"C\"]}";

        protected class SerializationTestClass
        {
            public string Text { get; set; }
            public int Number { get; set; }
            public DateTime Date { get; set; }
            public SerializationInnerTestClass Object { get; set; }
            public IList<string> List { get; set; }

            public SerializationTestClass()
            {
                Text = "Test";
                Number = 3;
                Date = new DateTime(2022, 1, 4);
                Object = new SerializationInnerTestClass();
                List = new List<string> { "A", "B", "C" };
            }
        }

        protected class SerializationInnerTestClass
        {
            public string Text { get; set; }

            public SerializationInnerTestClass()
            {
                Text = "Test 2";
            }
        }
    }
}