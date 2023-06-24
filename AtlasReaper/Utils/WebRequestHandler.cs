using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace AtlasReaper.Utils
{
    public class WebRequestHandler
    {
        public T GetJson<T>(string url, string cookie)
        {
            try
            {
                if (cookie != null)
                {
                    Uri baseAddress = new Uri(url);
                    CookieContainer cookieContainer = new CookieContainer();
                    using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                    using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                    {
                        System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        cookieContainer.Add(baseAddress, new Cookie("cloud.session.token", cookie));
                        HttpResponseMessage httpResponse = client.GetAsync(url).Result;
                        string result = httpResponse.Content.ReadAsStringAsync().Result;
                        T deserializedObject = JsonConvert.DeserializeObject<T>(result);
                        return deserializedObject;
                    }
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        HttpResponseMessage httpResponse = client.GetAsync(url).Result;
                        string result = httpResponse.Content.ReadAsStringAsync().Result;
                        T deserializedObject = JsonConvert.DeserializeObject<T>(result);
                        return deserializedObject;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured in Utils.WebRequestHandler.GetJson method: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                return default;
            }
        }

        public T PostJson<T>(string url, string cookie, string serializedData)
        {
            try
            {
                Uri baseAddress = new Uri(url);
                CookieContainer cookieContainer = new CookieContainer();

                using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    if (cookie != null)
                    {
                        cookieContainer.Add(baseAddress, new Cookie("cloud.session.token", cookie));
                    }

                    HttpContent content = new StringContent(serializedData, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = client.PostAsync(url, content).Result;

                    string result = httpResponse.Content.ReadAsStringAsync().Result;
                    T deserializedObject = JsonConvert.DeserializeObject<T>(result);
                    return deserializedObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in Utils.WebRequestHandler.PostJson method: " + ex.Message);
                return default;

            }
        }

        public T PutJson<T>(string url, string cookie, string serializedData)
        {
            try
            {

                Uri baseAddress = new Uri(url);
                CookieContainer cookieContainer = new CookieContainer();

                using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    if (cookie != null)
                    {
                        cookieContainer.Add(baseAddress, new Cookie("cloud.session.token", cookie));
                    }

                    HttpContent content = new StringContent(serializedData, Encoding.UTF8, "application/json");
                    HttpResponseMessage httpResponse = client.PutAsync(url, content).Result;

                    string result = httpResponse.Content.ReadAsStringAsync().Result;
                    T deserializedObject = JsonConvert.DeserializeObject<T>(result);
                    return deserializedObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in Utils.WebRequestHandler.PutJson method: " + ex.Message);
                return default;
            }
        }

        public T PostForm<T>(string url, string cookie, object formData, string fileName)
        {
            try
            {
                Uri baseAddress = new Uri(url);
                CookieContainer cookieContainer = new CookieContainer();

                using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    if (cookie != null)
                    {
                        cookieContainer.Add(baseAddress, new Cookie("cloud.session.token", cookie));
                    }

                    // LOL tell confluence to NOT require a CSRF token
                    client.DefaultRequestHeaders.Add("X-Atlassian-Token", "nocheck");

                    MultipartFormDataContent formContent = new MultipartFormDataContent();

                    PropertyInfo[] properties = formData.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        object value = property.GetValue(formData);
                        if (value is string stringValue)
                        {
                            var stringContent = new StringContent(stringValue);
                            formContent.Add(stringContent, property.Name);
                        }
                        else if (value is byte[] byteArrayValue)
                        {
                            var fileContent = new ByteArrayContent(byteArrayValue);
                            formContent.Add(fileContent, property.Name, fileName);
                        }
                        else
                        {
                            throw new ArgumentException($"Unsupported form field type: {property.PropertyType}");
                        }
                    }

                    HttpResponseMessage httpResponse = client.PostAsync(url, formContent).Result;

                    string result = httpResponse.Content.ReadAsStringAsync().Result;
                    T deserializedObject = JsonConvert.DeserializeObject<T>(result);
                    return deserializedObject;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in Utils.WebRequestHandler.PostForm method: " + ex.Message);
                return default;
            }
        }

        public void DownloadFile(string url, string cookie, string outputFilePath)
        {
            try
            {
                if (cookie != null)
                {
                    Uri baseAddress = new Uri(url);
                    CookieContainer cookieContainer = new CookieContainer();
                    using (HttpClientHandler handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                    using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                    {
                        System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        cookieContainer.Add(baseAddress, new Cookie("cloud.session.token", cookie));
                        HttpResponseMessage httpResponse = client.GetAsync(url).Result;
                        string redirectUrl = httpResponse.RequestMessage.RequestUri.ToString();

                        using (Stream contentStream = httpResponse.Content.ReadAsStreamAsync().Result)
                        using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                        {
                            contentStream.CopyToAsync(fileStream).Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred in Utils.WebRequestHandler.DownloadFile method: " + ex.Message);
            }
            
        }
    }
}
