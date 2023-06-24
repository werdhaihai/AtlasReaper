using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtlasReaper.Confluence
{
    internal class Link
    {
        internal void AddLink(Options.ConfluenceOptions.LinkOptions options)
        {
            try
            {
                string linkMessage = "";

                if (options.At != null)
                {
                    List<string> ats = options.At.Split(',').ToList();
                    linkMessage += "<p>";
                    foreach (string at in ats)
                    {
                        linkMessage += "<ac:link><ri:user ri:account-id=\"" + at + "\" /></ac:link>";
                    }
                    linkMessage += "</p>";
                }
                if (options.Message != null)
                {
                    linkMessage += options.Message;
                }

                linkMessage += " <a href=\"" + options.Link + "\">" + options.Text + "</a>";
                

                // Build page url
                string pageUrl = options.Url + "/wiki/api/v2/pages/" + options.Page + "?body-format=storage";

                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                // GET page
                Page page = webRequestHandler.GetJson<Page>(pageUrl, options.Cookie);

                PutBody putBody = new PutBody();

                putBody.Id = page.Id;
                putBody.Status = page.Status;
                putBody.Title = page.Title;
                putBody.SpaceId = page.SpaceId;
                putBody.Body = page.Body;
                putBody.Body.Storage.Value += linkMessage;
                putBody.Body.Storage.Representation = "storage";
                putBody.Version = page.Version;
                putBody.Version.Number += 1;

                string serializedPage = JsonConvert.SerializeObject(putBody);

                // PUT page
                webRequestHandler.PutJson<Page>(pageUrl, options.Cookie, serializedPage);

                // Get page to show updated page

                // GET page
                page = webRequestHandler.GetJson<Page>(pageUrl, options.Cookie);
                Console.WriteLine("Created link on page id: " + options.Page);
                Console.WriteLine("Output of " + page.Title + " after update.");
                Console.WriteLine();
                Console.WriteLine(page.Body.Storage.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while adding link: " + ex.Message);
            }
            
        }
    }
}
