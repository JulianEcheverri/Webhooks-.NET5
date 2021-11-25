using System;
using System.Linq;
using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookSubscriptionController : ControllerBase
    {
        private readonly AirlineDbContext _airlineDbContext;
        private readonly IMapper _mapper;

        public WebhookSubscriptionController(AirlineDbContext airlineDbContext, IMapper mapper)
        {
            _airlineDbContext = airlineDbContext;
            _mapper = mapper;
        }

        [HttpGet("{secret}", Name = "GetSubscriptionBySecret")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookSubscriptionReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionBySecret(string secret)
        {
            var webhookSubscription = _airlineDbContext.WebhookSubscriptions.FirstOrDefault(x => x.Secret == secret);
            if (webhookSubscription == null)
                return NotFound();

            return Ok(_mapper.Map<WebhookSubscriptionReadDto>(webhookSubscription));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookSubscriptionReadDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WebhookSubscriptionReadDto> CreateSubscription([FromBody] WebhookSubscriptionCreateDto webhookSubscriptionCreateDto)
        {
            var webhookSubscriptionExistent = _airlineDbContext.WebhookSubscriptions.FirstOrDefault(x => x.WebhookURI == webhookSubscriptionCreateDto.WebhookURI);
            if (webhookSubscriptionExistent != null)
                return NoContent();

            var webhookSubscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);
            webhookSubscription.Secret = Guid.NewGuid().ToString();
            webhookSubscription.WebhookPublisher = "PanAus";

            try
            {
                _airlineDbContext.WebhookSubscriptions.Add(webhookSubscription);
                _airlineDbContext.SaveChanges();
                var webhookSubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(webhookSubscription);

                return CreatedAtAction(nameof(CreateSubscription), new { secret = webhookSubscriptionReadDto.Secret, webhookSubscriptionReadDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong attempting to save the webhook subscription. Exception: {0}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}