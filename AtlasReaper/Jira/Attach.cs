using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasReaper.Jira
{
    class Attach
    {
        internal void AttachFile(JiraOptions.AttachOptions options) 
        {
            try
            {
                string url = options.Url + "/rest/api/3/issue/" + options.Issue + "/attachments";
                string fileName = options.Name;


                if (options.File != null)
                {
                    if (options.Issue == null)
                    {
                        Console.WriteLine("Please specify an issue with -i/--issue");
                        return;
                    }
                    if (options.Name == null)
                    {
                        fileName = Path.GetFileName(options.File);
                    }

                    List<Attachment> attachmentList = UploadFile(url, options, fileName);
                    if (attachmentList.Count < 1)
                    {
                        Console.WriteLine("Attachment already exists with the name " + fileName);
                        Console.WriteLine();
                        Console.WriteLine("    Use -a/--attachment to specify an existing attachment");
                        return;
                    }

                    Console.WriteLine("Uploaded " + fileName);
                    Console.WriteLine("Attachment Id: " + attachmentList[0].Id);

                    //AttachPage(attachmentObject.Results[0].Title, options);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while attaching files: " + ex.Message);
            }

        }

        private List<Attachment> UploadFile(string url, JiraOptions.AttachOptions options, string fileName)
        {
            FormData formData = new FormData
            {
                //comment = options.Comment,
                file = File.ReadAllBytes(options.File)
            };
            Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
            List<Attachment> attachmentList = webRequestHandler.PostForm<List<Attachment>>(url, options.Cookie, formData, fileName);

            return attachmentList;
        }

        internal class FormData
        {
            //public string comment { get; set; }
            public byte[] file { get; set; }
        }
    }
}
