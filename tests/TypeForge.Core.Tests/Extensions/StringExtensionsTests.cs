using TypeForge.Core.Extensions;

namespace TypeForge.Core.Tests.Extensions;

public class StringExtensionsTests : IClassFixture<TestFactory>
{
    private readonly TestFactory _testFactory;

    public StringExtensionsTests(TestFactory testFactory)
    {
        _testFactory = testFactory;
    }
    
    [Fact]
    public void ToCamelCase_ShouldConvertToCamelCase()
    {
        // Arrange
        var str = "Test_String";

        // Act
        var result = str.ToCamelCase();

        // Assert
        Assert.Equal("testString", result);
    }

    [Fact]
    public void ToKebabCase_ShouldConvertToKebabCase()
    {
        // Arrange
        var str = "TestString";

        // Act
        var result = str.ToKebabCase();

        // Assert
        Assert.Equal("test-string", result);
    }

    [Fact]
    public void ToPascalCase_ShouldConvertToPascalCase()
    {
        // Arrange
        var str = "test_string";

        // Act
        var result = str.ToPascalCase();

        // Assert
        Assert.Equal("TestString", result);
    }

    [Fact]
    public void ToPrettyJson_ShouldConvertToPrettyJson()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 123 };

        // Act
        var result = obj.ToPrettyJson();

        // Assert
        var expected = "{\n  \"name\": \"Test\",\n  \"value\": 123\n}";
        result = result.Replace("\r\n", "\n");
        Assert.Equal(expected, result);
    }
}
