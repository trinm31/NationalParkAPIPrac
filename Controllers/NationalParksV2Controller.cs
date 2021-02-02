using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParkAPI.Models;
using NationalParkAPI.Models.Dtos;
using NationalParkAPI.Repository.IRepository;

namespace NationalParkAPI.Controllers
{
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    [ApiController]
    // [ApiExplorerSettings(GroupName = "NationalParkAPINP")]
    public class NationalParksV2Controller : ControllerBase
    {

        private INationalParkRepository _nationalParkRepository;

        private readonly IMapper _mapper;

        public NationalParksV2Controller(INationalParkRepository nationalParkRepository, IMapper mapper)
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
            var obj = _nationalParkRepository.GetNationalPark().FirstOrDefault();
            return Ok(_mapper.Map<NationalParkDto>(obj));
        }
       
    }
}