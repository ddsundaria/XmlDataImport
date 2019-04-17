using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Contexts;
using Sitecore.DataExchange.Models;
using Sitecore.DataExchange.Plugins;
using Sitecore.DataExchange.Processors.PipelineSteps;
using System;
using System.IO;
using System.Xml;
using SaudiAramco.Foundation.XmlDataImport.Plugins;
using SaudiAramco.Foundation.XmlDataImport.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace SaudiAramco.Foundation.XmlDataImport.Processors
{
    [RequiredEndpointPlugins(typeof(XmlFileSettings))]
    public class ReadXmlFileStepProcessor : BaseReadDataStepProcessor
    {
        protected override void ReadData(
            Endpoint endpoint,
            PipelineStep pipelineStep,
            PipelineContext pipelineContext)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (pipelineStep == null)
            {
                throw new ArgumentNullException(nameof(pipelineStep));
            }
            if (pipelineContext == null)
            {
                throw new ArgumentNullException(nameof(pipelineContext));
            }
            var logger = pipelineContext.PipelineBatchContext.Logger;
            //
            //get the file path from the plugin on the endpoint
            var settings = endpoint.GetXmlFileSettings();
            if (settings == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(settings.Path))
            {
                logger.Error(
                    "No path is specified on the endpoint. " +
                    "(pipeline step: {0}, endpoint: {1})",
                    pipelineStep.Name, endpoint.Name);
                return;
            }
            
            List<string> xmlFilePath = Directory.GetFiles(settings.Path, "*.xml", SearchOption.AllDirectories).ToList();

            if (xmlFilePath == null || xmlFilePath.Count() <= 0)
            {
                logger.Error(
                    "The path specified on the endpoint does not have any xml file. " +
                    "(pipeline step: {0}, endpoint: {1}, path: {2})",
                    pipelineStep.Name, endpoint.Name, settings.Path);
                return;
            }
            var newsNodes = new List<XmlNode>();
            XmlNamespaceManager xmlNameSpaceMgr;
            xmlNameSpaceMgr = new XmlNamespaceManager(new XmlDocument().NameTable);
            xmlNameSpaceMgr.AddNamespace("jcr", "http://www.jcp.org/jcr/1.0");

            foreach (string path in xmlFilePath)
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(File.OpenRead(path));

                var catNodes = xdoc.SelectNodes(settings.XmlRoot, xmlNameSpaceMgr);

                XmlAttribute attr = xdoc.CreateAttribute("filePath");
                attr.Value = path;

                string itemName = Path.GetFileName(Path.GetDirectoryName(path));

                itemName = itemName.Trim('-');
                itemName = itemName.Trim(' ');

                //^[\w\*\$][\w\s\-\$] * (\(\d{ 1,}\)){ 0,1}$

                XmlAttribute attrItemName = xdoc.CreateAttribute("itemName");
                attrItemName.Value = itemName.Trim();

                foreach (XmlNode node in catNodes)
                {
                    node.Attributes.SetNamedItem(attr);
                    node.Attributes.SetNamedItem(attrItemName);
                    newsNodes.Add(node);
                }
            }

            var dataSettings = new IterableDataSettings(newsNodes);

            pipelineContext.Plugins.Add(dataSettings);
        }

        
    }
}
