using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtlasReaper.Jira
{
    internal class AddComment
    {

        internal void CommentAdd(Options.JiraOptions.AddCommentOptions options)
        {
            try
            {
                string linkText = options.Text;
                string commentUrl = options.Url + "/rest/api/3/issue/" + options.Issue + "/comment";


                Root root = new Root
                {
                    Body = new Body
                    {
                        Version = 1,
                        Type = "doc",
                        ContentList = new List<Content>()
                    }
                };


                Content textParagraph = new Content
                {
                    Type = "paragraph",
                    CommentContents = new List<CommentContent>()
                };

                if (options.At != null)
                {
                    CommentContent mention = new CommentContent
                    {
                        Type = "mention",
                        Attrs = new Attrs
                        {
                            Id = options.At,
                            AccessLevel = ""
                        }
                    };
                    textParagraph.CommentContents.Add(mention);
                }

                if (options.Message != null)
                {
                    CommentContent commentMessage = new CommentContent
                    {
                        Type = "text",
                        Text = " " + options.Message
                    };

                    textParagraph.CommentContents.Add(commentMessage);
                }

                if (textParagraph.CommentContents.Count > 0)
                {
                    root.Body.ContentList.Add(textParagraph);
                }



                Content linkParagraph = new Content
                {
                    Type = "paragraph",
                    CommentContents = new List<CommentContent>()
                };

                if (options.Link != null)
                {
                    CommentContent linkContent = new CommentContent
                    {
                        Type = "text",
                        Text = linkText,
                        Marks = new List<Mark>
                {
                    new Mark
                    {
                        Type = "link",
                        Attrs = new Attrs
                        {
                            Href = options.Link
                        }
                    }
                }
                    };

                    linkParagraph.CommentContents.Add(linkContent);
                    root.Body.ContentList.Add(linkParagraph);
                }



                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                string json = JsonConvert.SerializeObject(root, Formatting.None, settings);
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                webRequestHandler.PostJson<Root>(commentUrl, options.Cookie, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred adding comment: " + ex.Message);
            }
            
        } 
    }

    internal class Root
    {
        [JsonProperty("body")]
        internal Body Body { get; set; }
    }
}

