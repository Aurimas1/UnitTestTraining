using System.Net;
using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;

namespace UnitTestTraining.Tests;

public class HttpTrainingTests
{
    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task GetNodesAsync_BadResponse_ThrowsException(HttpStatusCode statusCode)
    {
        // Arrange
        var url = "https://localhost/nodes";
        var expectedContent = "{}";
        var handler = new MockHttpMessageHandler();
        
        var content = new StringContent(expectedContent);
        handler.When(url).Respond(statusCode, content);
        
        HttpClient httpClient = handler.ToHttpClient();

        var sut = new HttpTraining(httpClient, null);
        
        // Act
        
        // Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetNodesAsync());
    }
    
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    public async Task GetNodesAsync_SuccessResponse_OK(HttpStatusCode statusCode)
    {
        // Arrange
        var url = "https://localhost/nodes";
        var expectedContent = "[]";
        var handler = new MockHttpMessageHandler();
        
        var content = new StringContent(expectedContent);
        handler.When(url).Respond(statusCode, content);
        
        HttpClient httpClient = handler.ToHttpClient();

        var sut = new HttpTraining(httpClient, null);
        
        // Act
        await sut.GetNodesAsync();
            
        // Assert
        // Do not throw exception
    }
    
    [Fact]
    public async Task GetNodesAsync_Unauthorized_CallsLogin()
    {
        // Arrange
        var url = "https://localhost/nodes";
        var expectedContent = "{}";
        var handler = new MockHttpMessageHandler();
        
        var content = new StringContent(expectedContent);
        handler.When(url).Respond(HttpStatusCode.Unauthorized, content);

        var loginProvider = new Mock<ILoginProvider>(MockBehavior.Strict);
        loginProvider.Setup(x => x.Login());
        
        HttpClient httpClient = handler.ToHttpClient();

        var sut = new HttpTraining(httpClient, loginProvider.Object);
        
        // Act
        await sut.GetNodesAsync();
            
        // Assert
        loginProvider.VerifyAll();
    }
    
    [Fact]
    public async Task GetNodesAsync_BodyWithObject_ResultWithOneElement()
    {
        // Arrange
        var url = "https://localhost/nodes";
        var expectedContent = "[{\"Id\":4}]";
        var handler = new MockHttpMessageHandler();
        
        var content = new StringContent(expectedContent);
        handler.When(url).Respond(HttpStatusCode.OK, content);

        var loginProvider = new Mock<ILoginProvider>(MockBehavior.Strict);
        loginProvider.Setup(x => x.Login());
        
        HttpClient httpClient = handler.ToHttpClient();

        var sut = new HttpTraining(httpClient, loginProvider.Object);
        
        // Act
        var result = await sut.GetNodesAsync();
            
        // Assert
        Assert.Single(result);
        result.Single().Should().BeEquivalentTo(new {Id = 4});
    }
}