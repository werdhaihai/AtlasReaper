using Newtonsoft.Json;
using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtlasReaper.Confluence
{
    class Attach
    {
        internal void AttachFile(Options.ConfluenceOptions.AttachOptions options)
        {
            try
            {

                string url = options.Url + "/wiki/rest/api/content/" + options.Page + "/child/attachment";
                string fileName = options.Name;


                if (options.AttachmentId != null && options.File == null)
                {
                    // Attach existing attachment

                    string attachmentUrl = options.Url + "/wiki/api/v2/attachments/" + options.AttachmentId;
                    Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
                    AttachmentResult attachmentResult = webRequestHandler.GetJson<AttachmentResult>(attachmentUrl, options.Cookie);

                    string attachmentPage = attachmentResult.DownloadLink.Split('/')[3];

                    if (options.Page == null)
                    {
                        options.Page = attachmentPage;
                    }

                    if (options.Page != attachmentPage)
                    {
                        Console.WriteLine("Must attach to the same page the attachment is uploaded to.");
                        Console.WriteLine("Attachment is uploaded to " + attachmentPage);
                        return;
                    }

                    AttachPage(attachmentResult.Title, options);

                }
                if (options.File != null)
                {
                    if (options.Page == null)
                    {
                        Console.WriteLine("Please specify a page with -p/--page");
                        return;
                    }
                    if (options.Name == null)
                    {
                        fileName = Path.GetFileName(options.File);
                    }

                    RootAttachObject attachmentObject = UploadFile(url, options, fileName);
                    if (attachmentObject.Results.Count < 1)
                    {
                        Console.WriteLine("Attachment already exists with the name " + fileName);
                        Console.WriteLine();
                        Console.WriteLine("    Use -a/--attachment to specify an existing attachment");
                        return;
                    }

                    Console.WriteLine("Uploaded " + fileName);
                    Console.WriteLine("Attachment Id: " + attachmentObject.Results[0].Id);

                    AttachPage(attachmentObject.Results[0].Title, options);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while adding attaching file : " + ex.Message);
            }



        }

        internal void AttachPage(string attachmentTitle, ConfluenceOptions.AttachOptions options)
        {
            // Build page url
            string pageUrl = options.Url + "/wiki/api/v2/pages/" + options.Page + "?body-format=storage";
            string attachText = "";
            Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();


            if (options.At != null)
            {
                List<string> ats = options.At.Split(',').ToList();
                attachText += "<p>";
                foreach (string at in ats)
                {
                    attachText += "<ac:link><ri:user ri:account-id=\"" + at + "\" /></ac:link>";
                }

                attachText += "</p><p>" + options.Text + "</p>\r\n<p><ac:structured-macro ac:name=\"view-file\" ac:schema-version=\"1\"><ac:parameter ac:name=\"name\"><ri:attachment ri:filename=\"" +
                    attachmentTitle +
                    "\" /></ac:parameter><ac:parameter ac:name=\"height\">250</ac:parameter></ac:structured-macro></p>";
            }

            else
            {
                attachText = "<p>" + options.Text + "</p>\r\n<ac:structured-macro ac:name=\"view-file\" ac:schema-version=\"1\"><ac:parameter ac:name=\"name\"><ri:attachment ri:filename=\"" + attachmentTitle + "\" /></ac:parameter><ac:parameter ac:name=\"height\">250</ac:parameter></ac:structured-macro>";
            }

            Page page = webRequestHandler.GetJson<Page>(pageUrl, options.Cookie);

            PutBody putBody = new PutBody();

            putBody.Id = page.Id;
            putBody.Status = page.Status;
            putBody.Title = page.Title;
            putBody.SpaceId = page.SpaceId;
            putBody.Body = page.Body;
            putBody.Body.Storage.Value += attachText;
            putBody.Body.Storage.Representation = "storage";
            putBody.Version = page.Version;
            putBody.Version.Number += 1;

            string serializedPage = JsonConvert.SerializeObject(putBody);

            // PUT page
            webRequestHandler.PutJson<Page>(pageUrl, options.Cookie, serializedPage);

            // GET page
            page = webRequestHandler.GetJson<Page>(pageUrl, options.Cookie);
            Console.WriteLine("Attached file to page id: " + page.Id);
            Console.WriteLine("Output of " + page.Title + " after update.");
            Console.WriteLine();
            Console.WriteLine(page.Body.Storage.Value);
        }

        internal RootAttachObject UploadFile(string url, ConfluenceOptions.AttachOptions options, string fileName)
        {
            FormData formData = new FormData
            {
                comment = options.Comment,
                file = File.ReadAllBytes(options.File)
            };
            Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
            RootAttachObject attachmentObject = webRequestHandler.PostForm<RootAttachObject>(url, options.Cookie, formData, fileName);

            return attachmentObject;
        }
    }

    public class FormData
    {
        public string comment { get; set; }
        public byte[] file { get; set; }
    }

    internal class RootAttachObject
    {
        [JsonProperty("results")]
        internal List<AttachmentResult> Results { get; set; }

        [JsonProperty("totalSize")]
        internal int TotalSize { get; set; }

        [JsonProperty("_links")]
        internal _Links _Links { get; set; }
    }

    internal class AttachmentResult
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("metadata")]
        internal MetaData MetaData { get; set; }

        [JsonProperty("downloadLink")]
        internal string DownloadLink { get; set; }
    }

    internal class MetaData
    {
        [JsonProperty("mediaType")]
        internal string mediaType { get; set; }
    }
}
