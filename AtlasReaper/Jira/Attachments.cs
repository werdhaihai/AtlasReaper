using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtlasReaper.Jira
{
    internal class Attachments
    {
        
        // GET /rest/api/3/search?jql=attachments+IS+NOT+EMPTY&fields=attachment

        // GET "/rest/api/3/search?jql=project+=+" + options.ProjectId + "AND+attachments+IS+NOT+EMPTY&fields=attachment"
        internal void ListAttachments(JiraOptions.ListAttachmentsOptions options)
        {
            try
            {
                List<Issue> issues = new List<Issue>();
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
                // Building the url
                string restUrl = "/rest/api/3/search?jql=";
                string url = options.Url + restUrl;

                // GET all projects
                if (options.AllProjects)
                {
                    url += "attachments+IS+NOT+EMPTY&fields=attachment,summary,status";

                    // GET all issues for all projects
                    if (options.All)
                    {
                        url += "&maxResults=" + "100";
                        int startAt = 0;
                        RootIssuesObject issuesList = GetIssues(url, options);

                        issues.AddRange(issuesList.Issues);

                        while (issues.Count < issuesList.Total)
                        {
                            startAt += 100;
                            string nextUrl = url + "&startAt=" + startAt.ToString();
                            RootIssuesObject issuesListNext = GetIssues(nextUrl, options);
                            issues.AddRange(issuesListNext.Issues);
                        }
                    }
                    // GET issues for all projects by limit
                    else if (options.Limit != null)
                    {
                        url += "&maxResult=" + options.Limit;

                        RootIssuesObject issuesList = GetIssues(url, options);

                        issues.AddRange(issuesList.Issues);
                    }
                }
                // GET issues with attachments for specfic project
                else if (options.Project != null)
                {
                    url += "Project+=+" + options.Project + "+AND+attachments+IS+NOT+EMPTY&fields=attachment,summary,status";

                    // GET all issues with attachments for specific project
                    if (options.All)
                    {
                        url += "&maxResults=" + "100";
                        int startAt = 0;
                        RootIssuesObject issuesList = GetIssues(url, options);

                        issues.AddRange(issuesList.Issues);

                        while (issues.Count < issuesList.Total)
                        {
                            startAt += 100;
                            string nextUrl = url + "&startAt=" + startAt.ToString();
                            RootIssuesObject issuesListNext = GetIssues(nextUrl, options);
                            issues.AddRange(issuesListNext.Issues);
                        }
                    }

                    // GET issues with attachmetns for a specific project by limit
                    else if (options.Limit != null)
                    {
                        url += "&maxResult=" + options.Limit;

                        RootIssuesObject issuesList = GetIssues(url, options);

                        issues.AddRange(issuesList.Issues);
                    }
                }
                // Implies all issues with attachments in all projects
                else if (options.All)
                {
                    url += "attachments+IS+NOT+EMPTY&fields=attachment,summary,status";

                    // GET all issues for all projects
                    if (options.All)
                    {
                        url += "&maxResults=" + "100";
                        int startAt = 0;
                        RootIssuesObject issuesList = GetIssues(url, options);

                        issues.AddRange(issuesList.Issues);

                        while (issues.Count < issuesList.Total)
                        {
                            startAt += 100;
                            string nextUrl = url + "&startAt=" + startAt.ToString();
                            RootIssuesObject issuesListNext = GetIssues(nextUrl, options);
                            issues.AddRange(issuesListNext.Issues);
                        }
                    }
                }

                issues = FilterAttachments(issues, options);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while listing attachments: " + ex.Message);
            }

        }

        private List<Issue> FilterAttachments(List<Issue> issues, JiraOptions.ListAttachmentsOptions options)
        {
            try
            {
                // Exclude
                if (options.Exclude != null)
                {
                    List<string> excludeList = options.Exclude.Split(',').ToList();

                    foreach (Issue issue in issues)
                    {
                        List<Attachment> attachmentsToRemove = new List<Attachment>();

                        foreach (Attachment attachment in issue.Fields.Attachments)
                        {
                            string extension = Path.GetExtension(attachment.FileName).TrimStart('.');

                            if (excludeList.Contains(extension))
                            {
                                attachmentsToRemove.Add(attachment);
                            }
                        }

                        foreach (Attachment attachment in attachmentsToRemove)
                        {
                            issue.Fields.Attachments.Remove(attachment);
                        }
                    }
                }

                // Include
                if (options.Include != null)
                {
                    List<string> includeList = options.Include.Split(',').ToList();

                    foreach (Issue issue in issues)
                    {
                        // Create a separate list to store the attachments we want to remove
                        List<Attachment> attachmentsToRemove = new List<Attachment>();

                        foreach (Attachment attachment in issue.Fields.Attachments)
                        {
                            string extension = Path.GetExtension(attachment.FileName).TrimStart('.');

                            if (!includeList.Contains(extension))
                            {
                                attachmentsToRemove.Add(attachment);
                            }
                        }

                        // Remove the attachments not in include list
                        foreach (Attachment attachment in attachmentsToRemove)
                        {
                            issue.Fields.Attachments.Remove(attachment);
                        }
                    }
                }
                // Remove any issues with no attachments after filtering
                issues = issues.Where(issue => issue.Fields.Attachments.Any()).ToList();
                return issues;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while filtering attachments: " + ex.Message);
                return issues;
            }
        }


        private RootIssuesObject GetIssues(string url, JiraOptions.ListAttachmentsOptions options)
        {
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                RootIssuesObject issuesList = webRequestHandler.GetJson<RootIssuesObject>(url, options.Cookie);

                return issuesList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting issues: " + ex.Message);
                return null;
            }
        }

        private void PrintIssues(List<Issue> issues, TextWriter writer)
        {
            try
            {
                for (int i = 0; i < issues.Count; i++)
                {
                    Issue issue = issues[i];
                    List<Attachment> attachments = issue.Fields.Attachments;

                    writer.WriteLine("  Issue Title    : " + issue.Fields.Title);
                    writer.WriteLine("  Issue Key      : " + issue.Key);
                    writer.WriteLine("  Issue Id       : " + issue.Id);
                    writer.WriteLine("  Status         : " + issue.Fields.Status.Name);
                    if (attachments.Count > 0)
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while printing issues: " + ex.Message);
            }
        }
    }
}
