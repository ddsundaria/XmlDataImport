using Sitecore.DataExchange.DataAccess;
using Sitecore.Services.Core.Model;
using System;
using System.Globalization;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer
{
    public class XmlValueWriter : IValueWriter
    {
        public XmlValueWriter(string xpath, bool isAttribute)
        {
            XPath = xpath;
            IsAttribute = isAttribute;
        }

        public string XPath { get; set; }
        public bool IsAttribute { get; set; }

        public CanWriteResult CanWrite(object target, object value, DataAccessContext context)
        {
            throw new NotImplementedException();
        }

        public bool Write(object target, object value, DataAccessContext context)
        {
            throw new NotImplementedException();
        }
    }
}
