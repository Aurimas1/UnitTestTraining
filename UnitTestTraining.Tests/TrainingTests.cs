using System.Net;
using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;

namespace UnitTestTraining.Tests;

public class TrainingTests
{
    [Fact]
    public void GenerateName_None_CorrectName()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>(MockBehavior.Strict);

        systemClock.Setup(x => x.Now()).Returns(new DateTime(2000, 1, 1));
        
        var sut = new Training(systemClock.Object);
        
        // Act
        var result = sut.GenerateName();

        // Assert
        Assert.Equal("name20000101", result);
    }
    
    [Fact]
    public void GenerateName_WithYear1999_ThrowException()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>(MockBehavior.Strict);

        systemClock.Setup(x => x.Now()).Returns(new DateTime(1999, 1, 1));
        
        var sut = new Training(systemClock.Object);
        
        // Act

        // Assert
        Assert.Throws<Exception>(() => sut.GenerateName());
    }
    
    [Fact]
    public void GetOrAdd_CallTwoTimesWithId1_SameInstance()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>();
        var sut = new Training(systemClock.Object);
        
        // Act
        var result1 = sut.GetOrAdd(1);
        var result2 = sut.GetOrAdd(1);

        // Assert
        Assert.Equal(result1, result2);
    }
    
    [Fact]
    public void GetOrAdd_CallTwoTimesWithId1In4MinutesTime_SameInstance()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>(MockBehavior.Strict);

        systemClock.SetupSequence(x => x.Now())
            .Returns(new DateTime(2000, 1, 1, 1, 1, 1))
            .Returns(new DateTime(2000, 1, 1, 1, 5, 1));
        
        var sut = new Training(systemClock.Object);
        
        // Act
        var result1 = sut.GetOrAdd(1);
        var result2 = sut.GetOrAdd(1);

        // Assert
        Assert.Equal(result1, result2);
    }
    
    [Fact]
    public void GetOrAdd_CallTwoTimesWithId1In6MinutesTime_DifferentInstance()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>(MockBehavior.Strict);

        systemClock.SetupSequence(x => x.Now())
            .Returns(new DateTime(2000, 1, 1, 1, 1, 1))
            .Returns(new DateTime(2000, 1, 1, 1, 7, 1));
        
        var sut = new Training(systemClock.Object);
        
        // Act
        var result1 = sut.GetOrAdd(1);
        var result2 = sut.GetOrAdd(1);

        // Assert
        Assert.NotEqual(result1, result2);
    }
    
    [Fact]
    public void GetOrAdd_CallTwoTimesWithId1and2_DifferentInstance()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>();
        var sut = new Training(systemClock.Object);
        
        // Act
        var result1 = sut.GetOrAdd(1);
        var result2 = sut.GetOrAdd(2);

        // Assert
        Assert.NotEqual(result1, result2);
    }
    
    [Fact]
    public void GetOrAdd_CallTwoTimesWithId1aRemove1Add1_DifferentInstance()
    {
        // Arrange
        var systemClock = new Mock<ISystemClock>();
        var sut = new Training(systemClock.Object);
        
        // Act
        var result1 = sut.GetOrAdd(1);
        sut.Remove(1);
        var result2 = sut.GetOrAdd(1);

        // Assert
        Assert.NotEqual(result1, result2);
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

        HttpClient sut = handler.ToHttpClient();
        
        // Act
        var response = await sut.GetAsync(url);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedContent, await response.Content.ReadAsStringAsync());
    }
}