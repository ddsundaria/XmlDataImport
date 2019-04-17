using SaudiAramco.Foundation.XmlDataImport.Plugins;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Processors.PipelineSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using System.Xml;
using Sitecore.Data;
using Sitecore.DataExchange.Providers.Sc.Plugins;
using Sitecore.Data.Items;
using SaudiAramco.Foundation.XmlDataImport.Extensions;

namespace SaudiAramco.Foundation.XmlDataImport.Processors
{
    [RequiredEndpointPlugins(typeof(ProcessChildSettings))]
    public class ProcessChildNodesStepProcessor : BasePipelineStepProcessor
    {
        private Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");

        public override void Process(PipelineStep pipelineStep, PipelineContext pipelineContext)
        {
            if (pipelineStep == null)
                throw new ArgumentNullException(nameof(pipelineStep));
            if (pipelineContext == null)
                throw new ArgumentNullException(nameof(pipelineContext));

            var xml = pipelineContext.GetPlugin<SynchronizationSettings>();

            XmlNode xmlData = (XmlNode)xml.Source;

            if (null == xmlData)
                return;
            var childnodeSettings = pipelineStep.GetPlugin<ProcessChildSettings>();

            if (null == childnodeSettings) return;

            var xmlNodes = xmlData.SelectNodes(childnodeSettings.Xpath);
            string itemName = xmlData.Attributes["itemName"].Value;
            List<string> lstLang = null;

            string parentPath = string.Empty;

            foreach (var step in pipelineContext.CurrentPipeline.PipelineSteps)
            {
                if (step.PipelineStepProcessor.GetType().UnderlyingSystemType.FullName == "Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.ResolveMultilanguageSitecoreItemDictionaryPipelineStep")
                {
                    foreach (var plugin in step.Plugins)
                    {
                        if (plugin.GetType().Name == "ResolveSitecoreItemSettings")
                        {
                            ResolveSitecoreItemSettings settings = (ResolveSitecoreItemSettings)plugin;

                            Item parentItem = dbContext.GetItem(new ID(settings.ParentItemIdItem.ToString()));
                            parentPath = parentItem.Paths.FullPath;
                        }
                    }
                }
                else if (step.PipelineStepProcessor.GetType().UnderlyingSystemType.FullName == "Sitecore.DataExchange.Providers.Sc.Processors.PipelineSteps.SelectLanguagesPipelineStep")
                {
                    foreach (var plugin in step.Plugins)
                    {
                        if (plugin.GetType().Name == "SelectedLanguagesSettings")
                        {
                            lstLang = ((SelectedLanguagesSettings)plugin).Languages.ToList();
                        }
                    }
                }
            }

            if ((!string.IsNullOrEmpty(parentPath)) && lstLang != null)
            {
                foreach (string lang in lstLang)
                {
                    Item item = dbContext.GetItem(string.Format("{0}/{1}", parentPath, itemName), Sitecore.Globalization.Language.Parse(lang));

                    if(item != null)
                    {
                        if (item.Name == item.Fields["Identifier"].Value.ToString())
                        {
                            ChildItemCreatorML obj = new ChildItemCreatorML(xmlData.OuterXml, item.ID.ToString(), lang);
                            obj.CreateChildItems();

                            ItemRendering.AddRenderings(item);
                        }

                        if (item.Name != item.Fields["Identifier"].Value.ToString())
                        {
                            item.Versions.RemoveVersion();
                        }
                    }
                }
            }
        }
    }
}