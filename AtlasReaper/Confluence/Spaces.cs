using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AtlasReaper.Options;

namespace AtlasReaper.Confluence
{
    internal class Spaces
    {
        // List spaces based on options
        internal void ListSpaces(ConfluenceOptions.ListSpacesOptions options)
        {
            try
            {
                List<Space> spaces = new List<Space>(); 

                if (options.Space != null)
                {
                    // Get a single space
                    Space space = GetSpace(options);
                    spaces.Add(space);
                }
                else if (options.AllSpaces)
                {
                    // List all spaces
                    spaces = GetAllSpaces(options);
                }
                else
                {
                    // List Spaces by limit
                    RootSpacesObject spacesList = GetSpaces(options);
                    spaces = spacesList.Results.ToList();
                    spaces = spaces.OrderBy(o => o.Type).ToList();
                }
                if (options.Type != null)
                {
                    spaces = spaces.Where(space => space != null && space.Type == options.Type).ToList();
                }
                if (options.outfile != null)
                {
                    using (StreamWriter writer = new StreamWriter(options.outfile))
                    {
                        PrintSpaces(spaces, writer);
                    }
                }
                else
                {
                    PrintSpaces(spaces, Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while listing spaces: " + ex.Message);
            }
            
            

        }

        // Get Spaces based on options
        internal static RootSpacesObject GetSpaces(ConfluenceOptions.ListSpacesOptions options, string paginationToken = null)
        {
            RootSpacesObject spaceList = new RootSpacesObject();
            var url = options.Url + "/wiki/api/v2/spaces?limit=" + options.Limit;
            if (paginationToken != null)
            {
                url += "&" + paginationToken;
            }

            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                spaceList = webRequestHandler.GetJson<RootSpacesObject>(url, options.Cookie);

                return spaceList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting spaces: " + ex.Message);
                return spaceList;
            }

        }

        // Get a single Space
        internal static Space GetSpace(ConfluenceOptions.ListSpacesOptions options)
        {
            Space space = new Space();
            string url = options.Url + "/wiki/api/v2/spaces/" + options.Space;
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
                space = webRequestHandler.GetJson<Space>(url, options.Cookie);
                return space;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting space: " + ex.Message);
                return space;
            }

        }

        // Get All Spaces 
        internal static List<Space> GetAllSpaces(ConfluenceOptions.ListSpacesOptions options)
        {
            List<Space> spaces = new List<Space>();
            // Set limit to 250 to reduce number of requests
            options.Limit = "250";
            try
            {
                RootSpacesObject spacesList = GetSpaces(options);

                spaces = spacesList.Results;

                while (spacesList != null && spacesList._Links.Next != null)
                {
                    string nextToken = spacesList._Links.Next.Split('&').Last();

                    spacesList = GetSpaces(options, nextToken);
                    spaces.AddRange(spacesList.Results);
                }

                spaces = spaces.OrderBy(o => o.Type).ToList();

                return spaces;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting all spaces: " + ex.Message);
                return spaces;
            }

        }

        // Print Spaces information
        internal void PrintSpaces(List<Space> spaces, TextWriter writer)
        {
            try
            {
                for (int i = 0; i < spaces.Count; i++)
                {
                    Space space = spaces[i];
                    writer.WriteLine("    Space Name  : " + space.Name);
                    writer.WriteLine("    Space Id    : " + space.Id);
                    writer.WriteLine("    Space Key   : " + space.Key);
                    writer.WriteLine("    Space Type  : " + space.Type);
                    //writer.WriteLine("Space Description: " + space.Description);
                    writer.WriteLine("    Space Status: " + space.Status);
                    writer.WriteLine();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while printing space: " + ex.Message);
            }

        }
    }

    internal class RootSpacesObject
    {
        [JsonProperty("results")]
        internal List<Space> Results { get; set; }

        [JsonProperty("_links")]
        internal _Links _Links { get; set; }
    }

    internal class Space
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("key")]
        internal string Key { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("status")]
        internal string Status { get; set; }

        [JsonProperty("homepageId")]
        internal string HomepageId { get; set; }

        [JsonProperty("description")]
        internal Description Description { get; set; }
    }

    internal class Description
    {
        [JsonProperty("plain")]
        internal string Plain { get; set; }

        [JsonProperty("view")]
        internal string View { get; set; }
    }

    internal class _Links
    {
        [JsonProperty("base")]
        internal string Base { get; set; }

        [JsonProperty("next")]
        internal string Next { get; set; }
    }
}


