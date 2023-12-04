namespace TypeForge.Core.Tests.Extensions;

using Xunit;
using TypeForge.Core.Extensions;
using TypeForge.Core.Configuration;
using System.IO;
using System;

public class DirectoryExtensionsTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public DirectoryExtensionsTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }

    [Fact]
    public void GetProjectDirectory_ShouldReturnCorrectDirectory()
    {
        // Arrange
        // No arrangement needed for this test

        // Act
        var result = DirectoryExtensions.GetProjectDirectory();

        // Assert
        // Assert based on your expected result
    }

    [Fact]
    public void GetRelativePath_ShouldReturnCorrectRelativePath_WhenFromPathAndToPathAreProvided()
    {
        // Arrange
        var fromPath = "C:\\Test\\FromPath";
        var toPath = "C:\\Test\\ToPath";

        // Act
        var result = DirectoryExtensions.GetRelativePath(fromPath, toPath);

        // Assert
        // Assert based on your expected result
    }

    // Continue with similar structure for the rest of the methods in the DirectoryExtensions class
}