using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly TravelAgentDbContext _travelAgentDbContext;

        public NotificationController(TravelAgentDbContext travelAgentDbContext)
        {
            _travelAgentDbContext = travelAgentDbContext;
        }

        [HttpPost]
        public ActionResult FlightChanged(FlightDetailUpdateDto flightDetailUpdateDto)
        {
            Console.WriteLine($"Webhook Received from: {flightDetailUpdateDto.Publisher}");

            var secretModel = _travelAgentDbContext.SubscriptionSecrets.FirstOrDefault(x => x.Publisher == flightDetailUpdateDto.Publisher && x.Secret == flightDetailUpdateDto.Secret);

            if (secretModel == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Secret - Ignore Webhook");
                Console.ResetColor();
                return BadRequest($"Invalid Secret - Ignore Webhook {flightDetailUpdateDto.Secret}");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid Webhook!");

            Console.WriteLine($"Old Price {flightDetailUpdateDto.OldPrice}");
            Console.WriteLine($"New Price {flightDetailUpdateDto.NewPrice}");
            Console.ResetColor();

            return Ok();
        }
    }
}