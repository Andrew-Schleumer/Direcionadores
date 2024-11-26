using Placemarks.Models;
using Placemarks.Services;
using Microsoft.AspNetCore.Mvc;
using SharpKml.Dom;
using SharpKml.Engine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Placemarks.Exceptions;
using System.Runtime.CompilerServices;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.Security.Cryptography.Xml;

namespace Placemarks.Controllers
{
    [ApiController]
    [Route("api/placemarks")]
    public class PlacemarksController : Controller
    {
        private readonly IPlacemarksService _placemarkService;

        private readonly ILogger<PlacemarksController> _logger;

        public PlacemarksController(ILogger<PlacemarksController> logger, IPlacemarksService placemarkService)
        {
            _logger = logger;
            _placemarkService = placemarkService;
        }

        [HttpPost("export")]
        public ActionResult ExportKml([FromBody] Filter filter)
        {
            try
            {
                byte[] kml = _placemarkService.GetFilteredKml(filter.Cliente, filter.Situacao, filter.Bairro, filter.Referencia, filter.RuaCruzamento);

                return File(new MemoryStream(kml), "application/octet-stream", fileDownloadName: "FilteredKml" + DateTime.Now + ".kml");
            }
            catch (FilteringException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Algo deu errado " + ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<string> GetJson(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento)
        {
            try
            {
                List<PlacemarkView> result = _placemarkService.GetJson(cliente, situacao, bairro, referencia, ruaCruzamento);

                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (FilteringException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Algo deu errado " + ex.Message);
            }
        }

        [HttpGet("filters")]
        public ActionResult<string> GetFilters()
        {
            JObject filters = _placemarkService.GetFilters();

            return Ok(filters.ToString());
        }

    }
}
