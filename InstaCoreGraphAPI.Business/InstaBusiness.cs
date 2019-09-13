using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using InstaCoreGraphAPI.Business.Interfaces;
using InstaCoreGraphAPI.Graph.Entity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace InstaCoreGraphAPI.Business
{
    public class InstaBusiness : IInstaBusiness
    {
        private readonly string _accessToken;
        protected static IConfiguration Configuration;
        private readonly string _fbGraphApiBaseUrl;
        private readonly string _instagramId;
        private IInstaBusiness _instaBusinessImplementation;

        public InstaBusiness(IConfiguration configuration)
        {
            Configuration = configuration;
            _accessToken = Configuration["AppSettings:AccessToken"];
            _fbGraphApiBaseUrl = Configuration["AppSettings:fbGraphApiBaseUrl"];
            _instagramId = Configuration["AppSettings:InstagramId"];
        }


        /// <summary>
        /// Launch Get request
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetGraphApiUrl(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            string returnStr;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                returnStr = reader.ReadToEnd();
            }

            return returnStr;
        }

        private Media DoMediaSearch(int limit = 25, string cursorBefore = null, string cursorAfter = null)
        {
            // get the list of media items
            // parse out the response and the fields we want
            // convert to DTOs and return
            string mediaSearchUrl = $"{_fbGraphApiBaseUrl}/{_instagramId}/media?access_token={_accessToken}&fields=media_url%2Cmedia_type%2Ccomments_count%2Clike_count%2Ctimestamp%2Cpermalink%2Ccaption";

            mediaSearchUrl += $"&limit={limit}";

            if (!string.IsNullOrEmpty(cursorBefore))
            {
                mediaSearchUrl += $"&before={cursorBefore}";
            }
            if (!string.IsNullOrEmpty(cursorAfter) && string.IsNullOrEmpty(cursorBefore))
            {
                mediaSearchUrl += $"&after={cursorAfter}";
            }

            //invoke the request
            string jsonResult = this.GetGraphApiUrl(mediaSearchUrl);

            // convert to json annotated object
            return JsonConvert.DeserializeObject<Media>(jsonResult);
        }

        public List<SimpleMedia> GetMedia(int limit = 25, string cursorBefore = null, string cursorAfter = null)
        {
            // invoke the private method - DoMediaSearch()
            Media instagramResults = DoMediaSearch(limit, cursorBefore, cursorAfter);
            List<SimpleMedia> mediaModels = new List<SimpleMedia>();

            //map from the JSON/DTO returned by DoMediaSearch() to the Domain Entities

            foreach (MediaData mediaData in instagramResults.data)
            {
                InstagramInsight insight = GetMediaImpressionsInsight(mediaData.id);

                mediaModels.Add(
                    new SimpleMedia
                    {
                        id = mediaData.id,
                        like_count = mediaData.like_count,
                        comments_count = mediaData.comments_count,
                        impression_count = insight.data.Find(i => i.name == "impressions").values[0].value,
                        engagement_count = insight.data.Find(i => i.name == "engagement").values[0].value,
                        reach_count = insight.data.Find(i => i.name == "reach").values[0].value,
                        media_url = mediaData.media_url,
                        permalink = mediaData.permalink,
                        //Comments = GetMediaCommentsEntities(mediaData.id),
                        timestamp = mediaData.timestamp
                    });
            }

            return mediaModels;
        }

        public BusinessDiscovery GetBusinessDiscovery(string instagramId, string accountName)
        {
            string businessUrl = $"{_fbGraphApiBaseUrl}/{instagramId}?fields=business_discovery.username({accountName})%7Bfollowers_count%2Cmedia_count%7D&access_token={_accessToken}";
            return JsonConvert.DeserializeObject<BusinessDiscovery>(GetGraphApiUrl(businessUrl));
        }

        public SimpleMedia GetMediaInsight(string id)
        {
            //url for one media
            string mediaUrl = $"{_fbGraphApiBaseUrl}/{id}?access_token={_accessToken}&fields=media_url%2Cmedia_type%2Ccomments_count%2Clike_count%2Ctimestamp%2Cpermalink%2Ccaption";
            //invoke the request
            string jsonResult = this.GetGraphApiUrl(mediaUrl);
            // convert to json annotated object
            MediaData mediaData = JsonConvert.DeserializeObject<MediaData>(jsonResult);

            InstagramInsight insight = GetMediaImpressionsInsight(mediaData.id);

            return 
                new SimpleMedia
                {
                    id = mediaData.id,
                    like_count = mediaData.like_count,
                    comments_count = mediaData.comments_count,
                    impression_count = insight.data.Find(i => i.name == "impressions").values[0].value,
                    engagement_count = insight.data.Find(i => i.name == "engagement").values[0].value,
                    reach_count = insight.data.Find(i => i.name == "reach").values[0].value,
                    media_url = mediaData.media_url,
                    permalink = mediaData.permalink,
                    //Comments = GetMediaCommentsEntities(mediaData.id),
                    timestamp = mediaData.timestamp
                };
        }

        private InstagramInsight GetMediaImpressionsInsight(string mediaDataId)
        {
            string impressionsUrl = $"{_fbGraphApiBaseUrl}/{mediaDataId}/insights/?metric=impressions%2Cengagement%2Creach&access_token={_accessToken}";

            InstagramInsight instagramInsight;

            string jsonResult = GetGraphApiUrl(impressionsUrl);

            instagramInsight = JsonConvert.DeserializeObject<InstagramInsight>(jsonResult);

            return instagramInsight;
        }

        private List<Comment> GetMediaCommentsEntities(string mediaDataId)
        {
            Comments commentsDtOs = GetMediaCommentsDto(mediaDataId);
            List<Comment> comments = new List<Comment>();

            foreach (CommentData commentData in commentsDtOs.data)
            {
                comments.Add(new Comment { id = commentData.id, text = commentData.text });
            }

            return comments;
        }

        private Comments GetMediaCommentsDto(string mediaDataId)
        {
            string commentsUrl = $"{_fbGraphApiBaseUrl}/{mediaDataId}/comments?access_token={_accessToken}";
            return JsonConvert.DeserializeObject<Comments>(GetGraphApiUrl(commentsUrl));
        }

       
    }
}
