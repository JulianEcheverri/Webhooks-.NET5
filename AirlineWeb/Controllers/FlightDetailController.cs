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
    public class FlightDetailController : ControllerBase
    {
        private readonly AirlineDbContext _airlineDbContext;
        private readonly IMapper _mapper;

        public FlightDetailController(AirlineDbContext airlineDbContext, IMapper mapper)
        {
            _airlineDbContext = airlineDbContext;
            _mapper = mapper;
        }

        [HttpGet("{flightCode}", Name = "GetFlightDetailByCode")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlightDetailReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FlightDetailReadDto> GetFlightDetailByCode(string flightCode)
        {
            var flightDetail = _airlineDbContext.FlightDetails.FirstOrDefault(x => x.FlightCode == flightCode);
            if (flightDetail == null)
                return NotFound();

            return Ok(_mapper.Map<FlightDetailReadDto>(flightDetail));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlightDetailCreateDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FlightDetailReadDto> CreateFlightDetail([FromBody] FlightDetailCreateDto flightDetailCreateDto)
        {
            var flightDetailExistent = _airlineDbContext.FlightDetails.FirstOrDefault(x => x.FlightCode == flightDetailCreateDto.FlightCode);
            if (flightDetailExistent != null)
                return NotFound();

            var flightDetail = _mapper.Map<FlightDetail>(flightDetailCreateDto);
            try
            {
                _airlineDbContext.FlightDetails.Add(flightDetail);
                _airlineDbContext.SaveChanges();

                var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetail);

                return CreatedAtAction(nameof(CreateFlightDetail), new { flightCode = flightDetailReadDto.FlightCode, flightDetailReadDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong attempting to save the Flight Detail. Exception: {0}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FlightDetailReadDto> UpdateFlightDetail(int id, [FromBody] FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flightDetail = _airlineDbContext.FlightDetails.FirstOrDefault(x => x.Id == id);
            if (flightDetail == null)
                return NotFound();

            try
            {
                _mapper.Map(flightDetailUpdateDto, flightDetail);

                _airlineDbContext.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong attempting to update the Flight Detail. Exception: {0}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}