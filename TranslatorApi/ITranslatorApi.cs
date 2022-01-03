using System.Threading.Tasks;

namespace TranslatorNS.TranslatorApi {
    interface ITranslatorApi
    {
        int GetBatchSize();
        void SetLang(Lang lang);
        string Translate(string text);
        Task<string> TranslateAsync(string text);
        string[] TranslateBatch(string[] text);
        Task<string[]> TranslateBatchAsync(string[] text);
    }
}