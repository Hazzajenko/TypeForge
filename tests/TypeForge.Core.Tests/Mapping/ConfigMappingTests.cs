namespace TypeForge.Core.Tests.Mapping;

using Xunit;
using TypeForge.Core.Mapping;
using TypeForge.Core.Configuration;
using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Models;
using System.Collections.Generic;

public class ConfigMappingTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public ConfigMappingTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }

    [Fact]
    public void ToTypeForgeConfig_ShouldReturnCorrectConfig_WhenCliConfigFileAndProjectDirAreProvided()
    {
        // Arrange
        var cliConfigFile = new CliConfigFile
        {
            Directories = new[] {
                new ConfigDirectories
                {
                    Input = "Input",
                    Depth = 1,
                    IncludeChildren = true,
                    Flatten = true,
                    KeepRootFolder = true,
                    Output = "Output"
                }
            },
            FolderNameCase = "PascalCase",
            TypeNamePrefix = "Prefix",
            TypeNameSuffix = "Suffix",
            TypeModel = "Type",
            TypeNameCase = "PascalCase",
            PropertyNameCase = "PascalCase",
            FileNameCase = "PascalCase",
            NullableType = "Nullable",
            GenerateIndexFile = true
        };
        var projectDir = "C:\\Test";

        // Act
        var result = cliConfigFile.ToTypeForgeConfig(projectDir);

        // Assert
        // Assert based on your expected result
    }

    [Fact]
    public void ToConfigNameSpaceWithPath_ShouldReturnCorrectConfigDirectoryWithPath_WhenConfigDirectoriesIsProvided()
    {
        // Arrange
        var configDirectories = new ConfigDirectories
        {
            Input = "Input",
            Depth = 1,
            IncludeChildren = true,
            Flatten = true,
            KeepRootFolder = true,
            Output = "Output"
        };

        // Act
        var result = configDirectories.ToConfigNameSpaceWithPath();

        // Assert
        // Assert based on your expected result
    }
}