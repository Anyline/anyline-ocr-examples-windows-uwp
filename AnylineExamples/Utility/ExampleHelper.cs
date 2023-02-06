using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AnylineExamples
{
    public class ExampleHelper
    {
        public static Dictionary<string, List<Example>> GetProjectExamples()
        {
            var examples = new Dictionary<string, List<Example>>();

            // Acquire the path where all json configs are
            DirectoryInfo executingDirectoryInfo = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var configPath = Path.Combine(executingDirectoryInfo.Parent.FullName, "Configs");
            var configDirectoryInfo = new DirectoryInfo(configPath);
            var fileInfos = configDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            foreach(var fileInfo in fileInfos)            
            {
                var content = File.ReadAllText(fileInfo.FullName);
                var example = new Example(content, $@"Configs\{fileInfo.Name}");

                if (!examples.ContainsKey(example.Category))
                    examples.Add(example.Category, new List<Example>());

                examples[example.Category].Add(example);
            }

            // sort categories alphabetically
            examples = examples.OrderBy(x => x.Key).ToDictionary(k => k.Key, k => k.Value);
            
            // for each category, sort list alphabetically
            foreach(var key in examples.Keys.ToList())
                examples[key] = examples[key].OrderBy(k => k.Name).ToList();
            
            return examples;
        }
    }
}
