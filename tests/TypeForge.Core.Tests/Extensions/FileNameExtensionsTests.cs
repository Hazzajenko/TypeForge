using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;

namespace TypeForge.Core.Tests.Extensions;

public class FileNameExtensionsTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public FileNameExtensionsTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }

    [Fact]
    public void GetFileName_ShouldReturnCorrectFileName_WhenConfigIsProvided()
    {
        // Arrange
        var fileName = "TestFile.cs";
        var config = new TypeForgeConfig
        {
            FileNameCase = FileNameCase.PascalCase,
            TypeNamePrefix = "Prefix",
            TypeNameSuffix = "Suffix"
        };

        // Act
        var result = fileName.GetFileName(config);

        // Assert
        Assert.Equal("PrefixTestFileSuffix.ts", result);
    }

    [Fact]
    public void RemoveCsExtension_ShouldRemoveCsExtension_WhenCalled()
    {
        // Arrange
        var fileName = "TestFile.cs";

        // Act
        var result = fileName.RemoveCsExtension();

        // Assert
        Assert.Equal("TestFile", result);
    }

    [Fact]
    public void AddPrefixAndSuffix_ShouldAddPrefixAndSuffix_WhenCalled()
    {
        // Arrange
        var fileName = "TestFile";
        var prefix = "Prefix";
        var suffix = "Suffix";

        // Act
        var result = fileName.AddPrefixAndSuffix(FileNameCase.PascalCase, prefix, suffix);

        // Assert
        Assert.Equal("PrefixTestFileSuffix", result);
    }

    [Fact]
    public void ToCaseOfOption_ShouldConvertToCorrectCase_WhenOptionIsProvided()
    {
        // Arrange
        var fileName = "test_file";
        var option = FileNameCase.PascalCase;

        // Act
        var result = fileName.ToCaseOfOption(option);

        // Assert
        Assert.Equal("TestFile", result);
    }

    [Fact]
    public void AddTsExtension_ShouldAddTsExtension_WhenCalled()
    {
        // Arrange
        var fileName = "TestFile";

        // Act
        var result = fileName.AddTsExtension();

        // Assert
        Assert.Equal("TestFile.ts", result);
    }

    [Fact]
    public void GetFileNameFromPath_ShouldReturnFileName_WhenPathIsProvided()
    {
        // Arrange
        var path = "C:\\Test\\TestFile.cs";

        // Act
        var result = path.GetFileNameFromPath();

        // Assert
        Assert.Equal("TestFile.cs", result);
    }

}