using SaudiAramco.Foundation.XmlDataImport.Plugins;
using Sitecore.DataExchange.Models;

namespace SaudiAramco.Foundation.XmlDataImport.Extensions
{
    public static class EndpointExtensions
    {
        public static XmlFileSettings GetXmlFileSettings(this Endpoint endpoint)
        {
            return endpoint.GetPlugin<XmlFileSettings>();
        }
        public static bool HasXmlFileSettings(this Endpoint endpoint)
        {
            return (GetXmlFileSettings(endpoint) != null);
        }
    }
}
