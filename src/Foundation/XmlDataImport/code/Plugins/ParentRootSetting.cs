using Sitecore.DataExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaudiAramco.Foundation.XmlDataImport.Plugins
{
    public class ParentRootSetting : IPlugin
    {
        public Guid ParentRoot { get; set; }
    }
}