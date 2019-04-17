using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace SaudiAramco.Foundation.XmlDataImport.Extensions
{
    public class ChildItemCreator
    {
        public string ChildXml { set; get; }
        public string ItemID { set; get; }
        public string Language { set; get; }
        private Item ParentItem { set; get; }
        private Item LocalDatasource { set; get; }
        private Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");

        public ChildItemCreator(string childXml, string itemId)
        {
            ChildXml = childXml;
            ItemID = itemId;
        }

        public void CreateChildItems()
        {
            ParentItem = dbContext.GetItem(new ID(ItemID));

            ParentItem.DeleteChildren();

            //Create Local Datasource Folder
            CreateLocalDatasource();
            ItemRendering.ResetRenderings(ParentItem);

            AddChild();

            //Create Renderings
            //ItemRendering.AddRenderings(ParentItem, LocalDatasource);
        }

        private void AddChild()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(ChildXml);

            XmlNode xnode = xdoc.SelectSingleNode("//primary");
            foreach (XmlNode node in xnode.ChildNodes)
            {
                if (node.Name.StartsWith("text"))
                {
                    string textValue = string.Empty;
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name.EndsWith(":text"))
                        {
                            textValue = attr.Value;
                        }
                    }

                    CreateTextChild(textValue);
                }
                else if (node.Name.StartsWith("callout"))
                {
                    string quote = string.Empty;
                    string citeOrigin = string.Empty;

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name.EndsWith(":header"))
                        {
                            citeOrigin = attr.Value;
                        }
                        if (attr.Name.EndsWith(":text"))
                        {
                            quote = attr.Value;
                        }
                    }

                    CreateQuoteChild(quote, citeOrigin);
                }
            }
        }

        private void CreateLocalDatasource()
        {
            using (new SecurityDisabler())
            {
                TemplateItem template = dbContext.GetTemplate(LocalContent.ID);

                Item newItem = ParentItem.Add("_Local", template);
                if (newItem != null)
                {
                    newItem.Editing.BeginEdit();
                    newItem["__Subitems Sorting"] = "{C1FF011E-B02A-44E3-8444-9FC89CFC28CE}";
                    newItem.Appearance.DisplayName = "Local Content";
                    newItem.Editing.EndEdit();
                }

                LocalDatasource = newItem;
            }
        }

        private void CreateTextChild(string text)
        {
            var parentItem = LocalDatasource;

            using (new SecurityDisabler())
            {
                TemplateItem template = dbContext.GetTemplate(NewsChild.Text);

                Item newItem = parentItem.Add("Text", template);

                if (newItem != null)
                {
                    newItem.Editing.BeginEdit();
                    newItem.Fields["TextContent"].Value = text;
                    newItem.Editing.EndEdit();
                }
            }
        }

        private void CreateQuoteChild(string quote, string citeOrigin)
        {
            var parentItem = LocalDatasource;

            using (new SecurityDisabler())
            {
                TemplateItem template = dbContext.GetTemplate(NewsChild.Quote);

                Item newItem = parentItem.Add("Quote", template);

                if (newItem != null)
                {
                    newItem.Editing.BeginEdit();
                    newItem.Fields["Quote"].Value = quote;
                    newItem.Fields["CiteOrigin"].Value = citeOrigin;
                    newItem.Editing.EndEdit();
                }
            }
        }


    }
}