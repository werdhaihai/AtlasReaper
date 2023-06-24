using Newtonsoft.Json;
using AtlasReaper.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AtlasReaper.Jira
{
    internal class Users
    {
        internal void ListUsers(JiraOptions.ListUsersOptions options)
        {
            try
            {
                // Get All Users
                // GET /rest/api/3/users/search?maxResults=200&startAt=200
                int count = 0;
                int pageSize = 200;
                bool moreUsers = true;

                Utils.WebRequestHandler webRequestHandler = new Utils.WebRequestHandler();

                List<User> allUsers = new List<User>();


                while (moreUsers)
                {
                    string url = options.Url + "/rest/api/3/users/search?maxResults=200&startAt=" + count.ToString();
                    List<User> users = webRequestHandler.GetJson<List<User>>(url, options.Cookie);
                    allUsers.AddRange(users);

                    count += pageSize;

                    if (users.Count < pageSize)
                    {
                        moreUsers = false;
                    }
                }
                if (options.outfile != null)
                {
                    using (StreamWriter writer = new StreamWriter(options.outfile))
                    {
                        PrintUsers(allUsers, options.Full, writer);
                    }
                }
                else
                {
                    PrintUsers(allUsers, options.Full, Console.Out);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while listing users: " + ex.Message);

            }

        }

        private void PrintUsers(List<User> Users, bool full, TextWriter writer)
        {
            try
            {
                Users = Users.OrderBy(o => o.EmailAddress).ToList();
                for (int i = 0; i < Users.Count; i++)
                {
                    User user = Users[i];
                    if (user.EmailAddress != null)
                    {
                        if (full)
                        {
                            writer.WriteLine("User Name : " + user.DisplayName);
                            writer.WriteLine("User Id   : " + user.AccountId );
                            writer.WriteLine("Active    : " + user.Active.ToString());
                        }
                        writer.WriteLine("User Email: " + user.EmailAddress);
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while printing users: " + ex.Message);
            }
        }
    }

    internal class User
    {
        [JsonProperty("accountId")]
        internal string AccountId { get; set; }

        [JsonProperty("accountType")]
        internal string AccountType { get; set; }

        [JsonProperty("active")]
        internal bool Active { get; set; }

        [JsonProperty("displayName")]
        internal string DisplayName { get; set; }

        [JsonProperty("emailAddress")]
        internal string EmailAddress { get; set; }

        [JsonProperty("name")]
        internal string Name { get; set; }

    }
}
