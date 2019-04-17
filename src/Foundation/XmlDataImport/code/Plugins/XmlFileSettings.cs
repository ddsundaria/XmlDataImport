using Sitecore.DataExchange;

namespace SaudiAramco.Foundation.XmlDataImport.Plugins
{
    public class XmlFileSettings : IPlugin
    {
        public string Path { get; set; }
        public string XmlRoot { get; set; }
        
    }
}
