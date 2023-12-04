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

    // [Fact]
    // public void GetConfigFile_ShouldReturnCorrectConfigFile_WhenConfigFileTypeAndPathAreProvided()
    // {
    //     // Arrange
    //     var projectDir = Directory.GetCurrentDirectory();
    //     var configFileType = ConfigFileType.Json;
    //     var configFilePath = Path.Combine(projectDir, "test.json");
    //
    //     // Act
    //     var result = projectDir.GetConfigFile(configFileType, configFilePath);
    //
    //     // Assert
    //     // Assert based on your expected result
    // }
    //
    // [Fact]
    // public void GetConfigFileTypeIfExist_ShouldReturnCorrectConfigFileType_WhenProjectDirAndConfigNameAreProvided()
    // {
    //     // Arrange
    //     var projectDir = Directory.GetCurrentDirectory();
    //     var configName = "test";
    //
    //     // Act
    //     var result = projectDir.GetConfigFileTypeIfExist(configName);
    //
    //     // Assert
    //     // Assert based on your expected result
    // }
    //
    // [Fact]
    // public void GetConfigFileYml_ShouldReturnCorrectConfigFile_WhenProjectDirAndConfigNameAreProvided()
    // {
    //     // Arrange
    //     var projectDir = Directory.GetCurrentDirectory();
    //     var configName = "test.yml";
    //
    //     // Act
    //     var result = projectDir.GetConfigFileYml(configName);
    //
    //     // Assert
    //     // Assert based on your expected result
    // }

    // [Fact]
    // public void GetConfigFileJson_ShouldReturnCorrectConfigFile_WhenProjectDirAndConfigNameAreProvided()
    // {
    //     // Arrange
    //     var projectDir = Directory.GetCurrentDirectory();
    //     var configName = "test.json";
    //
    //     // Act
    //     var result = projectDir.GetConfigFileJson(configName);
    //
    //     // Assert
    //     // Assert based on your expected result
    // }
}