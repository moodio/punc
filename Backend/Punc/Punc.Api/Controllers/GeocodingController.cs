using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Punc.Api.Controllers
{
    [ApiController()]
    [Route("api/[controller]")]
    public class GeocodingController : ControllerBase
    {
        private readonly IGeocodingService _geoService;

        public GeocodingController(IGeocodingService geocodingService)
        {
            _geoService = geocodingService;
        }

        [HttpGet("ReverseLookup")]
        public async Task<IActionResult> ReverseLookupCoordinates([FromQuery]decimal lat, [FromQuery]decimal lng)
        {
            var res = await _geoService.ReverseLookup(lat, lng);
            if (res != null)
            {
                return new JsonResult(new { FormattedAddress = res });
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
