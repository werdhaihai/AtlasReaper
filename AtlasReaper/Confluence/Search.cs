using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace AtlasReaper.Confluence
{
    internal class Search
    {
        internal void SearchConfluence(ConfluenceOptions.SearchOptions options)
        {
            try
            {
                //Perform search based on provided options
                if (!options.All)
                {
                    SearchObject searchObject = DoSearchAsync(options, options.Query);
                    PrintResults(searchObject.Results);
                }
                else
                {
                    // Return all results
                    SearchObject searchObject = DoSearchAsync(options, options.Query);

                    List<SearchResult> results = searchObject.Results;

                    while (searchObject != null && searchObject._Links.Next != null)
                    {
                        string paginationUrl = searchObject._Links.Base + searchObject._Links.Next;

                        searchObject = DoSearchAsync(options, options.Query, paginationUrl);
                        results.AddRange(searchObject.Results);
                    }

                    PrintResults(results);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while searching Confluence: " + ex.Message);
            }


        }

        internal SearchObject DoSearchAsync(ConfluenceOptions.SearchOptions options, string query, string paginationUrl = null)
        {
            try
            {
                // Encode query for Url
                query = WebUtility.UrlEncode(query);
                string url = options.Url + "/wiki/rest/api/search?cql=text~%22" + query + "~%22&limit=" + options.Limit;
                Console.WriteLine(url);
                if (paginationUrl != null)
                {
                    url = paginationUrl;
                }

                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                SearchObject searchObject = webRequestHandler.GetJson<SearchObject>(url, options.Cookie);

                return searchObject;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while performing the search: " + ex.Message);
                return null;
            }

        }

        internal void PrintResults(List<SearchResult> results)
        {
            try
            {
                //results = results.OrderByDescending(o => o.LastModified).ToList();
                for (int i = 0; i < results.Count; i++)
                {
                    SearchResult result = results[i];

                    Console.WriteLine("    Title  : " + result.Title.Replace("@@@hl@@@", "").Replace("@@@endhl@@@", ""));
                    Console.WriteLine("    Id     : " + result.Content.Id);
                    Console.WriteLine("    Type   : " + result.Content.Type);
                    Console.WriteLine("    Excerpt: " + result.Excerpt.Replace("@@@hl@@@", "").Replace("@@@endhl@@@", ""));
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while printing the search results: " + ex.Message);
            }

        }
    }

    internal class SearchObject
    {
        [JsonProperty("results")]
        internal List<SearchResult> Results { get; set; }   

        [JsonProperty("start")]
        internal int Start { get; set; }

        [JsonProperty("limit")]
        internal int Limit { get; set; }

        [JsonProperty("size")]
        internal int Size { get; set; }

        [JsonProperty("totalSize")]
        internal int TotalSize { get; set; }

        [JsonProperty("cqlQuery")]
        internal string CqlQuery { get; set; }

        [JsonProperty("searchDuration")]
        internal string SearchDuration { get; set; }
        
        [JsonProperty("_links")]
        internal _SearchLinks _Links { get; set; }
    }
    
    internal class SearchResult
    {
        [JsonProperty("content")]
        internal Content Content { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("excerpt")]
        internal string Excerpt { get; set; }

        [JsonProperty("url")]
        internal string Url { get; set; }

        [JsonProperty("resultGlobalContainer")]
        internal ResultGlobalContainer ResultGlobalContainer { get; set; }

        [JsonProperty("lastModified")]
        internal string LastModified { get; set; }

        [JsonProperty("friendlyLastModified")]
        internal string FriendlyLastModified { get; set; }

        [JsonProperty("score")]
        internal string Score { get; set; }

    }

    internal class Content
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("status")]
        internal string Status { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

    }
    internal class ResultGlobalContainer
    {
        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("displayUrl")]
        internal string DisplayUrl { get; set; }
    }

    internal class _SearchLinks
    {
        [JsonProperty("base")]
        internal string Base { get; set; }
        [JsonProperty("context")]
        internal string Context { get; set; }

        [JsonProperty("next")]
        internal string Next { get; set; }

        [JsonProperty("self")]
        internal string Self { get; set; }

    }
}
