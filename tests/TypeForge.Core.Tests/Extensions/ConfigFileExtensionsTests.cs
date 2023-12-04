namespace TypeForge.Core.Tests.Extensions;

using Xunit;
using TypeForge.Core.Extensions;
using TypeForge.Core.Configuration;
using System.IO;
using System;


public class ConfigFileExtensionsTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public ConfigFileExtensionsTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }
}