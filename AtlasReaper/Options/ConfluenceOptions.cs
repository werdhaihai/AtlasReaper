using CommandLine;
using AtlasReaper.Utils;
using System;
using System.IO;

namespace AtlasReaper.Options
{
    internal class ConfluenceOptions
    {

        private string _status;

        // Shared options for Confluence commands

        [Option('u', "url", Required = true, HelpText = "Confluence URL")]
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
                    string fullPath;

                    if (Path.IsPathRooted(value))
                    {
                        fullPath = Path.GetFullPath(value);
                    }
                    else
                    {
                        string currentDirectory = Environment.CurrentDirectory;
                        fullPath = Path.Combine(currentDirectory, value);
                        Console.WriteLine(fullPath);
                    }
                    string directory = Path.GetDirectoryName(fullPath);
                    string fileName = Path.GetFileName(fullPath);
                    try
                    {
                        if (File.Exists(fullPath))
                        {
                            Console.WriteLine("File already exists. Please choose a different file name.");
                            return;
                        }

                        if (!Directory.Exists(directory))
                        {
                            Console.WriteLine("Invalid directory. Please specify a valid directory.");
                            return;
                        }

                        if (!FileUtils.CanWriteToDirectory(directory))
                        {
                            Console.WriteLine("Unable to write to the specified directory. Please choose a different location.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                outfile = value;
            }
        }

        // Attach a file
        [Verb("attach", HelpText = "Attach a file to a page")]
        internal class AttachOptions : ConfluenceOptions
        {
            [Option('a', "attachment", Required = false, HelpText = "Attachment Id to attach to page (if attachment is already created)")]
            public string AttachmentId { get; set; }

            [Option("at", Required = false, HelpText = "User id to @ on the page (get user id from the jira listusers command)")]
            public string At { get; set; }

            [Option("comment", Required = false, Default = "untitled", HelpText = "Comment for uploaded file")]
            public string Comment { get; set; }

            [Option('f', "file", Required = false, HelpText = "File to attach")]
            public string File { get; set; }

            [Option('n', "name", Required = false, HelpText = "Name of file attachment. (Defaults to filename passed with -f/--file")]
            public string Name { get; set; }

            [Option('p', "page", Required = true, HelpText = "Page to attach")]
            public string Page { get; set; }

            [Option('t', "text", Required = false, HelpText = "Text to add to page to provide context (e.g \"I uploaded this file, please take a look\")")]
            public string Text { get; set; }
        }

        // Embed command options
        [Verb("embed", HelpText = "Embed a 1x1 pixel image to perform farming attacks")]
        internal class EmbedOptions : ConfluenceOptions
        {
            [Option("at", Required = false, HelpText = "User id to @ on the page (get user id from the jira listusers command)")]
            public string At { get; set; }

            [Option('l', "link", Required = true, HelpText = "Url to listener")]
            public string Link { get; set; }

            [Option('m', "message", Required = false, HelpText = "Messgage to add to the page (i.e. I need you to take a look at this)")]
            public string Message { get; set; }

            [Option('p', "page", Required = true, HelpText = "Page to embed")]
            public string Page { get; set; }
        }

        // Download command options
        [Verb("download", HelpText = "Download Attachment")]
        internal class DownloadOptions : ConfluenceOptions
        {
            [Option('a', "attachments", Required = true, HelpText = "Comma-separated attachment ids to download (no spaces)")]
            public string Attachments { get; set; }

            [Option('o', "output-dir", Required = false, HelpText = "Directory to save the downloaded attachments")]
            public string OutputDir { get; set; }
        }

        // Embed command options
        [Verb("link", HelpText = "Add link to page")]
        internal class LinkOptions : ConfluenceOptions
        {
            [Option("at", Required = false, HelpText = "User id to @ on the page (get user id from the jira listusers command)")]
            public string At { get; set; }

            [Option('l', "link", Required = true, HelpText = "Url to link to")]
            public string Link { get; set; }

            [Option('m', "message", Required = false, HelpText = "Messgage to add to the page (i.e. I need you to take a look at this)")]
            public string Message { get; set; }

            [Option('p', "page", Required = true, HelpText = "Page to embed")]
            public string Page { get; set; }

            [Option('t', "text", Required = false, Default = "Here", HelpText = "Link text to display")]
            public string Text { get; set; }
        }

        //Listattachments command options
        [Verb("listattachments", HelpText = "List Attachments")]
        internal class ListAttachmentsOptions : ConfluenceOptions
        {
            [Option('a', "all", Required = false, Default = false, HelpText = "Return all attachments for supplied space")]
            public bool All { get; set; }

            [Option("all-spaces", Required = false, Default = false, HelpText = "Return attachments for all spaces. WARNING!! This can make a lot of requests!")]
            public bool AllSpaces { get; set; }

            [Option('i', "include", Required = false, HelpText = "Comma-separated list of extensions to include (e.g. png,jpeg)")]
            public string Include { get; set; }

            [Option('l', "limit", Required = false, Default = "200", HelpText = "Number or attachments to return")]
            public string Limit { get; set; }

            [Option('p', "page", Required = false, HelpText = "Page to return attachments for")]
            public string Page { get; set; }

            [Option('s', "space", Required = false, HelpText = "Space to return attachments for")]
            public string Space { get; set; }

            [Option('x', "exclude", Required = false, HelpText = "Comma-separated list of extensions to exclude (e.g. png,jpeg)")]
            public string Exclude { get; set; }
        }

        // Listpages command options
        [Verb("listpages", HelpText = "List pages")]
        internal class ListPagesOptions : ConfluenceOptions
        {

            [Option("all", Required = false, Default = false, HelpText = "Return all pages (Returns every Page if no Space is specified)")]
            public bool AllPages { get; set; }

            [Option('b', "body", Required = false, Default = false, HelpText = "Print body of pages")]
            public bool Body { get; set; }

            [Option('l', "limit", Required = false, Default = "250", HelpText = "Number of results to return")]
            public string Limit { get; set; }

            [Option('p', "page", Required = false, HelpText = "Page to return")]
            public string Page { get; set; }

            [Option('s', "space", Required = false, HelpText = "Space to search")]
            public string Space { get; set; }

            [Option("status", Required = false, HelpText = "Page Status (current, archived, deleted, trashed) Defaults to all")]
            public string Status
            {
                get => _status;
                set
                {
                    if (IsValidStatus(value))
                    {
                        _status = value;
                    }
                    else
                    {
                        Console.WriteLine("Invalid status value. Please use one of the following values: current, archived, deleted, or trashed.");
                        Console.WriteLine();
                        return;
                    }
                }
            }

        }

        // Check if status value is valid
        private bool IsValidStatus(string value)
        {
            string[] validStatuses = new[] { "current", "archived", "deleted", "trashed" };
            return Array.Exists(validStatuses, s => s.Equals(_status, StringComparison.OrdinalIgnoreCase));
        }

        // Listspaces command options
        [Verb("listspaces", HelpText = "List spaces")]
        internal class ListSpacesOptions : ConfluenceOptions
        {

            [Option("all", Required = false, Default = false, HelpText = "Returns all spaces")]
            public bool AllSpaces { get; set; }

            [Option('l', "limit", Required = false, Default = "100", HelpText = "Number of results to return")]
            public string Limit { get; set; }

            [Option('s', "space", Required = false, HelpText = "Space to search")]
            public string Space { get; set; }

            [Option('t', "type", Required = false, HelpText = "Space type to return (global, personal, ...?)")]
            public string Type { get; set; }

        }
        
        // Search command options
        [Verb("search", HelpText = "Search Confluence")]
        internal class SearchOptions : ConfluenceOptions
        {
            [Option('a', "all", Required = false, Default = false, HelpText = "Return all matches")]
            public bool All { get; set; }

            [Option('l', "limit", Required = false, Default = "250", HelpText = "Number of results to return")]
            public string Limit { get; set; }

            [Option('q', "query", Required = true, HelpText = "String or phrase to query")]
            public string Query { get; set; }

        }
    }
}
