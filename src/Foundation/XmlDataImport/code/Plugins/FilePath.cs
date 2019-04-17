using Sitecore.DataExchange.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaudiAramco.Foundation.XmlDataImport.Plugins
{
    public class FilePath : IDataAccessPlugin
    {
        public string Path { get; set; }
    }
}