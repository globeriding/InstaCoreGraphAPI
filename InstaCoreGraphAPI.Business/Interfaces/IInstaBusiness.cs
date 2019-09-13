using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using InstaCoreGraphAPI.Graph.Entity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace InstaCoreGraphAPI.Business.Interfaces
{
    public interface IInstaBusiness
    {
        string GetGraphApiUrl(string uri);

        List<SimpleMedia> GetMedia(int limit, string cursorBefore, string cursorAfter);

        InstagramInsight GetMediaImpressionsInsight(string mediaDataId);

        List<Comment> GetMediaCommentsEntities(string mediaDataId);

        Comments GetMediaCommentsDto(string mediaDataId);
        
    }
}
