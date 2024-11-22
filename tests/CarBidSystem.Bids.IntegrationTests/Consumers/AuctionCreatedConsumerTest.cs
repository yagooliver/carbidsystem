using CarBidSystem.Bids.IntegrationTests.Configurations;
using CarBidSystem.Bids.UseCases.Consumers;
using CarBidSystem.Common.Models;
using Docker.DotNet.Models;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Bids.IntegrationTests.Consumers
{
    public class AuctionCreatedConsumerTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuctionCreatedConsumerTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AuctionCreatedConsumer_ShouldConsume_CreateAuction()
        {
            var serviceProvider = _factory.Services;
            var harness = serviceProvider.GetRequiredService<ITestHarness>();
            var consumer = harness.GetConsumerHarness<AuctionCreatedConsumer>();

            await harness.Start();

            var message = new AuctionCreatedCommandMessage
            {
                AuctionId = 1,
                StartedAt = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
            };

            await harness.Bus.Publish(message);

            Assert.True(await consumer.Consumed.Any<AuctionCreatedCommandMessage>());

            var auction = await _factory.GetAuctionByIdAsync(1);

            auction.Should().NotBeNull();
            auction.Id.Should().Be(1);
        }
    }
}
