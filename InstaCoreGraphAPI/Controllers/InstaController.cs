using System.Collections.Generic;
using InstaCoreGraphAPI.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using InstaCoreGraphAPI.Graph.Entity;

namespace InstaCoreGraphAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InstaController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IInstaBusiness _instaBusiness;
        private static IConfiguration _configuration { get; set; }

        private readonly string _instagramId;
        private string _access_token;
        private readonly string _fbGraphApiBaseUrl;
        
        public InstaController(ILogger<InstaController> logger, IConfiguration configuration, IInstaBusiness instabusiness)
        {
            _logger = logger;
            _instaBusiness = instabusiness;
            _configuration = configuration;
            _instagramId = _configuration["AppSettings:InstagramId"];
            _access_token = _configuration["AppSettings:AccessToken"];
            _fbGraphApiBaseUrl = _configuration["AppSettings:fbGraphApiBaseUrl"];
    }

        /// <summary>
        /// Get results from an url
        /// </summary>
        /// <param name="uri">an url of facebook graph api</param>
        /// <returns>A json file </returns>
        [HttpGet("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Success.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server error.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request", typeof(string))]
        public IActionResult Get(string uri)
        {
            var returnStr = string.Empty;
            if (!string.IsNullOrEmpty(uri))
                returnStr= _instaBusiness.GetGraphApiUrl(uri);

            return Ok(returnStr);
        }

        /// <summary>
        /// Get medias list from instagram graph api
        /// </summary>
        /// <param name="limit">number of medias to display</param>
        /// <param name="cursorAfter">cursor for the next list of medias </param>
        /// <param name="cursorBefore">cursor for the previous list of medias </param>
        /// <returns>A json file </returns>
        [HttpGet("GetMedias")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Success.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server error.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request", typeof(string))]
        public IActionResult GetMedias(int limit = 25, string cursorBefore = null, string cursorAfter = null)
        {
            List<SimpleMedia> objectResult = _instaBusiness.GetMedia(limit, cursorBefore, cursorAfter);
            return Ok(objectResult);
        }


        [HttpGet("GetMediaInsight")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Success.")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal Server error.")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Bad request", typeof(string))]
        public IActionResult GetMediaInsight(string mediaId)
        {
            SimpleMedia objectResult = _instaBusiness.GetMediaInsight(mediaId);
            return Ok(objectResult);
        }

        /// <summary>
        /// Get Business Discovery
        /// </summary>
        /// <param name="accountName">Instagram account name</param>
        /// <returns></returns>
        [HttpGet("GetBusinessDiscovery")]
        [SwaggerResponse((int) HttpStatusCode.OK, "Success.")]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, "Internal Server error.")]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, "Bad request", typeof(string))]
        public IActionResult GetBusinessDiscovery(string accountName)
        {
            BusinessDiscovery business = _instaBusiness.GetBusinessDiscovery(_instagramId, accountName);
            return Ok(business);
        }
    }
}