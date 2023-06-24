using System;
using System.Collections.Generic;
using System.Linq;
using AtlasReaper.Options;

namespace AtlasReaper.Confluence
{
    internal class Download
    {
        internal void DownloadAttachments(ConfluenceOptions.DownloadOptions options)
        {
            try
            {
                List<string> attachments = options.Attachments.Split(',').ToList();

                // Iterate over each attachment id supplied
                foreach (string attachment in attachments)
                {
                    Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                    // Construct url to return attachment information
                    string url = options.Url + "/wiki/rest/api/search?cql=type=attachment+AND+Id=" + attachment + "&expand=content.extensions";

                    // Get attachment information
                    RootAttachmentsObject attachmentObj = webRequestHandler.GetJson<RootAttachmentsObject>(url, options.Cookie);

                    // Construct download url and file name
                    string downloadUrl = options.Url + "/wiki" + attachmentObj.Results[0].AttachmentContent._ContentLinks.Download;
                    string fileName = attachmentObj.Results[0].AttachmentContent.Title;
                    
                    // Set path for file
                    string fullPath = options.OutputDir + fileName;

                    // Download the attachment
                    webRequestHandler.DownloadFile(downloadUrl, options.Cookie, fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while downloading attachments: " + ex.Message);
            }

        }
    }
}
