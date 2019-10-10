using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace InstaCoreGraphAPI.IntegrationTests
{
    public class APIIntegrationTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;

        public APIIntegrationTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task TestGetMediasInsightAsync()
        {
            // Arrange
            var request = new
            {
                Url = "/api/v1/Insta/GetMediasInsight",
                Body = new
                {
                    limit = 1
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, ContentHelper.GetStringContent(request.Body));

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetMediaInsightAsync()
        {
            // Arrange
            string baseUri = $"/api/v1/Insta/GetMediaInsight?mediaId=18025098451232996";

            // Act
            var response = await Client.GetAsync(baseUri);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetStoryInsightAsync()
        {
            // Arrange
            string baseUri = $"/api/v1/Insta/GetStoryInsight?mediaId=18084620248110066";

            // Act
            var response = await Client.GetAsync(baseUri);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetStoriesInsightAsync()
        {
            // Arrange
            string baseUri = $"/api/v1/Insta/GetStoriesInsight";

            // Act
            var response = await Client.GetAsync(baseUri);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestGetBusinessDiscoveryAsync()
        {
            // Arrange
            string baseUri = $"/api/v1/Insta/GetBusinessDiscovery?accountName=kidecoration";

            // Act
            var response = await Client.GetAsync(baseUri);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
