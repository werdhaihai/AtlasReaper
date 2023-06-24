using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace AtlasReaper.Jira
{
    class Search
    {
        internal void SearchJira(JiraOptions.SearchIssuesOptions options)
        {
            try
            {
                RootIssuesObject issuesList = new RootIssuesObject();

                // Building the url
                string query = WebUtility.UrlEncode(options.Query);
                string url = 
                    options.Url + 
                    "/rest/api/3/search?jql=text~" + 
                    query + 
                    "&expand=renderedFields&fields=description,summary,created,updated,status,creator,assignee";

                if (options.Comments)
                {
                    url = url + ",comment";
                }
                if (options.Attachments)
                {
                    url = url + ",attachment";
                }
                if (!options.All)
                {
                    issuesList = DoSearch(options, url);
                }
                else 
                {
                    // return all results
                }

                PrintIssues(issuesList.Issues, Console.Out);

                if (options.outfile != null)
                {
                    using (StreamWriter writer = new StreamWriter(options.outfile))
                    {
                        PrintIssues(issuesList.Issues, writer);
                    }
                }
                else
                {
                    PrintIssues(issuesList.Issues, Console.Out);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occurred while searching Jira: " + ex.Message);
            }
        }

        private RootIssuesObject DoSearch(JiraOptions.SearchIssuesOptions options, string url)
        {
            
            

            Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

            RootIssuesObject searchObject = webRequestHandler.GetJson<RootIssuesObject>(url, options.Cookie);

            return searchObject;

        }

        internal void PrintIssues(List<Issue> issues, TextWriter writer)
        {
            try
            {
                for (int i = 0; i < issues.Count; i++)
                {
                    Issue issue = issues[i];
                    List<Comment> comments = issue.RenderedFields.RenderedCommentObj?.Comments;
                    List<Attachment> attachments = issue.RenderedFields?.Attachments;

                    writer.WriteLine("  Issue Title    : " + issue.Fields.Title);
                    writer.WriteLine("  Issue Key      : " + issue.Key);
                    writer.WriteLine("  Issue Id       : " + issue.Id);
                    writer.WriteLine("  Created        : " + issue.RenderedFields.Created);
                    writer.WriteLine("  Updated        : " + issue.RenderedFields.Updated);
                    writer.WriteLine("  Status         : " + issue.Fields.Status?.Name);
                    writer.WriteLine("  Creator        : " + issue.Fields.Creator?.EmailAddress + " - " + issue.Fields.Creator?.DisplayName + " - " + issue.Fields.Creator?.TimeZone);
                    writer.WriteLine("  Assignee       : " + issue.Fields.Assignee?.EmailAddress + " - " + issue.Fields.Assignee?.DisplayName + " - " + issue.Fields.Assignee?.TimeZone);
                    writer.WriteLine("  Issue Contents : " + Regex.Replace(issue.RenderedFields.Description, @"<(?!\/?a(?=>|\s.*>))\/?.*?>", "").Trim('\r', '\n'));
                    writer.WriteLine();
                    if (attachments?.Count > 0)
                    {
                        writer.WriteLine("  Attachments    : ");
                        writer.WriteLine();
                        for (int j = 0; j < attachments.Count; j++)
                        {
                            Attachment attachment = attachments[j];
                            writer.WriteLine("    Filename      : " + attachment.FileName);
                            writer.WriteLine("    Attachment Id : " + attachment.Id);
                            writer.WriteLine("    Mimetype      : " + attachment.mimeType);
                            writer.WriteLine("    File size     : " + attachment.Size);
                            writer.WriteLine();
                        }
                    }
                    if (comments?.Count > 0)
                    {
                        writer.WriteLine();
                        writer.WriteLine("  Comments       : ");
                        writer.WriteLine();
                        for (int j = 0; j < comments.Count; j++)
                        {
                            writer.WriteLine("    - " + comments[j].Author.EmailAddress + " - " + comments[j].Author.DisplayName + " - " + comments[j].Created);
                            List<Content> contentList = comments[j]?.Body.ContentList;
                            for (int k = 0; k < contentList.Count; k++)
                            {

                                List<CommentContent> commentContents = contentList[k]?.CommentContents;
                                if (commentContents != null)
                                {
                                    for (int l = 0; l < commentContents.Count; l++)
                                    {
                                        writer.WriteLine("             " + commentContents[l].Text?.Trim('\r', '\n'));

                                    }
                                }
                            }
                            writer.WriteLine();
                        }

                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while printing issues: " + ex.Message);
            }

        }
    }

}
