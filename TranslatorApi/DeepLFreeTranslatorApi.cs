using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TranslatorNS.TranslatorApi
{
    internal class DeepLFreeTranslatorApi : ITranslatorApi
    {
        private const string path = "https://api-free.deepl.com/v2/translate";
        private const string authPath = "https://api-free.deepl.com/v2/usage";

        private readonly string key;
        private readonly HttpClient httpClient;

        private Lang dstLang;

        public DeepLFreeTranslatorApi(string apikey)
        {
            key = apikey;
            httpClient = new HttpClient();
            dstLang = Lang.English;

            if (!IsApiKeyValid())
                throw new Exception("api key is not a valid key.");
        }

        public bool IsApiKeyValid()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(authPath);
                request.Headers["Authorization"] = $"DeepL-Auth-Key {key}";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                return false;
            }catch { return false; }
        }

        public void SetLang(Lang lang)
        {
            dstLang = lang;
        }

        public string Translate(string text)
        {
            return TranslateAsync(text).GetAwaiter().GetResult();
        }

        public string ParseDstLang()
        {
            switch (dstLang)
            {
                case Lang.English: return "EN";
                case Lang.Japanese: return "JAP";
                case Lang.French: return "FR";
                case Lang.German: return "DE";
                default: return "EN";
            }
        }

        public async Task<string> TranslateAsync(string text)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("auth_key", key));
            parameters.Add(new KeyValuePair<string, string>("text", text));
            parameters.Add(new KeyValuePair<string, string>("target_lang", ParseDstLang()));

            FormUrlEncodedContent data = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await httpClient.PostAsync(path, data);
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return json["translations"][0]["text"].ToString();
        }

        public string[] TranslateBatch(string[] texts)
        {
            return TranslateBatchAsync(texts).GetAwaiter().GetResult();
        }

        public async Task<string[]> TranslateBatchAsync(string[] texts)
        {
            if (texts.Length > 50)
                return null;

            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>("auth_key", key));
            parameters.Add(new KeyValuePair<string, string>("target_lang", ParseDstLang()));

            foreach(string s in texts)
            {
                parameters.Add(new KeyValuePair<string, string>("text", s));
            }

            FormUrlEncodedContent data = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await httpClient.PostAsync(path, data);
            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            JToken[] translations = json["translations"].ToArray();

            string[] r = new string[translations.Length];

            for (int i = 0; i < translations.Length; i++)
            {
                r[i] = translations[i]["text"].ToString();
            }

            return r;
        }

        public int GetBatchSize()
        {
            return 50;
        }
    }
}
