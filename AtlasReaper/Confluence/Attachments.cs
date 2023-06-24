using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AtlasReaper.Options;

namespace AtlasReaper.Confluence
{
    class Attachments
    {

        // List attachmetns based on provided options
        internal void ListAttachments(ConfluenceOptions.ListAttachmentsOptions options)
        {
            try
            {
                List<Attachment> attachmentsList = new List<Attachment>();

                if (options.Space != null)
                {
                    if (int.TryParse(options.Space, out int result))
                    {
                        ConfluenceOptions.ListSpacesOptions spacesOptions = new ConfluenceOptions.ListSpacesOptions
                        {
                            Url = options.Url,
                            Cookie = options.Cookie,
                            Space = options.Space,
                        };

                        Space space = Spaces.GetSpace(spacesOptions);

                        options.Space = space.Key;
                    }

                    //Constructing URL for searching attachments in a given Space
                    string url = options.Url + "/wiki/rest/api/search?cql=type=attachment+AND+Space+=+%20" + options.Space + "%20&expand=content.extensions";

                    if (options.All)
                    {
                        url = url + "&limit=200";

                        // Get all attachments
                        RootAttachmentsObject rootAttachmentsObject = GetAttachments(options, url);
                        attachmentsList.AddRange(rootAttachmentsObject.Results);

                        while (attachmentsList.Count < rootAttachmentsObject.TotalSize)
                        {
                            string nextUrl = rootAttachmentsObject._Links.Base + rootAttachmentsObject._Links.Next;
                            rootAttachmentsObject = GetAttachments(options, nextUrl);
                            attachmentsList.AddRange(rootAttachmentsObject.Results);
                        }

                        attachmentsList = FilterAttachments(attachmentsList, options);
                    }
                    else
                    {
                        // Get attachments with a specified limit
                        url = url + "&limit=" + options.Limit;

                        RootAttachmentsObject rootAttachmentsObject = GetAttachments(options, url);
                        attachmentsList = rootAttachmentsObject.Results;

                        attachmentsList = FilterAttachments(attachmentsList, options);
                    }


                }

                else
                {
                    // Construct URL for searching all attachments
                    string url = options.Url + "/wiki/rest/api/search?cql=type=attachment&expand=content.extensions";

                    if (options.All)
                    {
                        // Get all attachments
                        attachmentsList = GetAllAttachments(options, url);
                        attachmentsList = FilterAttachments(attachmentsList, options);
                    }
                    else
                    {
                        // Get attachments with specified limit
                        url = url + "&limit=" + options.Limit;

                        RootAttachmentsObject rootAttachmentsObject = GetAttachments(options, url);

                        attachmentsList = rootAttachmentsObject.Results;
                        attachmentsList = FilterAttachments(attachmentsList, options);
                    }
                }

                if (options.outfile != null)
                {
                    using (StreamWriter writer = new StreamWriter(options.outfile))
                    {
                        PrintAttachments(attachmentsList, writer);
                    }
                }
                else
                {
                    PrintAttachments(attachmentsList, Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while listing attachments: " + ex.Message);
            }
        }

        // Filter attachments based on include or exclude options
        private List<Attachment> FilterAttachments(List<Attachment> attachmentsList, ConfluenceOptions.ListAttachmentsOptions options)
        {
            try
            {             
                // Exclude
                if (options.Exclude != null)
                {
                    List<string> excludeList = options.Exclude.Split(',').ToList();

                    foreach (string exclude in excludeList)
                    {
                        attachmentsList.RemoveAll(item =>
                        {
                            List<string> extension = item.AttachmentContent.Title.Split('.').ToList();
                            if (extension.LastOrDefault() == exclude)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        });
                    }
                }

                // Include
                if (options.Include != null)
                {
                    List<string> includeList = options.Include.Split(',').ToList();
                    attachmentsList = attachmentsList.Where(item =>
                    {
                        string extension = item.AttachmentContent.Title.Split('.').LastOrDefault();
                        return includeList.Contains(extension);
                    }).ToList();
                }

                return attachmentsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while filtering attachments: " + ex.Message);
                return attachmentsList;
            }
        }

        // Get all attachments 
        private List<Attachment> GetAllAttachments(ConfluenceOptions.ListAttachmentsOptions options, string url)
        {
            List<Attachment> attachmentsList = new List<Attachment>();
            try 
            { 
                url = url + "&limit=200";
                
                RootAttachmentsObject rootAttachmentsObject = GetAttachments(options, url);
                attachmentsList.AddRange(rootAttachmentsObject.Results);

                while (attachmentsList.Count < rootAttachmentsObject.TotalSize)
                {
                    string nextUrl = rootAttachmentsObject._Links.Base + rootAttachmentsObject._Links.Next;
                    rootAttachmentsObject = GetAttachments(options, nextUrl);
                    attachmentsList.AddRange(rootAttachmentsObject.Results);
                }

                return attachmentsList;

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while getting all attachments: " + ex.Message);
                return attachmentsList;
            }

        }

        // Print attachments to the console 
        private void PrintAttachments(List<Attachment> attachments, TextWriter writer)
        {
            try 
            {
                writer.WriteLine("Attachments Count: " + attachments.Count);
                for (int i = 0; i < attachments.Count; i++)
                {
                    Attachment attachment = attachments[i];
                    writer.WriteLine("    Attachment Title:            " + attachment.AttachmentContent.Title);
                    writer.WriteLine("    Attachment Id:               " + attachment.AttachmentContent.Id);
                    writer.WriteLine("    Attachment Type:             " + attachment.AttachmentContent.Extensions.mediaType);
                    writer.WriteLine("    Attachment Type Description: " + attachment.AttachmentContent.Extensions.MediaTypeDescription);
                    writer.WriteLine("    Attachment Size:             " + FormatFileSize(attachment.AttachmentContent.Extensions.FileSize));
                    writer.WriteLine("    Download Link:               " + attachment.AttachmentContent._ContentLinks.Download);
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while printing attachmetns: " + ex.Message);
            }
        }

        // Get Attachments using the REST API
        internal static RootAttachmentsObject GetAttachments(ConfluenceOptions.ListAttachmentsOptions options, string url)
        {
            RootAttachmentsObject attachments = new RootAttachmentsObject();
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                attachments = webRequestHandler.GetJson<RootAttachmentsObject>(url, options.Cookie);
                return attachments;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while Getting attachments: " + ex.Message);
                return attachments;
            }
            

        }

        // Format file size to human-readable format
        private static string FormatFileSize(int fileSize)
        {
            try
            {
                string[] sizes = { "b", "Kb", "Mb", "Gb", "Tb" };
                int i = 0;
                long numFileSize = fileSize;

                while (numFileSize >= 1024 && i < sizes.Length - 1)
                {
                    numFileSize /= 1024;
                    i++;
                }

                string sFileSize = numFileSize.ToString() + " " + sizes[i];
                return sFileSize;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while formatting file size: " + ex.Message);
                return string.Empty;
            }

        }
    }


    // Definition of attachment object
    internal class RootAttachmentsObject
    {
        [JsonProperty("results")]
        internal List<Attachment> Results { get; set; }

        [JsonProperty("totalSize")]
        internal int TotalSize { get; set; }

        [JsonProperty("_links")]
        internal _Links _Links { get; set; }
    }

    internal class Attachment
    {
        [JsonProperty("content")]
        internal AttachmentContent AttachmentContent { get; set; }

        [JsonProperty("excerpt")]
        internal string Excerpt { get; set; }

        [JsonProperty("lastModified")]
        internal string LastModified { get; set; }

        [JsonProperty("friendlyLastModified")]
        internal string FriendlyLastModified { get; set; }
    }

    internal class AttachmentContent
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("title")]
        internal string Title { get; set; }

        [JsonProperty("extensions")]
        internal Extensions Extensions { get; set; }

        [JsonProperty("_links")]
        internal _ContentLinks _ContentLinks { get; set; }

    }

    internal class Extensions
    {
        [JsonProperty("mediaType")]
        internal string mediaType { get; set; }

        [JsonProperty("fileSize")]
        internal int FileSize { get; set; }

        [JsonProperty("mediaTypeDescription")]
        internal string MediaTypeDescription { get; set; }


    }

    internal class _ContentLinks
    {
        [JsonProperty("download")]
        internal string Download { get; set; }
    }
}

