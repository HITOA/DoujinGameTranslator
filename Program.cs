using CommandLine;
using System;
using System.Linq;
using System.Reflection;

namespace TranslatorNS
{
    
    static class Program
    {
        static void Main(string[] args)
        {
            Type[] verbs = GetVerbs();

            try
            {
                Parser.Default.ParseArguments(args, verbs)
                    .WithParsed(Translator.Run);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        static Type[] GetVerbs()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttributes<VerbAttribute>() != null).ToArray();
        }
        static void HandleException(Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
    }
}