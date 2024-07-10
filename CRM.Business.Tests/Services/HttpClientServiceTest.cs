using CRM.Business.Configuration.HttpClients;
using CRM.Business.Services;
using FluentAssertions;
using RichardSzalay.MockHttp;

namespace CRM.Business.Tests.Services;

public class HttpClientServiceTest
{
    private readonly ConfigurationManagerHttpClient _configurationManagerHttpClient;
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;

    public HttpClientServiceTest()
    {
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        var httpClient = _mockHttpMessageHandler.ToHttpClient();
        _configurationManagerHttpClient = new ConfigurationManagerHttpClient(httpClient);
    }

    
    [Fact]
    public async Task GetAsync_ValidStringUriSent_DictionaryConfigurationSettingsReceived()
    {
        //arrange
        const string content = "{\"CrmDb_ConfigurationManager\":\"TestDb\",\"Log_ConfigurationManager\":\"Log\",\"RabbitMqPassword_ConfigurationManager\":\"Test\",\"CrmHost_ConfigurationManager\":\"CrmHost\",\"TransactionStoreHost_ConfigurationManager\":\"TStoreHost\",\"RabbitMqHost_ConfigurationManager\":\"rabbiHost\",\"RabbitMqLogin_ConfigurationManager\":\"Test\"}";
        _mockHttpMessageHandler.When("https://194.87.210.5:13000/api/configuration?service=1")
            .Respond("application/json", content);
        var expected = new Dictionary<string, string>
        {
            { "CrmDb_ConfigurationManager", "TestDb" },
            { "Log_ConfigurationManager", "Log" },
            { "RabbitMqPassword_ConfigurationManager", "Test" },
            { "CrmHost_ConfigurationManager", "CrmHost" },
            { "TransactionStoreHost_ConfigurationManager", "TStoreHost" },
            { "RabbitMqHost_ConfigurationManager", "rabbiHost" },
            { "RabbitMqLogin_ConfigurationManager", "Test" },
        };
        var sut = new HttpClientService<ConfigurationManagerHttpClient>(_configurationManagerHttpClient, new CancellationTokenSource(6000));

        // Act
        var actual = await sut.GetAsync<Dictionary<string, string>>("https://194.87.210.5:13000/api/configuration?service=1");

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetAsync_InvalidStringUriSent_MockHttpMatchErrorReceived()
    {
        //arrange
        const string content = "{\"CrmDb_ConfigurationManager\":\"TestDb\",\"Log_ConfigurationManager\":\"Log\",\"RabbitMqPassword_ConfigurationManager\":\"Test\",\"CrmHost_ConfigurationManager\":\"CrmHost\",\"TransactionStoreHost_ConfigurationManager\":\"TStoreHost\",\"RabbitMqHost_ConfigurationManager\":\"rabbiHost\",\"RabbitMqLogin_ConfigurationManager\":\"Test\"}";
        _mockHttpMessageHandler.When("https://194.87.210.5:13000/api/configuration?service=1")
            .Respond("application/json", content);
        var sut = new HttpClientService<ConfigurationManagerHttpClient>(_configurationManagerHttpClient, new CancellationTokenSource(6000));

        //act
        var act = async () => await sut.GetAsync<Dictionary<string, string>>("https://194.87.210.5:13000/api/configuration?service=2");

        //assert
        await act.Should().ThrowAsync<MockHttpMatchException>();
    }
}
