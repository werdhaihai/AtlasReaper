using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AtlasReaper.Options;

namespace AtlasReaper.Confluence
{
    internal class Pages
    {
        internal void ListPages(ConfluenceOptions.ListPagesOptions options)
        {
            try
            {
                List<Page> pages = new List<Page>();
                if (options.Page != null)
                {
                    // List specific page
                    Page page = new Page();
                    page = GetPage(options);
                    pages.Add(page);
                    options.Body = true;
                }
                else if (options.AllPages && options.Space == null)
                {
                    // List all pages for all spaces
                    // List all spaces
                    ConfluenceOptions.ListSpacesOptions spacesOptions = new ConfluenceOptions.ListSpacesOptions()
                    {
                        Url = options.Url,
                        Cookie = options.Cookie,
                        Limit = options.Limit
                    };

                    List<Space> spaces = Spaces.GetAllSpaces(spacesOptions);

                    for (int i = 0; i < spaces.Count; i++)
                    {
                        List<Page> spacePages = new List<Page>();
                        options.Space = spaces[i].Id;
                        spacePages = GetAllPages(options);
                        pages.AddRange(spacePages);
                    }

                    pages = pages.OrderByDescending(o => o.Version.CreatedAt).ToList();
                }
                else if (options.Space != null)
                {
                    // List pages for Space
                    RootPagesObject pagesList = GetPages(options);
                    pages = pagesList.Results.ToList();
                    pages = pages.OrderByDescending(o => o.Version.CreatedAt).ToList();
                }
                else 
                {
                    Console.WriteLine("Please use one of: ");
                    Console.WriteLine();
                    Console.WriteLine("  --all           (Default: false) Return all pages (Returns every Page if no Space is specified)");
                    Console.WriteLine("  -p, --page      Page to return");
                    Console.WriteLine("  -s, --space     Space to search");
                    Console.WriteLine();
                }

                pages = pages.Where(page => page != null && page.Id != null).ToList();
                if (pages.Count > 0)
                {
                    if (options.outfile != null)
                    {
                        using (StreamWriter writer = new StreamWriter(options.outfile))
                        {
                            PrintPage(pages, options.Body, writer);
                        }
                    }
                    else
                    {
                        PrintPage(pages, options.Body, Console.Out);
                    }
                }
                else
                {
                    Console.WriteLine("No pages returned.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while listing pages: " + ex.Message);
            }
            
          
        }

        // Get Page
        internal Page GetPage(ConfluenceOptions.ListPagesOptions options)
        {
            Page page = new Page();
            string url = options.Url + "/wiki/api/v2/pages/" + options.Page + "?body-format=atlas_doc_format";
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
                page = webRequestHandler.GetJson<Page>(url, options.Cookie);

                return page;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting page: " + ex.Message);
                return page;
            }

        }

        // Get pages based on options
        internal static RootPagesObject GetPages(ConfluenceOptions.ListPagesOptions options, string paginationToken = null)
        {
            RootPagesObject pagesList = new RootPagesObject();

            string url =
                options.Url + "/wiki/api/v2/spaces/" + options.Space +
                "/pages?limit=" + options.Limit +
                "&status=" + options.Status;
            try
            {
                if (options.Body)
                {
                    url = url + "&body-format=atlas_doc_format";
                }

                if (paginationToken != null)
                {
                    url += "&" + paginationToken;
                }

                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                pagesList = webRequestHandler.GetJson<RootPagesObject>(url, options.Cookie);

                return pagesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while getting pages: " + ex.Message);
                return pagesList;
            }


        }

        // Get text values from JSON token
        internal static List<string> GetPageText(JToken token, string key)
        {
            List<String> results = new List<string>();

            try
            {
                if (token.Type == JTokenType.Object)
                {
                    JObject obj = (JObject)token;

                    foreach (JProperty property in obj.Properties())
                    {
                        if (property.Name == key)
                        {
                            results.Add(property.Value.ToString());
                        }

                        results.AddRange(GetPageText(property.Value, key));
                    }
                }
                else if (token.Type == JTokenType.Array)
                {
                    foreach (JToken item in token)
                    {
                        results.AddRange(GetPageText(item, key));
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while parsing page text: " + ex.Message);
                return results;
            }

        }

        // Get all pages for a space
        internal static List<Page> GetAllPages(ConfluenceOptions.ListPagesOptions options)
        {
            List<Page> pages = new List<Page>();

            // Set limit to 250 to reduce number of request
            options.Limit = "250";

            try
            {
                RootPagesObject pagesList = GetPages(options);
                pages = pagesList.Results;

                while (pagesList != null && pagesList._Links.Next != null)
                {
                    string[] nextTokenParts = pagesList._Links.Next.Split('?').Last().Split('&');
                    string nextToken = "";
                    foreach (string param in nextTokenParts)
                    {
                        if (param.Contains("cursor"))
                        {
                            nextToken = param;
                        }
                    }
                    //string nextToken = pagesList._Links.Next.Split('?').Last();

                    pagesList = GetPages(options, nextToken);
                    pages.AddRange(pagesList.Results);
                }

                pages = pages.OrderByDescending(o => o.Version.CreatedAt).ToList();

                return pages;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting all pages: " + ex.Message);
                return pages;
            }
            
        }

        // Print page information
        internal static void PrintPage(List<Page> pages, bool body, TextWriter writer)
        {
            try
            {
                
                for (int i = 0; i < pages.Count; i++)
                {
                    Page page = pages[i];
                    writer.WriteLine("Page Title: " + page.Title);
                    writer.WriteLine("Updated   : " + page.Version.CreatedAt);
                    writer.WriteLine("Page Id   : " + page.Id);
                    if (body)
                    {
                        JToken json = JObject.Parse(page.Body.Atlas_Doc_Format.Value);
                        List<string> textValues = GetPageText(json, "text");
                        string bodyText = string.Join("\n                ", textValues);
                        writer.WriteLine("Page Body : ");
                        writer.WriteLine("                " + bodyText);
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while printing page with body: " + ex.Message);
            }
        }

    }

    internal class RootPagesObject
    {
        [JsonProperty("results")]
        internal List<Page> Results { get; set; }
        [JsonProperty("_links")]
        internal _Links _Links { get; set; }
    }

    internal class Page 
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("status")]
        internal string Status { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("spaceId")]
        internal string SpaceId { get; set; }
        [JsonProperty("parentId")]
        internal string ParentId { get; set; }
        [JsonProperty("authorId")]
        internal string AuthorId { get; set; }
        [JsonProperty("createdAt")]
        internal string CreatedAt { get; set; }
        [JsonProperty("version")]
        internal Version Version { get; set; }
        [JsonProperty("body")]
        internal Body Body { get; set; }
    }

    internal class Version
    {
        [JsonProperty("createdAt")]
        internal string CreatedAt { get; set; }
        [JsonProperty("message")]
        internal string Message { get; set; }
        [JsonProperty("number")]
        internal int Number { get; set; }
        [JsonProperty("minorEdit")]
        internal bool MinorEdit { get; set; }
        [JsonProperty("authorId")]
        internal string AuthorId { get; set; }
    }

    internal class Body
    {
        [JsonProperty("storage")]
        internal BodyType Storage { get; set; }

        [JsonProperty("atlas_doc_format")]
        internal BodyType Atlas_Doc_Format { get; set; }
    }

    internal class BodyType
    {
        [JsonProperty("value")]
        internal string Value { get; set; }
        [JsonProperty("representation")]
        internal string Representation { get; set; }
    }

}
