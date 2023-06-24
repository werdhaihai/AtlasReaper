using System;
using CommandLine;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Text;
using AtlasReaper.Options;
using AtlasReaper.Confluence;
using AtlasReaper.Jira;


namespace AtlasReaper
{
    internal class ArgHandler
    {
        internal void HandleArgs(string[] args)
        {

            if (args.Length == 0)
            {
                // If no arguments provided, print help and return
                PrintHelp();
                return;
            }

            // Manual parse of first verb
            switch (args[0])
            {
                case "confluence":
                    Console.WriteLine(logo);
                    ParseConfluence(args);
                    break;
                case "jira":
                    Console.WriteLine(logo);
                    ParseJira(args);
                    break;
                default:
                    Console.WriteLine($"Unrecognized command: {args[0]}");
                    PrintHelp();
                    break;
            }

        }

        // Parse arguments for Confluence commands
        internal void ParseConfluence(string[] args)
        {
            Parser parser = new Parser();
            Parser.Default.ParseArguments<
                ConfluenceOptions.AttachOptions,
                ConfluenceOptions.DownloadOptions,
                ConfluenceOptions.EmbedOptions,
                ConfluenceOptions.LinkOptions,
                ConfluenceOptions.ListAttachmentsOptions,
                ConfluenceOptions.ListPagesOptions,
                ConfluenceOptions.ListSpacesOptions,
                ConfluenceOptions.SearchOptions
                
            >(args.Skip(1))
            .WithParsed<ConfluenceOptions.AttachOptions>(opts =>
            {
                try
                {

                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Confluence.Attach attach = new Confluence.Attach();
                    attach.AttachFile(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.DownloadOptions>(opts =>
            {
                try
                {

                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Confluence.Download download = new Confluence.Download();
                    download.DownloadAttachments(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.EmbedOptions>(opts =>
            {
                try
                {

                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Embed embed = new Embed();
                    embed.EmbedIframe(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.LinkOptions>(opts =>
            {
                try
                {

                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Link link = new Link();
                    link.AddLink(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.ListSpacesOptions>(opts =>
            {
                try
                {

                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Spaces listSpaces = new Spaces();
                    listSpaces.ListSpaces(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.ListPagesOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Pages listPages = new Pages();
                    listPages.ListPages(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<ConfluenceOptions.ListAttachmentsOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Confluence.Attachments attachments = new Confluence.Attachments();
                    attachments.ListAttachments(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
            })
            .WithParsed<ConfluenceOptions.SearchOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Confluence.Search search = new Confluence.Search();
                    search.SearchConfluence(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
            })
            .WithNotParsed(HandleParseErrors);
        }

        // Parse arguments for Jira commands
        private void ParseJira(string[] args)
        {
            
            Parser.Default.ParseArguments<
                JiraOptions.AddCommentOptions,
                JiraOptions.AttachOptions,
                JiraOptions.CreateIssueOptions,
                JiraOptions.DownloadOptions,
                JiraOptions.ListAttachmentsOptions,
                JiraOptions.ListIssuesOptions,
                JiraOptions.ListProjectsOptions,
                JiraOptions.ListUsersOptions,
                JiraOptions.SearchIssuesOptions
            >(args.Skip(1))
            .WithParsed<JiraOptions.AddCommentOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.AddComment addComment = new Jira.AddComment();
                    addComment.CommentAdd(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.AttachOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.Attach attach = new Jira.Attach();
                    attach.AttachFile(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.CreateIssueOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.CreateIssue createIssue = new Jira.CreateIssue();
                    createIssue.CreateIssueM(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.DownloadOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.Download download = new Jira.Download();
                    download.DownloadAttachments(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.ListAttachmentsOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.Attachments attachments = new Jira.Attachments();
                    attachments.ListAttachments(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.SearchIssuesOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Jira.Search search = new Jira.Search();
                    search.SearchJira(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }

            })
            .WithParsed<JiraOptions.ListUsersOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Users users = new Users();
                    users.ListUsers(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
            })
            .WithParsed<JiraOptions.ListIssuesOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Issues issues = new Issues();
                    issues.ListIssues(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
            })
            .WithParsed<JiraOptions.ListProjectsOptions>(opts =>
            {
                try
                {
                    Auth.CheckAuth(opts.Url, opts.Cookie);
                    Projects projects = new Projects();
                    projects.ListProjects(opts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return;
                }
            })
            // Handle errors during argument parsing
            .WithNotParsed(HandleParseErrors);

        }

        internal void HandleParseErrors(IEnumerable<Error> errs)
        {
            // Don't need to actually do anything, CommandLineParser already prints error.
            return;
        }

        // Print Help information
        internal void PrintHelp()
        {
            Console.WriteLine(logo);
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine();
            Console.WriteLine("    confluence       - query confluence");
            Console.WriteLine("    jira             - query jira");
            Console.WriteLine();
            return;
        }

        public static string logo = @"
                                                   .@@@@             
                                               @@@@@                 
                                            @@@@@   @@@@@@@          
                                          @@@@@   @@@@@@@@@@@        
                                         @@@@@  @@@@@@@@@@@@@@@      
                                        @@@@,  @@@@        *@@@@     
                                          @@@@ @@@  @@  @@@ .@@@     
   _  _   _         ___                       @@@@@@@     @@@@@@     
  /_\| |_| |__ _ __| _ \___ __ _ _ __  ___ _ _   @@   @@@@@@@@       
 / _ \  _| / _` (_-<   / -_) _` | '_ \/ -_) '_|  @@   @@@@@@@@       
/_/ \_\__|_\__,_/__/_|_\___\__,_| .__/\___|_|    @@@@@@@@   &@       
                                |_|             @@@@@@@@@@  @@&      
                                                @@@@@@@@@@@@@@@@@    
                                               @@@@@@@@@@@@@@@@. @@
                                                  @werdhaihai ";
    }
}