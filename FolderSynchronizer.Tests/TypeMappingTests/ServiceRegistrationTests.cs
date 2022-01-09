using FolderSynchronizer.TypeMapping;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FolderSynchronizer.Tests.TypeMappingTests
{
    [TestFixture]
    public class ServiceRegistrationTests
    {
        [Test]
        public void RegistrationTest()
        {
            var serviceCollection = new ServiceCollection();

            ServiceRegistation.Register(serviceCollection);

            VerifyAllImplemenationsAreRegistered(serviceCollection);
        }

        private void VerifyAllImplemenationsAreRegistered(ServiceCollection serviceCollection)
        {
            var implementationTypes = GetImplementationTypes();

            foreach(var impType in implementationTypes)
            {
                VerifyImplementationTypeIsRegistered(serviceCollection, impType);
            }
        }

        private IEnumerable<Type> GetImplementationTypes()
        {
            var assembly = Assembly.GetAssembly(typeof(ServiceRegistation));
            var types = assembly.GetTypes();
            var implementationTypes = types.Where(t => t.Namespace != null && t.Namespace.Contains("Implementations") && !t.IsAbstract && t.IsPublic && t.IsClass);

            return implementationTypes;
        }

        private void VerifyImplementationTypeIsRegistered(ServiceCollection serviceCollection, Type type)
        {
            serviceCollection.ShouldContain(d => d.ImplementationType == type);
        }
    }
}