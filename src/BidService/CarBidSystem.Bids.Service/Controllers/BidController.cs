using CarBidSystem.Bids.CoreBusiness.DTOs;
using CarBidSystem.Bids.UseCases.Bids.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CarBidSystem.Bids.Service.Controllers
{
    [Route("api/[controller]")]
    public class BidController(IMediator mediator) : Controller
    {
        private readonly IMediator mediator = mediator;

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody]PlaceBidCommand placeBidCommand)
        {
            await mediator.Send(placeBidCommand);

            return Created();
        }

        [HttpGet("{auctionId}/bids")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBidsByAction(int auctionId)
        {
            var bids = await mediator.Send(new GetBidsByActionIdCommand(auctionId));
            return Ok(bids);
        }

        [HttpGet("{bidId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int bidId)
        {
            var bid = await mediator.Send(new GetBidByIdCommand(bidId));
            if (bid == null)
                return NotFound();

            return Ok(bid);
        }
    }
}
