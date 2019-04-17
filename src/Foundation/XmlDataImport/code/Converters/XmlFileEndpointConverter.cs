using SaudiAramco.Foundation.XmlDataImport.Plugins;
using SaudiAramco.Foundation.XmlDataImport.Models;
using Sitecore.DataExchange.Converters.Endpoints;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;

namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class XmlFileEndpointConverter : BaseEndpointConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{4D550AA7-FE46-4820-A87F-68122ED3C4A3}");

        public XmlFileEndpointConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        protected override void AddPlugins(ItemModel source, Endpoint endpoint)
        {
            //
            //create the plugin
            var settings = new XmlFileSettings();
            //
            //populate the plugin using values from the item
            settings.XmlRoot =
                base.GetStringValue(source, XmlFileEndpointItemModel.XmlRoot);
            settings.Path =
                base.GetStringValue(source, XmlFileEndpointItemModel.Path);
            //
            //add the plugin to the endpoint
            endpoint.Plugins.Add(settings);
        }
    }
}
