using TypeForge.Core.Configuration.TypeForgeConfig;
using TypeForge.Core.Extensions;
using TypeForge.Core.Models;

namespace TypeForge.Core.Tests.Extensions;

public class TypeNameExtensionsTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public TypeNameExtensionsTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }

    [Fact]
    public void GetTypeName_ShouldReturnCorrectName_WhenConfigIsProvided()
    {
        // Arrange
        var typeName = "TestType";
        var config = new TypeForgeConfig
        {
            TypeNamePrefix = "Prefix",
            TypeNameSuffix = "Suffix",
            TypeNameCase = TypeNameCase.PascalCase
        };

        // Act
        var result = typeName.GetTypeName(config);

        // Assert
        Assert.Equal("PrefixTestTypeSuffix", result);
    }

    [Fact]
    public void AddPrefixAndSuffix_ShouldAddPrefixAndSuffix_WhenProvided()
    {
        // Arrange
        var typeName = "TestType";
        var prefix = "Prefix";
        var suffix = "Suffix";

        // Act
        var result = typeName.AddPrefixAndSuffix(prefix, suffix, PropertyNameCase.PascalCase);

        // Assert
        Assert.Equal("PrefixTestTypeSuffix", result);
    }

    [Fact]
    public void ToCaseOfOption_ShouldConvertToCorrectCase_WhenOptionIsProvided()
    {
        // Arrange
        var typeName = "testType";
        var option = TypeNameCase.PascalCase;

        // Act
        var result = typeName.ToCaseOfOption(option);

        // Assert
        Assert.Equal("TestType", result);
    }

    [Fact]
    public void GetExportStatement_ShouldReturnCorrectStatement_WhenFileIsProvided()
    {
        // Arrange
        var file = new TypeScriptFile { FileName = "TestFile.ts" };

        // Act
        var result = file.GetExportStatement();

        // Assert
        Assert.Equal("export * from './TestFile'", result);
    }

    [Fact]
    public void GetExportModelType_ShouldReturnCorrectType_WhenTypeScriptTypeAndConfigAreProvided()
    {
        // Arrange
        var typeScriptType = new TypeScriptType { Name = "TestType" };
        var config = new TypeForgeConfig
        {
            TypeModel = TypeModel.Type
        };

        // Act
        var result = typeScriptType.GetExportModelType(config);

        // Assert
        Assert.Equal("export type TestType = {", result);
    }

}
