/*
using System.Text.Json;
using CRM.Business.Services;
using CRM.Core.Exceptions;
using RichardSzalay.MockHttp;

namespace CRM.Business.Tests.Services;

public class HttpClientServiceTest
{ 
        private readonly MockHttpMessageHandler _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        
        public HttpClientServiceTest()
        {
            _mockHttpMessageHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttpMessageHandler);
        }

        [Fact]
        public async Task GetAsync_SuccessfulResponse_ReturnsDeserializedObject()
        {
            //arrange
            var expectedResponse = new { Id = 1, Name = "John Doe" };
            _mockHttpMessageHandler.When("https://example.com/api/users/1")
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));
            var sut = new HttpClientService<>(_accountsRepositoryMock.Object, _leadsRepositoryMock.Object, _messagesService, _mapper);

            // Act
            var result = await _httpClientWrapper.GetAsync<dynamic>("https://example.com/api/users/1");

            // Assert
            Assert.AreEqual(expectedResponse.Id, result.Id);
            Assert.AreEqual(expectedResponse.Name, result.Name);
        }

        [Test]
        public async Task GetAsync_OperationCanceledException_ThrowsGatewayTimeoutException()
        {
            // Arrange
            _mockHttpMessageHandler.When("https://example.com/api/users/1")
                .Throw(new OperationCanceledException("The operation was canceled."));

            // Act and Assert
            var ex = Assert.ThrowsAsync<GatewayTimeoutException>(async () =>
                await _httpClientWrapper.GetAsync<dynamic>("https://example.com/api/users/1"));
            Assert.AreEqual("The operation was canceled.", ex.Message);
        }

        [Test]
        public async Task GetAsync_HttpRequestException_ThrowsBadGatewayException()
        {
            // Arrange
            _mockHttpMessageHandler.When("https://example.com/api/users/1")
                .Throw(new HttpRequestException("The request failed."));

            // Act and Assert
            var ex = Assert.ThrowsAsync<BadGatewayException>(async () =>
                await _httpClientWrapper.GetAsync<dynamic>("https://example.com/api/users/1"));
            Assert.AreEqual("The request failed.", ex.Message);
        }
        
        
    }
}

{
    public class HttpClientWrapperTests
    {
        
    }
}
*/
