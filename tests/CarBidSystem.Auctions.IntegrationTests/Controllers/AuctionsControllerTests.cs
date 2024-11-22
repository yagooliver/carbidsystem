using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Auctions.CoreBusiness.Entities;
using CarBidSystem.Auctions.IntegrationTests.Factory;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Auctions.IntegrationTests.Controllers
{
    public class AuctionControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AuctionControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Creates an HttpClient to interact with the API
        }

        [Fact]
        public async Task PostAuction_ShouldReturn204_WhenValidPayload()
        {
            // Arrange
            var payload = new
            {
                CarId = 1,
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auctions", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PostAuction_ShouldReturn400_WhenInvalidPayload()
        {
            // Arrange
            var payload = new
            {
                CarId = 0, // Invalid CarId
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auctions", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostAuction_ShouldReturn400_WhenEndTimeBeforeStartTime()
        {
            // Arrange
            var payload = new
            {
                CarId = 0,
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(1) // Invalid EndTime
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auctions", payload);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAuctions_ShouldReturn200_WhenAuctionsExist()
        {
            // Arrange
            var auction = new Auction(3, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
            await _factory.AddAuctionAsync(auction);

            //ACT
            var response = await _client.GetAsync("/api/auctions");
            var responseStr = await response.Content.ReadAsStringAsync();
            var auctions = JsonConvert.DeserializeObject<List<AuctionDto>>(responseStr);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            auctions.Should().NotBeNull();
            auctions.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAuctionById_ShouldReturn200_WhenAuctionExists()
        {
            // Arrange
            var auction = new Auction(4,DateTime.UtcNow.AddHours(1),DateTime.UtcNow.AddHours(2));
            await _factory.AddAuctionAsync(auction);

            // Act
            var response = await _client.GetAsync($"/api/auctions/{auction.Id}");

            var auctionDto = JsonConvert.DeserializeObject<AuctionDto>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            auctionDto.Should().NotBeNull();
            auctionDto.CarId.Should().Be(auction.CarId);
        }

        [Fact]
        public async Task GetAuctionById_ShouldReturn404_WhenAuctionDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/auctions/999");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
