using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtlasReaper.Jira
{
    class Download
    {
        internal void DownloadAttachments(JiraOptions.DownloadOptions options)
        {
            try
            {
                List<string> attachmentIds = options.Attachments.Split(',').ToList();

                // Iterate over each attachment id supplied
                foreach (string attachmentId in attachmentIds)
                {
                    Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                    // Construct url to return attachment information
                    string url = options.Url + "/rest/api/3/attachment/" + attachmentId;

                    // Get attachment information
                    Attachment attachment = webRequestHandler.GetJson<Attachment>(url, options.Cookie);

                    // Construct download url and file name
                    string downloadUrl = attachment.Content;
                    string fileName = attachment.FileName;

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
