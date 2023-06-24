using Newtonsoft.Json;
using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtlasReaper.Confluence
{
    internal class Embed
    {
        internal void EmbedIframe(ConfluenceOptions.EmbedOptions options)
        {
            try
            {
                string imgMessage = "";
                if (options.At != null)
                {
                    List<string> ats = options.At.Split(',').ToList();
                    imgMessage += "<p>";
                    foreach (string at in ats)
                    {
                        imgMessage += "<ac:link><ri:user ri:account-id=\"" + at + "\" /></ac:link>";
                    }
                    imgMessage += "</p>";
                }
                if (options.Message != null)
                {
                    imgMessage += options.Message;
                }
                // Build embed iframe
                //embedIframe += "<p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><p></p><ac:structured-macro ac:name=\"iframe\" ac:schema-version=\"1\" data-layout=\"default\"><ac:parameter ac:name=\"src\"><ri:url ri:value=\"" + options.Link + "\" /></ac:parameter><ac:parameter ac:name=\"width\">1</ac:parameter><ac:parameter ac:name=\"frameborder\">hide</ac:parameter><ac:parameter ac:name=\"height\">1</ac:parameter></ac:structured-macro>";

                imgMessage = "<img src=\"" + options.Link + "\" />";
                
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
                putBody.Body.Storage.Value += imgMessage;
                putBody.Body.Storage.Representation = "storage";
                putBody.Version = page.Version;
                putBody.Version.Number += 1;

                string serializedPage = JsonConvert.SerializeObject(putBody);

                // PUT page
                webRequestHandler.PutJson<Page>(pageUrl, options.Cookie, serializedPage);

                // GET page
                page = webRequestHandler.GetJson<Page>(pageUrl, options.Cookie);
                Console.WriteLine("Embedded image on page id: " + page.Id);
                Console.WriteLine("Output of " + page.Title + " after update.");
                Console.WriteLine();
                Console.WriteLine(page.Body.Storage.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while embedding image: " + ex.Message);
            }


        }

    }
    
    internal class PutBody
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("status")]
        internal string Status { get; set; }
        [JsonProperty("title")]
        internal string Title { get; set; }
        [JsonProperty("spaceId")]
        internal string SpaceId { get; set; }
        [JsonProperty("body")]
        internal Body Body { get; set; }
        [JsonProperty("version")]
        internal Version Version { get; set; }
    }
}
