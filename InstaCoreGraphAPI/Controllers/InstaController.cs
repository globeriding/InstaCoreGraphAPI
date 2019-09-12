using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InstaCoreGraphAPI.Graph.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace InstaCoreGraphAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InstaController : ControllerBase
    {
        protected readonly ILogger _logger;
        protected static IConfiguration _configuration { get; set; }

        private readonly string _instagramId;
        private string _access_token;
        private readonly string _fbGraphApiBaseUrl;
        
        public InstaController(ILogger<InstaController> logger, IConfiguration configuration)
        {
            _logger = logger;
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
                returnStr= GetGraphApiUrl(uri);

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
            //https://graph.facebook.com/v4.0/xxinstagramidxx/media?access_token=xxtokenxx&limit=25

            var path = $"{_fbGraphApiBaseUrl}/{_instagramId}/media?access_token={_access_token}";

            path += $"&limit={limit}";

            if (!string.IsNullOrEmpty(cursorBefore))
            {
                path += $"&before={cursorBefore}";
            }
            if (!string.IsNullOrEmpty(cursorAfter))
            {
                path += $"&after={cursorAfter}";
            }

            var returnStr = GetGraphApiUrl(path);
            return Ok(returnStr);
        }

        [NonAction]
        private static string GetGraphApiUrl(string uri)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string returnStr;
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                returnStr = reader.ReadToEnd();
            }

            return returnStr;
        }

        private List<InstagramResult> DoMediaSearch()
        {
            // get the list of media items
            // parse out the reponse and the fields we want
            // convert to DTOs and return

            string mediaFields = "media%7Bmedia_url%2Cmedia_type%2Ccomments_count%2Clike_count%2Ctimestamp%2Cpermalink%2Ccaption%7D";
            string mediaSearchUrl =String.Format(_fbGraphApiBaseUrl, _instagramId,mediaFields + "&access_token=" + _access_token);

            List<InstagramResult> list = new List<InstagramResult>();

            //invoke the request
            string jsonResult = GetGraphApiUrl(mediaSearchUrl);

            // convert to json annotated object
            InstagramResult instagramResult = new InstagramResult();
            instagramResult = JsonConvert.DeserializeObject<InstagramResult>(jsonResult);

            if (instagramResult != null && instagramResult.media != null)
            {
                foreach (MediaData mediaData in instagramResult.media.data)
                {
                    list.Add(instagramResult);
                }
            }

            return list;
        }
        [NonAction]
        public List<SimpleMedia> GetMedia()
        {
            // invoke the private method - DoMediaSearch()
            List<InstagramResult> instagramResults = this.DoMediaSearch();
            List<SimpleMedia> mediaModels = new List<SimpleMedia>();

            //map from the JSON/DTO returned by DoMediaSearch() to the Domain Entities
            foreach (InstagramResult instagramResult in instagramResults)
            {
                foreach (MediaData mediaData in instagramResult.media.data)
                {
                    mediaModels.Add(
                        new SimpleMedia
                        {
                            id = mediaData.id,
                            like_count = mediaData.like_count,
                            comments_count = mediaData.comments_count,
                            impression_count = GetMediaImpressionValue(GetMediaImpressionsInsight(mediaData)),
                            media_url = mediaData.media_url,
                            permalink = mediaData.permalink,
                            Comments = GetMediaCommentsEntities(mediaData),
                            timestamp = mediaData.timestamp,
                            DateCreated = mediaData.DateCreated
                        });
                }
            }
            return mediaModels;
        }

        private InstagramInsight GetMediaImpressionsInsight(MediaData mediaData)
        {
            string impressionsUrl = "https://graph.facebook.com/v3.2/" + mediaData.id + "/insights/?metric=impressions&access_token=" + _access_token;

            InstagramInsight instagramInsight;

            string jsonResult = GetGraphApiUrl(impressionsUrl);

            instagramInsight = JsonConvert.DeserializeObject<InstagramInsight>(jsonResult);

            return instagramInsight;
        }

        private int GetMediaImpressionValue(InstagramInsight insight)
        {
            return insight.data.Find(i => i.name == "impressions").values[0].value;
        }

        private List<Comment> GetMediaCommentsEntities(MediaData mediaData)
        {
            Comments commentsDTOs = GetMediaCommentsDTO(mediaData);
            List<Comment> comments = new List<Comment>();

            foreach (CommentData commentData in commentsDTOs.data)
            {
                comments.Add(new Comment { id = commentData.id, text = commentData.text });
            }

            return comments;
        }

        private Comments GetMediaCommentsDTO(MediaData mediaData)
        {
            string commentsUrl = "https://graph.facebook.com/v3.2/" + mediaData.id + "/comments?access_token=" + _access_token;

            Comments comments = new Comments();

            string jsonResult = GetGraphApiUrl(commentsUrl);

            comments = JsonConvert.DeserializeObject<Comments>(jsonResult);

            return comments;
        }
    }
}