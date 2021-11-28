﻿using FolderSynchronizer.AWS.Implementations;
using NUnit.Framework;

namespace FolderSynchronizer.Tests.AWS.AWSPathManagerImpTests
{
    [TestFixture]
    public abstract class AWSPathManagerImpTestBase
    {
        protected const string FolderName = "Test";

        protected AWSPathManagerImp PathManager { get; set; }

        [SetUp]
        public void SetUp()
        {
            var configData = new ConfigData { LocalFolderName = FolderName };

            PathManager = new AWSPathManagerImp(configData);
        }
    }
}