using Sitecore.DataExchange.Converters.PipelineSteps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using SaudiAramco.Foundation.XmlDataImport.Models;
using SaudiAramco.Foundation.XmlDataImport.Plugins;

namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class ProcessChildNodes : BasePipelineStepConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{AF199FB6-65E4-4B0E-9FE9-7703EAD5922A}");

        public ProcessChildNodes(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        protected override void AddPlugins(ItemModel source, PipelineStep pipelineStep)
        {
            AddEndpointSettings(source, pipelineStep);
        }
        private void AddEndpointSettings(ItemModel source, PipelineStep pipelineStep)
        {
            var childnodes = new ProcessChildSettings();

            var xpath = GetStringValue(source, ProcessChildNodesStepModel.XPath);

            if (xpath != null)
            {
                childnodes.Xpath = xpath;
            }
            pipelineStep.Plugins.Add(childnodes);
        }
    }
}