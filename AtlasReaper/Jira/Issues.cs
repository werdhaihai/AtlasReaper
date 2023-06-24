using Newtonsoft.Json;
using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AtlasReaper.Jira
{
    class Issues
    {
        internal void ListIssues(JiraOptions.ListIssuesOptions options)
        {
            try
            {
                // Building the url
                string restUrl = "/rest/api/3/search?jql=";
                string url = options.Url + restUrl;
                if (options.Project != null)
                {
                    url = url + "Project=" + options.Project + "&";
                }
                if (options.Issue != null)
                {
                    url = url + "Issue=" + options.Issue + "&";
                }
                url = url + "&expand=renderedFields&fields=description,summary,created,updated,status,creator,assignee";

                if (options.Comments)
                {
                    url = url + ",comment";
                }
                if (options.Attachments)
                {
                    url = url + ",attachment";
                }

                // Return all Issues
                if (options.All)
                {
                    url = url + "&maxResults=" + "100";
                    int startAt = 0;
                    List<Issue> issues = new List<Issue>();
                    RootIssuesObject issuesList = GetIssues(url, options.Cookie);

                    /*                if (options.Limit != issuesList.Total.ToString())
                                    {
                                        int numRequests = issuesList.Total / 100 + 1;
                                        Console.WriteLine("This will generate " + numRequests.ToString() + " web requests, potentially resulting in a large quantity of data returned.");
                                        Console.WriteLine("If you want to continue, please use the -l/--limit flag with the total number of issues: " + issuesList.Total.ToString());
                                        return;
                                    }*/

                    issues.AddRange(issuesList.Issues);


                    while (issues.Count < issuesList.Total)
                    {
                        startAt += 100;
                        string nextUrl = url + "&startAt=" + startAt.ToString();
                        RootIssuesObject issuesListNext = GetIssues(nextUrl, options.Cookie);
                        issues.AddRange(issuesListNext.Issues);
                    }

                    if (options.outfile != null)
                    {
                        using (StreamWriter writer = new StreamWriter(options.outfile))
                        {
                            PrintIssues(issues, writer);
                        }
                    }
                    else
                    {
                        PrintIssues(issues, Console.Out);
                    }


                }
                // Return issues by limit
                else
                {
                    url = url + "&maxResults=" + options.Limit;

                    RootIssuesObject issuesList = GetIssues(url, options.Cookie);

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
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while listing Jira issues: " + ex.Message);
            }

        }

        internal RootIssuesObject GetIssues(string url, string cookie)
        {
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                RootIssuesObject issuesList = webRequestHandler.GetJson<RootIssuesObject>(url, cookie);

                return issuesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting issues: " + ex.Message);
                return null;
            }
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

    internal class RootIssuesObject
    {
        [JsonProperty("startAt")]
        internal int StartAt { get; set; }

        [JsonProperty("total")]
        internal int Total { get; set; }

        [JsonProperty("issues")]
        internal List<Issue> Issues { get; set; }
    }

    internal class Issue
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("key")]
        internal string Key { get; set; }

        [JsonProperty("renderedFields")]
        internal RenderedFields RenderedFields { get; set; }

        [JsonProperty("fields")]
        internal Fields Fields { get; set; }
    }

    internal class RenderedFields
    {
        [JsonProperty("created")]
        internal string Created { get; set; }

        [JsonProperty("updated")]
        internal string Updated { get; set; }

        [JsonProperty("description")]
        internal string Description { get; set; }

        [JsonProperty("comment")]
        internal RenderedCommentObj RenderedCommentObj { get; set; }

        [JsonProperty("attachment")]
        internal List<Attachment> Attachments { get; set; }

    }

    internal class Fields
    {
        [JsonProperty("assignee")]
        internal Author Assignee { get; set; }

        [JsonProperty("creator")]
        internal Author Creator { get; set; }

        [JsonProperty("status")]
        internal Status Status { get; set; }

        [JsonProperty("summary")]
        internal string Title { get; set; }

        [JsonProperty("attachment")]
        internal List<Attachment> Attachments { get; set; }


    }

    internal class Author
    {
        [JsonProperty("emailAddress")]
        internal string EmailAddress { get; set; }

        [JsonProperty("displayName")]
        internal string DisplayName { get; set; }

        [JsonProperty("timeZone")]
        internal string TimeZone { get; set; }

    }

    internal class RenderedCommentObj
    {
        [JsonProperty("comments")]
        internal List<Comment> Comments { get; set; }
    }

    internal class Comment
    {
        [JsonProperty("author")]
        internal Author Author { get; set; }

        [JsonProperty("body")]
        internal Body Body { get; set; }

        [JsonProperty("created")]
        internal string Created { get; set; }

    }

    internal class Body
    {
        [JsonProperty("content")]
        internal List<Content> ContentList { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("version")]
        internal int Version { get; set; }
    }

    internal class Content
    {
        [JsonProperty("content")]
        internal List<CommentContent> CommentContents { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("attrs")]
        internal Attrs Attrs { get; set; }
    }

    internal class CommentContent
    {
        [JsonProperty("text")]
        internal string Text { get; set; }

        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("attrs")]
        internal Attrs Attrs { get; set; }

        [JsonProperty("marks")]
        internal List<Mark> Marks { get; set; }
    }

    internal class Mark
    {
        [JsonProperty("type")]
        internal string Type { get; set; }

        [JsonProperty("attrs")]
        internal Attrs Attrs { get; set; }
    }

    internal class Attrs
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("text")]
        internal string Text { get; set; }

        [JsonProperty("accessLevel")]
        internal string AccessLevel { get; set; }

        [JsonProperty("href")]
        internal string Href { get; set; }

        [JsonProperty("url")]
        internal string Url { get; set; }
    }

    internal class Attachment
    {
        [JsonProperty("filename")]
        internal string FileName { get; set; }

        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("author")]
        internal Author Author { get; set; }

        [JsonProperty("created")]
        internal string Created { get; set; }

        [JsonProperty("size")]
        internal string Size { get; set; }

        [JsonProperty("mimeType")]
        internal string mimeType { get; set; }

        [JsonProperty("content")]
        internal string Content { get; set; }
    }

    internal class Status
    {
        [JsonProperty("name")]
        internal string Name { get; set; }
    }

}
