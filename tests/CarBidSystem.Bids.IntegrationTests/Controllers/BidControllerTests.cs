using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Bids.IntegrationTests.Configurations;
using CarBidSystem.Common.Response;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace CarBidSystem.Bids.IntegrationTests.Controllers
{
    public class BidControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public BidControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Creates an HttpClient to interact with the API
        }

        [Fact]
        public async Task PostBid_ShouldReturn204_WhenValidPayload()
        {

            // Arrange
            var payload = new
            {
                AuctionId = 1,
                Amount = 100,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PostBid_ShouldReturn400_WhenValidPayload_And_AuctionNotExist()
        {

            // Arrange
            var payload = new
            {
                AuctionId = 2,
                Amount = 100,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostBid_ShouldReturn400_WhenInValidPayload()
        {
            // Arrange
            var payload = new
            {
                AuctionId = 1,
                Amount = 0,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostBid_ShouldReturn400_WhenAuctionHasEnded()
        {
            await _factory.AddEndedAuction();
            // Arrange
            var payload = new
            {
                AuctionId = 2,
                Amount = 10000,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetBid_ShouldReturn200_WhenBidExists()
        {             
            // Arrange
            var payload = new
            {
                AuctionId = 1,
                Amount = 10000,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            // Act
            var bidResponse = await _client.GetAsync($"/api/bid/{1}");

            var bid = JsonConvert.DeserializeObject<BidDto>(await bidResponse.Content.ReadAsStringAsync());
            
            // Assert
            bidResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            bid.Should().NotBeNull();
            bid?.AuctionId.Should().Be(1);
        }

        [Fact]
        public async Task GetBid_ShouldReturn400_WhenBidNotExists()
        {
            // Act
            var bidResponse = await _client.GetAsync($"/api/bid/{2}");

            var bid = JsonConvert.DeserializeObject<BidDto>(await bidResponse.Content.ReadAsStringAsync());

            // Assert
            bidResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBidsByActionId_ShouldReturn200_WhenBidExists()
        {
            // Arrange
            var payload = new
            {
                AuctionId = 1,
                Amount = 10000,
                UserId = "test-user"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/bid", payload);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

            int pageNumber = 1;
            int pageSize = 5;
            var bidResponse = await _client.GetAsync($"/api/bid/1/bids?pageNumber={pageNumber}&pageSize={pageSize}");

            var paginatedResponse = JsonConvert.DeserializeObject<PagedResponse<List<BidDto>>>(
                await bidResponse.Content.ReadAsStringAsync());

            // Assert
            bidResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            paginatedResponse.Should().NotBeNull();
            paginatedResponse?.Data.Should().NotBeEmpty();
            paginatedResponse?.PageNumber.Should().Be(pageNumber);
            paginatedResponse?.PageSize.Should().Be(pageSize);
            paginatedResponse?.TotalRecords.Should().BeGreaterThan(0);
            paginatedResponse?.Data.Count.Should().BeLessOrEqualTo(pageSize);
        }
    }
}
