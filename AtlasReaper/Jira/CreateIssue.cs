using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AtlasReaper.Jira
{
    class CreateIssue
    {
        internal void CreateIssueM(Options.JiraOptions.CreateIssueOptions options)
        {
            string createMetaUrl = options.Url + "/rest/api/3/issue/createmeta?projectKeys=" + options.Project;
            string postIssueUrl = options.Url + "/rest/api/3/issue";

            string linkText = "\u200b" + options.Text;

            Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

            IssueCreateMetaData createMetaData = webRequestHandler.GetJson<IssueCreateMetaData>(createMetaUrl, options.Cookie);

            IssueObj issue = new IssueObj
            {
                IssueFields = new IssueFields
                {
                    ProjectKey = new ProjectKey
                    {
                        Key = options.Project
                    },

                    Summary = options.Summary,
                    IssueType = new IssueType
                    {
                        Name = options.IssueType
                    },
                    Description = new Body
                    {
                        Type = "doc",
                        Version = 1,
                        ContentList = new List<Content>()

                    }

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
                issue.IssueFields.Description.ContentList.Add(textParagraph);
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
                issue.IssueFields.Description.ContentList.Add(linkParagraph);
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(issue, Formatting.None, settings);

            IssueCreated issueCreated = webRequestHandler.PostJson<IssueCreated>(postIssueUrl, options.Cookie, json);
            Console.WriteLine("Created issue : " + issueCreated.Key);

            string restUrl = options.Url +  "/rest/api/3/search?jql=Issue=" + issueCreated.Key + "&expand=renderedFields&fields=description,summary,created,updated,status,creator,assignee,comment,attachment";
            
            Issues issueClass = new Issues();
            RootIssuesObject issuesList = issueClass.GetIssues(restUrl, options.Cookie);
            issueClass.PrintIssues(issuesList.Issues, Console.Out);
        }
    }

    internal class IssueCreated
    {
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("key")]
        internal string Key { get; set; }
        [JsonProperty("self")]
        internal string Self { get; set; }
    }

    internal class IssueCreateMetaData
    {
        [JsonProperty("expand")]
        internal string Expand { get; set; }

        [JsonProperty("projects")]
        internal List<Project> Projects { get; set; }
    }

    internal class IssueObj
    {
        [JsonProperty("fields")]
        internal IssueFields IssueFields { get; set; }
    }
    
    internal class IssueFields
    {
        [JsonProperty("project")]
        internal ProjectKey ProjectKey { get; set; }

        [JsonProperty("summary")]
        internal string Summary { get; set; }

        [JsonProperty("issuetype")]
        internal IssueType IssueType { get; set; }

        [JsonProperty("description")]
        internal Body Description { get; set; }
    }

    internal class ProjectKey
    {
        [JsonProperty("key")]
        internal string Key { get; set; }
    }
}
