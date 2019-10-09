using InstaCoreGraphAPI.Business;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using InstaCoreGraphAPI.Graph.Entity;
using KellermanSoftware.CompareNetObjects;
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
        private readonly string _accountName;

        private readonly CompareLogic _logic;

        public InstaBusinessTests()
        {
            // the type specified here is just so the secrets library can
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<InstaBusinessTests>();
            SecretConfiguration = builder.Build();

            _accessToken = SecretConfiguration["AppSettings:AccessToken"];
            _instagramId = SecretConfiguration["AppSettings:InstagramId"];
            _accountName = SecretConfiguration["AppSettings:AccountName"];

            this._mockRepository = new MockRepository(MockBehavior.Strict);

            this._mockConfiguration = this._mockRepository.Create<IConfiguration>();
            _mockConfiguration.Setup(c => c["AppSettings:AccessToken"]).Returns(() => _accessToken);
            _mockConfiguration.Setup(c => c["AppSettings:InstagramId"]).Returns(() => _instagramId );
            _mockConfiguration.Setup(c => c["AppSettings:fbGraphApiBaseUrl"]).Returns(() => "https://graph.facebook.com/v4.0");

            ComparisonConfig config = new ComparisonConfig { MaxDifferences = int.MaxValue };
            _logic = new CompareLogic(config);
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
            int limit = 2;
            string cursorBefore = null;
            string cursorAfter = null;

            // Act
            var result = instaBusiness.GetMediasInsight(
                limit,
                cursorBefore,
                cursorAfter);

            // Assert
            Assert.True(result.Count ==  2);
        }

        [Fact]
        public void GetBusinessDiscoveryTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();

            // Act
            var result = instaBusiness.GetBusinessDiscovery(
                _instagramId, _accountName);

            // Assert
            Assert.True(result.media.followersCount > 0);
        }

        [Fact]
        public void GetBusinessDiscovery_LogicTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            BusinessDiscovery expected = new BusinessDiscovery();
            expected.media = new BusinessDiscoveryData();
            expected.media.followersCount = 1357;
            expected.media.mediaCount = 88;

            // Act
            var actual = instaBusiness.GetBusinessDiscovery(
                _instagramId, _accountName);

            // Assert
            ComparisonResult comparisonResult = _logic.Compare(actual, expected);
            Assert.True(comparisonResult.AreEqual, comparisonResult.DifferencesString);
        }

        [Fact]
        public void GetMediaInsighTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            string id = "18025098451232996";

            // Act
            var result = instaBusiness.GetMediaInsight(
                id);

            // Assert
            Assert.True(result.id == id);
        }

        [Fact]
        public void GetStoryInsighTest()
        {
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();
            string id = "18084620248110066";

            // Act
            var result = instaBusiness.GetStoryInsight(
                id);

            // Assert
            Assert.True(result.id == id);
        }

        [Fact]
        public void GetStoriesInsightTest()
        { 
            // Arrange
            var instaBusiness = this.CreateInstaBusiness();

            // Act
            var result = instaBusiness.GetStoriesInsight();

            // Assert
            Assert.True(result.Count > 0);
        }
    }
}
