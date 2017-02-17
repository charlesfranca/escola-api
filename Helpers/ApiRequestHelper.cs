using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EscolaDeVoce.API
{
    public class ApiRequestHelper
    {
        public const string CONTENT_TYPE_JSON = "application/json";
        public const string CONTENT_TYPE_URLENCODED = "application/x-www-form-urlencoded";
        public async static Task<T> Get<T>(string url, Dictionary<string, string> urlParameters = null){
            string saida = "";
            T response;
            
            using (var client = new HttpClient())
            {
                string _urlParameters = "";
                if(urlParameters != null){
                    StringBuilder strParam = new StringBuilder();
                    foreach(string p in urlParameters.Keys){
                        strParam.Append(string.Format("{0}={1}", p, urlParameters[p]));
                    }
                    _urlParameters = "?" + System.Net.WebUtility.UrlEncode(strParam.ToString());
                }

                saida = await client.GetStringAsync(url + _urlParameters);
                Console.Write(saida);
                response = JsonConvert.DeserializeObject<T>(saida);
            }

            return response;
        }
    }
}
