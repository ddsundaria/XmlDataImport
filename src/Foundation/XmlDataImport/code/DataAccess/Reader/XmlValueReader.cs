using System;
using System.Xml;
using Sitecore.DataExchange.DataAccess;
using System.Globalization;
using Sitecore.Data;
using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using SaudiAramco.Foundation.XmlDataImport.Plugins;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Reader
{
    public class XmlValueReader : IValueReader
    {
        public string XPath { get; set; }
        public bool IsAttribute { get; set; }
        public XmlValueReader(string xpath, bool isAttribute)
        {
            XPath = xpath;
            IsAttribute = isAttribute;
        }

        public CanReadResult CanRead(object source, DataAccessContext context)
        {
            CanReadResult canReadResult = new CanReadResult { CanReadValue = true };
            // NOTE - hardcoded to true
            return canReadResult;
        }

        public ReadResult Read(object source, DataAccessContext context)
        {
            var filePath = string.Empty;
            ReadResult readResult = new ReadResult(DateTime.Now);

            XmlNode productNode = (XmlNode)source;
            string value = string.Empty;
            XmlNamespaceManager xmlNameSpaceMgr;
            xmlNameSpaceMgr = new XmlNamespaceManager(new XmlDocument().NameTable);
            xmlNameSpaceMgr.AddNamespace("jcr", "http://www.jcp.org/jcr/1.0");


            if (!IsAttribute)
            {
                XmlNode firstNode = productNode.SelectSingleNode(XPath, xmlNameSpaceMgr);
                value = firstNode?.InnerText;
                if (string.IsNullOrEmpty(value))
                    value = firstNode?.InnerXml;
                if (XPath == ".")
                    value = firstNode?.OuterXml;
            }
            else if (productNode.Attributes != null)
            {
                value = productNode.Attributes[XPath]?.Value;
                filePath = productNode.Attributes["filePath"]?.Value;
            }

            readResult.ReadValue = value;
            readResult.WasValueRead = true;

            return readResult;
        }
    }
}
