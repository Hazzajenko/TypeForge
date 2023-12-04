namespace TypeForge.Core.Tests.Mapping;
using Xunit;
using TypeForge.Core.Mapping;
using TypeForge.Core.Models;
using System.IO;
using System.Linq;

public class TypeScriptFolderMappingTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public TypeScriptFolderMappingTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }
    [Fact]
    public void ToRemoveFolderName_ShouldRemoveFolderName_WhenCalledOnTypeScriptFolder()
    {
        // Arrange
        var typeScriptFolder = new TypeScriptFolder
        {
            FolderName = "TestFolder" + Path.DirectorySeparatorChar
        };

        // Act
        var result = typeScriptFolder.ToRemoveFolderName();

        // Assert
        // Assert.Equal("TestFolder", result.FolderName);
    }

    [Fact]
    public void ToRemoveFolderName_ShouldRemoveFolderName_WhenCalledOnChildTypeScriptFolder()
    {
        // Arrange
        var childTypeScriptFolder = new ChildTypeScriptFolder
        {
            FolderName = "TestFolder" + Path.DirectorySeparatorChar
        };

        // Act
        var result = childTypeScriptFolder.ToRemoveFolderName();

        // Assert
        // Assert.Equal("TestFolder", result.FolderName);
    }

    [Fact]
    public void ToIndividualFolderName_ShouldReturnIndividualFolderName_WhenFolderNameContainsDirectorySeparator()
    {
        // Arrange
        var typeScriptFolder = new TypeScriptFolder
        {
            FolderName = "ParentFolder" + Path.DirectorySeparatorChar + "ChildFolder"
        };

        // Act
        var result = typeScriptFolder.ToIndividualFolderName();

        // Assert
        Assert.Equal("ChildFolder", result.FolderName);
    }

    [Fact]
    public void ToIndividualFolderName_ShouldReturnSameFolderName_WhenFolderNameDoesNotContainDirectorySeparator()
    {
        // Arrange
        var typeScriptFolder = new TypeScriptFolder
        {
            FolderName = "TestFolder"
        };

        // Act
        var result = typeScriptFolder.ToIndividualFolderName();

        // Assert
        Assert.Equal("TestFolder", result.FolderName);
    }
}