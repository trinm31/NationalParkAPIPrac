using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParkAPI.Models;
using NationalParkAPI.Models.Dtos;
using NationalParkAPI.Repository.IRepository;

namespace NationalParkAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    //[Route("api/[Controller]")]
    [ApiController]
    // [ApiExplorerSettings(GroupName = "NationalParkAPINP")]
    public class NationalParksController : ControllerBase
    {

        private INationalParkRepository _nationalParkRepository;

        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            _nationalParkRepository = nationalParkRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Get list of national park
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetNationalParks()
        {
            var objList = _nationalParkRepository.GetNationalPark();
            var objDto = new List<NationalParkDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            return Ok(objDto);
        }
        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="nationalParkId">The id of national park</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _nationalParkRepository.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_nationalParkRepository.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.TryAddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }

            var ojbNationalPark = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_nationalParkRepository.CreateNationalPark(ojbNationalPark))
            {
                ModelState.AddModelError("",$"Something went wrong went creating the recored {ojbNationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new {version = HttpContext.GetRequestedApiVersion().ToString(),nationalParkId = ojbNationalPark.Id}, ojbNationalPark);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(204, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public IActionResult UpdateNationalPark(int nationalParkId,[FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }
            var ojbNationalPark = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_nationalParkRepository.EditNationalPark(ojbNationalPark))
            {
                ModelState.AddModelError("",$"Something went wrong went update the recored {ojbNationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_nationalParkRepository.NationalParkExist(nationalParkId))
            {
                return NotFound();
            }

            var ojbNationalPark = _nationalParkRepository.GetNationalPark(nationalParkId);
            if (!_nationalParkRepository.DeleteNationalPark(ojbNationalPark))
            {
                ModelState.AddModelError("",$"Something went wrong went deleting the recored {ojbNationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}