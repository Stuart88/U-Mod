using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using U_Mod.Models;

namespace U_Mod.Helpers
{
    public static class HttpHelpers
    {
        #region Private Fields

        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        private static readonly HttpClient Client = new HttpClient();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Fetches JSON as JsonBasicResult<T>
        /// </summary>
        /// <typeparam name="T">Expected return data type</typeparam>
        /// <param name="uri">uri key</param>
        /// <param name="queryDictionary"></param>
        /// <returns></returns>
        public static async Task<T> Fetch<T>(string uri, Dictionary<string, string> queryDictionary) where T : new()
        {
            try
            {
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string fullUri = string.Format("{0}?{1}", uri, AmgShared.Helpers.StringHelpers.DictToQueryString(queryDictionary));

                string json_data = await Client.GetStringAsync(fullUri);

                if (!string.IsNullOrEmpty(json_data))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(json_data);
                }
                else
                {
                    throw new Exception("No result data!");
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("Fetch", e);

                GeneralHelpers.ShowExceptionMessageBox("Mod list failed to download!", e);

                return new T();
            }
            finally
            {
                Client.DefaultRequestHeaders.Accept.Clear();
            }
        }

        /// <summary>
        /// Same as Fetch method but adds the given api key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="queryDictionary"></param>
        /// <param name="apiKey">Nexus Mods API Key</param>
        /// <returns></returns>
        public static async Task<FetchResponse<T>> FetchFromNexus<T>(string uri, Dictionary<string, string> queryDictionary, string apiKey) where T : new()
        {
            try
            {
                Client.DefaultRequestHeaders.Remove("apikey");
                Client.DefaultRequestHeaders.Accept.Clear();

                Client.DefaultRequestHeaders.Add("apikey", apiKey);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string fullUri = string.Format("{0}?{1}", uri, AmgShared.Helpers.StringHelpers.DictToQueryString(queryDictionary));

                string json_data = await Client.GetStringAsync(fullUri);

                Client.DefaultRequestHeaders.Remove("apikey");
                Client.DefaultRequestHeaders.Accept.Clear();

                if (!string.IsNullOrEmpty(json_data))
                {
                    T data = System.Text.Json.JsonSerializer.Deserialize<T>(json_data);
                    
                    return new FetchResponse<T>
                    {
                        Ok = true,
                        Data = data,
                        ErrorMessage = ""
                    };
                }
                else
                {
                    throw new Exception("No result data!");
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("FetchFromNexus", e);
                return new FetchResponse<T>
                {
                    Ok = false,
                    Data = new T(),
                    ErrorMessage = AmgShared.Helpers.StringHelpers.ErrorMessage(e)
                };
            }
        }

        public static async Task<T> FetchFromWasabi<T>(string uri, Dictionary<string, string> queryDictionary) where T : new()
        {
            try
            {
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string fullUri = string.Format("{0}?{1}", uri, AmgShared.Helpers.StringHelpers.DictToQueryString(queryDictionary));

                string json_data = await Client.GetStringAsync(fullUri);

                if (!string.IsNullOrEmpty(json_data))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(json_data);
                }
                else
                {
                    throw new Exception("No result data!");
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("Fetch", e);

                GeneralHelpers.ShowExceptionMessageBox("Mod list failed to download!", e);

                return new T();
            }
            finally
            {
                Client.DefaultRequestHeaders.Accept.Clear();
            }
        }

        #endregion Public Methods
    }
}