using FolderSynchronizer.TypeMapping;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace FolderSynchronizer.Tests.TypeMappingTests
{
    [TestFixture]
    public class ServiceRegistrationTests
    {
        [Test]
        public void SmokeTest()
        { 
            var serviceCollection = new ServiceCollection();

            Should.NotThrow(() => ServiceRegistation.Register(serviceCollection));
        }
    }
}