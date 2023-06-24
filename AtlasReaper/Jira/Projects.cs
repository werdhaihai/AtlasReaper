using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using AtlasReaper.Options;

namespace AtlasReaper.Jira
{
    class Projects
    {
        // List projects based on the provided options
        internal void ListProjects(JiraOptions.ListProjectsOptions options)
        {
            try
            {
                List<Project> projects = new List<Project>();

                if (options.Limit != "50" && !options.All)
                {
                    // Build the URL for listing projects based on limit
                    string restUrl = "/rest/api/3/project/search?expand=description,insight,issueTypes&maxResults=";
                    string url = options.Url + restUrl + options.Limit;
                    RootProjectsObject projectsList = GetProjects(options, url);

                    projects = SortProjects(options.sortBy, projectsList.Projects);
                }
                else if (options.All)
                {
                    // List all projects
                    string restUrl = "/rest/api/3/project/search?expand=description,insight,issueTypes";
                    string url = options.Url + restUrl;

                    RootProjectsObject projectsList = GetProjects(options, url);
                    projects.AddRange(projectsList.Projects);

                    while (!projectsList.IsLast)
                    {
                        projectsList = GetProjects(options, projectsList.NextPage);
                        projects.AddRange(projectsList.Projects);
                    }

                    projects = SortProjects(options.sortBy, projects);



                }
                else
                {
                    // List projects 
                    string restUrl = "/rest/api/3/project/search?expand=description,insight,issueTypes";
                    string url = options.Url + restUrl;
                    RootProjectsObject projectsList = GetProjects(options, url);
                    projects = SortProjects(options.sortBy, projectsList.Projects);
                }

                if (options.outfile != null)
                {
                    using (StreamWriter writer = new StreamWriter(options.outfile))
                    {
                        PrintProjects(projects, writer);
                    }
                }
                else
                {
                    PrintProjects(projects, Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while listing projects: " + ex.Message);
            }
            
        }

        // Sort projects based on the provided sort option
        private List<Project> SortProjects(string sortBy, List<Project> projects)
        {
            try
            {
                switch (sortBy)
                {
                    case "issues":
                        projects = projects.OrderByDescending(o => o.Insight.TotalIssueCount).ToList();
                        return projects;
                    case "updated":
                        projects = projects = projects.OrderByDescending(o => o.Insight.LastIssueUpdateTime).ToList();
                        return projects;
                }
                return projects;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while sorting projects: " + ex.Message);
                return projects;
            }

        }

        // Get projects from the Jira API
        private RootProjectsObject GetProjects(JiraOptions.ListProjectsOptions options, string url)
        {
            try
            {
                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();
                RootProjectsObject projectsList = webRequestHandler.GetJson<RootProjectsObject>(url, options.Cookie);
                return projectsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while getting projects: " + ex.Message);
                return null;
            }

        }

        // Print the list of projects
        private void PrintProjects(List<Project> projects, TextWriter writer)
        {
            try
            {
                writer.WriteLine();
                writer.WriteLine("Total projects = " + projects.Count.ToString());
                writer.WriteLine();

                for (int i = 0; i < projects.Count; i++)
                {
                    Project project = projects[i];
                    writer.WriteLine("  Project Name        : " + project.Name);
                    writer.WriteLine("  Project Key         : " + project.Key);
                    writer.WriteLine("  Project Id          : " + project.Id);
                    writer.WriteLine("  Project Type        : " + project.ProjectTypeKey);
                    writer.WriteLine("  Last Issue Update   : " + project.Insight.LastIssueUpdateTime);
                    writer.WriteLine("  Total Issues        : " + project.Insight.TotalIssueCount);
                    writer.WriteLine("  Project Description : " + project.Description.Replace("\r\n", " "));
                    writer.WriteLine("  Project Issue Types : ");
                    for (int j = 0; j < project.IssueTypes.Count; j++)
                    {
                        writer.WriteLine("                        " +project.IssueTypes[j].Name);
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while printing projects: " + ex.Message);
            }

        }
    }

    internal class RootProjectsObject
    {
        [JsonProperty("self")]
        internal string Self { get; set; }

        [JsonProperty("nextPage")]
        internal string NextPage { get; set; }

        [JsonProperty("total")]
        internal int Total { get; set; }

        [JsonProperty("isLast")]
        internal bool IsLast { get; set; }

        [JsonProperty("values")]
        internal List<Project> Projects { get; set; }
    }

    internal class Project
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("key")]
        internal string Key { get; set; }

        [JsonProperty("description")]
        internal string Description { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("projectCategory")]
        internal ProjectCategory ProjectCategory { get; set; }

        [JsonProperty("projectTypeKey")]
        internal string ProjectTypeKey { get; set; }

        [JsonProperty("insight")]
        internal Insight Insight { get; set; }

        [JsonProperty("issueTypes")]
        internal List<IssueType> IssueTypes { get; set; }

    }

    internal class IssueType
    {
        [JsonProperty("self")]
        internal string Self { get; set; }
        [JsonProperty("id")]
        internal string Id { get; set; }
        [JsonProperty("description")]
        internal string Description { get; set; }
        [JsonProperty("name")]
        internal string Name { get; set; }
    }

    internal class ProjectCategory
    {
        [JsonProperty("name")]
        internal string Name { get; set; }

        [JsonProperty("description")]
        internal string description { get; set; }

        [JsonProperty("id")]
        internal string Id { get; set; }
    }

    internal class Insight
    {
        [JsonProperty("totalIssueCount")]
        internal int TotalIssueCount { get; set; }

        [JsonProperty("lastIssueUpdateTime")]
        internal string LastIssueUpdateTime { get; set; }
    }
}
