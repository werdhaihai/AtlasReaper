using Newtonsoft.Json;
using System;



namespace AtlasReaper.Confluence
{
    public class Auth
    {
        // Sends email to the user!!
        // Look into creating an API token
        // POST https://id.atlassian.com/manage/rest/api-tokens
        // data: 
        //          {"label":"testtoken"}
        // cloud.session.token

        // response 
        //      {"passwordValue":""}
        public static void CheckAuth(string url, string cookie)
        {
            try
            {
                // Check cookie if supplied or attempt anonymous access
                var webRequestHandler = new Utils.WebRequestHandler();
                string authCheckUrl = url + "/wiki/rest/api/user/current";
                User user;

                // Get user information
                user = webRequestHandler.GetJson<User>(authCheckUrl, cookie);

                if (user.DisplayName != null)
                {
                    Console.WriteLine("Authenticated as: " + user.DisplayName);
                }
                else
                {
                    throw new InvalidOperationException("An error occurred while checking authentication (Session expired or invalid)");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
               
            }
        }
    }

    public class User 
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        //[JsonProperty("message")]
        //public string Message { get; set; }
    }

}
