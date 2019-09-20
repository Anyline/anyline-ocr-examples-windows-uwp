namespace Anyline_Windows_UWP_Examples.Model
{
    public class ExamplePlugin
    {
        public ExamplePlugin(string name, string jsonConfigFile)
        {
            Name = name;
            JSONConfigFile = jsonConfigFile;
        }

        public string Name { get; set; }
        public string JSONConfigFile { get; set; }
    }
}
