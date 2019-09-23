using InstaCoreGraphAPI.Business;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace InstaCoreGraphAPI.UnitTests
{
    public class InstaBusinessTests : IDisposable
    {
        IConfiguration SecretConfiguration { get; set; }

        private readonly MockRepository _mockRepository;

        private readonly Mock<IConfiguration> _mockConfiguration;

        private readonly string _accessToken;
        private readonly string _instagramId;

        public InstaBusinessTests()
        {
            // the type specified here is just so the secrets library can
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<InstaBusinessTests>();
            SecretConfiguration = builder.Build();

            _accessToken = SecretConfiguration["AppSettings:AccessToken"];
            _instagramId = SecretConfiguration["AppSettings:InstagramId"];

            this._mockRepository = new MockRepository(MockBehavior.Strict);

            this._mockConfiguration = this._mockRepository.Create<IConfiguration>();
            _mockConfiguration.Setup(c => c["AppSettings:AccessToken"]).Returns(() => _accessToken);
            _mockConfiguration.Setup(c => c["AppSettings:InstagramId"]).Returns(() => _instagramId );
            _mockConfiguration.Setup(c => c["AppSettings:fbGraphApiBaseUrl"]).Returns(() => "https://graph.facebook.com/v4.0");
        }

        public void Dispose()
        {
            this._mockRepository.VerifyAll();
        }

        private InstaBusiness CreateInstaBusiness()
        {
            return new InstaBusiness(
                this._mockConfiguration.Object);
        }

        [Fact]
        public void GetGraphApiUrlTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            string uri = $"https://graph.facebook.com/v4.0/{_instagramId}?access_token={_accessToken}";

            // Act
            var result = instaBusiness.GetGraphApiUrl(uri);

            // Assert
            Assert.True(_instagramId != null && result.Contains(_instagramId));
        }

        [Fact]
        public void GetMediasInsightTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            int limit = 0;
            string cursorBefore = null;
            string cursorAfter = null;

            // Act
            var result = instaBusiness.GetMediasInsight(
                limit,
                cursorBefore,
                cursorAfter);

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void GetBusinessDiscoveryTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            string instagramId = null;
            string accountName = null;

            // Act
            var result = instaBusiness.GetBusinessDiscovery(
                instagramId,
                accountName);

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void GetMediaInsighTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            string id = null;

            // Act
            var result = instaBusiness.GetMediaInsight(
                id);

            // Assert
            Assert.True(false);
        }

        [Fact]
        public void GetStoriesInsightTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();

            // Act
            var result = instaBusiness.GetStoriesInsight();

            // Assert
            Assert.True(false);
        }
    }
}
