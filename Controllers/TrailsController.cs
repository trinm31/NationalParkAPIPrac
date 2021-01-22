using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParkAPI.Models;
using NationalParkAPI.Models.Dtos;
using NationalParkAPI.Repository.IRepository;

namespace NationalParkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrailsController : Controller
    {

        private ITrailRepository _trailRepository;

        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepository, IMapper mapper)
        {
            _trailRepository = trailRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Get list of trail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetTrails()
        {
            var objList = _trailRepository.GetTrail();
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDto);
        }
        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="trailId">The id of trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepository.GetTrail(trailId);
            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<TrailDto>(obj);
            return Ok(objDto);
        }
        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_trailRepository.TrailExist(trailDto.Name))
            {
                ModelState.TryAddModelError("", "National Park Exists");
                return StatusCode(404, ModelState);
            }

            var ojbTrail = _mapper.Map<Trail>(trailDto);
            if (!_trailRepository.CreateTrail(ojbTrail))
            {
                ModelState.AddModelError("",$"Something went wrong went creating the recored {ojbTrail.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new {trailId = ojbTrail.Id}, ojbTrail);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId,[FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || trailId != trailDto.Id)
            {
                return BadRequest(ModelState);
            }
            var ojbTrail = _mapper.Map<Trail>(trailDto);
            if (!_trailRepository.EditTrail(ojbTrail))
            {
                ModelState.AddModelError("",$"Something went wrong went update the recored {ojbTrail.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExist(trailId))
            {
                return NotFound();
            }

            var ojbTrail = _trailRepository.GetTrail(trailId);
            if (!_trailRepository.DeleteTrail(ojbTrail))
            {
                ModelState.AddModelError("",$"Something went wrong went deleting the recored {ojbTrail.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}