using System;
using CommandLine;
using AtlasReaper.Utils;

namespace AtlasReaper.Options
{
    class JiraOptions
    {


        [Option('u', "url", Required = true, HelpText = "Jira URL")]
        public string Url { get; set; }

        [Option('c', "cookie", Required = false, Default = null, HelpText = "cloud.session.token")]
        public string Cookie { get; set; }

        internal string outfile;
        [Option('o', "output", Required = false, Default = null, HelpText = "Save output to file")]
        public string Outfile 
        { 
            get { return outfile; } 
            set
            {
                if (value != null)
                {
                    try
                    {
                        value = FileUtils.GetFileName(value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    } 
                }

                outfile = value;
            } 
        }

        [Verb("addcomment", HelpText = "Add a comment to an issue")]
        internal class AddCommentOptions : JiraOptions
        {
            [Option("at", Required = false, HelpText = "User id to @ on the comment (get user id from the jira listusers command)")]
            public string At { get; set; }

            [Option('l', "link", Required = true, HelpText = "Url to link to")]
            public string Link { get; set; }

            [Option('m', "message", Required = false, HelpText = "Message to add to the issue comment (i.e. I need you to take a look at this)")]
            public string Message { get; set; }

            [Option('i', "issue", Required = true, HelpText = "Issue name")]
            public string Issue { get; set; }

            [Option('t', "text", Required = false, Default = "Here", HelpText = "Link text to display")]
            public string Text { get; set; }
        }

        [Verb("attach", HelpText = "Attach a file to an issue")]
        internal class AttachOptions : JiraOptions
        {
            [Option('a', "attachment", Required = false, HelpText = "Attachment Id to attach to page (if attachment is already created)")]
            public string AttachmentId { get; set; }

            [Option("comment", Required = false, Default = "untitled", HelpText = "Comment for uploaded file")]
            public string Comment { get; set; }

            [Option('f', "file", Required = false, HelpText = "File to attach")]
            public string File { get; set; }

            [Option('i', "issue", Required = false, HelpText = "Issue to add attachment")]
            public string Issue { get; set; }

            [Option('n', "name", Required = false, HelpText = "Name of file attachment. (Defaults to filename passed with -f/--file")]
            public string Name { get; set; }

            [Option('t', "text", Required = false, HelpText = "Text to add to page to provide context (e.g \"I uploaded this file, please take a look\")")]
            public string Text { get; set; }
        }
        
        [Verb("createissue", HelpText = "Create an issue")]
        internal class CreateIssueOptions : JiraOptions
        {
            [Option("at", Required = false, HelpText = "User id to @ on the comment (get user id from the jira listusers command)")]
            public string At { get; set; }

            [Option('i', "issue-type", Required = true, HelpText = "Issue type to create")]
            public string IssueType { get; set; }

            [Option('l', "link", Required = false, HelpText = "Url to link to")]
            public string Link { get; set; }

            [Option('m', "message", Required = false, HelpText = "Message to add to the issue (i.e. I need you to take a look at this)")]
            public string Message { get; set; }

            [Option('p', "project", Required = true, HelpText = "Project to create issue for")]
            public string Project { get; set; }

            [Option('s', "summary", Required = false, Default = "Looking for Solutions", HelpText = "Issue summary (title)")]
            public string Summary { get; set; }

            [Option('t', "text", Required = false, Default = "Here", HelpText = "Link text to display")]
            public string Text { get; set; }
        }

        [Verb("download", HelpText = "Download attachment(s)")]
        internal class DownloadOptions : JiraOptions
        {
            [Option('a', "attachments", Required = true, HelpText = "Comma-separated attachment ids to download (no spaces)")]
            public string Attachments { get; set; }

            [Option('o', "output-dir", Required = false, HelpText = "Directory to save downloads to")]
            public string OutputDir { get; set; }
        }

        [Verb("search", HelpText = "Search issues")]
        internal class SearchIssuesOptions : JiraOptions
        {
            [Option('a', "all", Required = false, Default = false, HelpText = "Return all matches")]
            public bool All { get; set; }

            [Option("attachments", Required = false, Default = false, HelpText = "Include attachments")]
            public bool Attachments { get; set; }

            [Option("comments", Required = false, Default = false, HelpText = "Include Comments")]
            public bool Comments { get; set; }

            [Option('l', "limit", Required = false, Default = "100", HelpText = "Number of results to return")]
            public string Limit { get; set; }

            [Option('q', "query", Required = true, HelpText = "String or phrase to query")]
            public string Query { get; set; }
        }

        //Listattachments command options
        [Verb("listattachments", HelpText = "List Attachments")]
        internal class ListAttachmentsOptions : ConfluenceOptions
        {
            [Option('a', "all", Required = false, Default = false, HelpText = "Return all attachments for supplied project id")]
            public bool All { get; set; }

            [Option("all-projects", Required = false, Default = false, HelpText = "Return attachments for all projects. WARNING!! This can make a lot of requests!")]
            public bool AllProjects { get; set; }

            [Option('i', "include", Required = false, HelpText = "Comma-separated list of extensions to include (e.g. png,jpeg)")]
            public string Include { get; set; }

            [Option('l', "limit", Required = false, Default = "100", HelpText = "Number or attachments to return")]
            public string Limit { get; set; }

            [Option('p', "project", Required = false, HelpText = "Project to return attachments for")]
            public string Project { get; set; }

            [Option('x', "exclude", Required = false, HelpText = "Comma-separated list of extensions to exclude (e.g. png,jpeg)")]
            public string Exclude { get; set; }
        }

        [Verb("listissues", HelpText = "List Issues")]
        internal class ListIssuesOptions : JiraOptions
        {

            [Option('a', "all", Required = false, Default = false, HelpText = "Return all matches")]
            public bool All { get; set; }

            [Option("attachments", Required = false, Default = false, HelpText = "Include attachments")]
            public bool Attachments { get; set; }

            [Option("comments", Required = false, Default = false, HelpText = "Include Comments")]
            public bool Comments { get; set; }

            [Option('i', "issue", Required = false, HelpText = "Issue to list")]
            public string Issue { get; set; }

            [Option('p', "project", Required = false, HelpText = "Project Key or Id to list issues from")]
            public string Project { get; set; }

            [Option('l', "limit", Required = false, Default = "100", HelpText = "Number of results to return")]
            public string Limit { get; set; }

        }

        [Verb("listprojects", HelpText = "List Jira Projects")]
        internal class ListProjectsOptions : JiraOptions
        {

            [Option('a', "all", Required = false, Default = false, HelpText = "Return all matches")]
            public bool All { get; set; }

            // Projects API only returns 50 for some reason
            [Option('l', "limit", Required = false, Default = "50", HelpText = "Number of results to return")]
            public string Limit { get; set; }

            internal string sortBy;

            [Option('s', "sortby", Required = false, Default = "issues", HelpText = "Sort By \"issues\" for total number of issues or \"updated\" for most recently updated issues")]
            public string SortBy
            {
                get => sortBy;
                set
                {
                    if (value != "issues" && value != "updated")
                    {
                        throw new Exception("Invalid sort option. Use \"issues\" or \"updated\"");
                    }
                    sortBy = value;
                }
            }
        }

        [Verb("listusers", HelpText = "List Atlassian users")]
        internal class ListUsersOptions : JiraOptions
        {
            [Option('f', "full", Required = false, Default = false, HelpText = "Return display name and email")]
            public bool Full { get; set; }
        }


    }
}
