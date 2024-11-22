using CarBidSystem.Auctions.CoreBusiness.DTOs;
using CarBidSystem.Auctions.UseCases.Auctions.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarBidSystem.Auctions.Service.Controllers
{
    [Route("api/[controller]")]
    public class AuctionsController(IMediator mediator) : Controller
    {
        private readonly IMediator mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(List<AuctionDto>), 204)]
        [ProducesResponseType(typeof(List<AuctionDto>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody] CreateAuctionCommand request)
        {
            await mediator.Send(request);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<AuctionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            return Ok(await mediator.Send(new GetAuctionsCommand()));
        }

        [HttpGet("{auctionId}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(AuctionDto), 200)]
        public async Task<IActionResult> Get(int auctionId)
        {
            var auction = await mediator.Send(new GetAuctionByIdCommand { AuctionId = auctionId });
            if(auction == null)
                return NotFound();

            return Ok(await mediator.Send(new GetAuctionByIdCommand { AuctionId = auctionId }));
        }
    }
}
