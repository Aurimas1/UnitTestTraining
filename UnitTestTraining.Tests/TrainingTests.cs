using System.Net;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace UnitTestTraining.Tests;

public class TrainingTests
{
    [Fact]
    public void GenerateName_None_CorrectName()
    {
        // Arrange
        var sut = new Training();
        
        // Act
        var result = sut.GenerateName();

        // Assert
        Assert.Equal("name", result);
        result.Should().Be("name");
    }
    
    [Fact]
    public void EquivalentSample()
    {
        // Arrange
        var expected = new { a = 4 };
        var actual = new { a = 4, b = 5 };
        
        // Act
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task HttpSample()
    {
        // Arrange
        var url = "http://localhost:5000/test";
        var expectedContent = "sample";
        var handler = new MockHttpMessageHandler();
        
        var content = new StringContent(expectedContent);
        handler.When(url).Respond(HttpStatusCode.OK, content);

        var sut = handler.ToHttpClient();
        
        // Act
        var response = await sut.GetAsync(url);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedContent, await response.Content.ReadAsStringAsync());
    }
}