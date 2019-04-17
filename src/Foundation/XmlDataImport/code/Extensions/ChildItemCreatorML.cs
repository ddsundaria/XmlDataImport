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
    public class ChildItemCreatorML
    {
        public string ChildXml { set; get; }
        public string ItemID { set; get; }
        public string Language { set; get; }
        private Item ParentItem { set; get; }
        private Item LocalDatasource { set; get; }
        private Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");

        public ChildItemCreatorML(string childXml, string itemId, string language)
        {
            ChildXml = childXml;
            ItemID = itemId;
            Language = language;
        }

        public void CreateChildItems()
        {
            ParentItem = dbContext.GetItem(new ID(ItemID), Sitecore.Globalization.Language.Parse(Language));

            if (ParentItem.Name == ParentItem.Fields["Identifier"].Value)
            {
                if (ParentItem.Children.Count == 0)
                {
                    CreateLocalDatasource();
                    AddChild();
                }
                else
                {
                    LocalDatasource = ParentItem.Children.FirstOrDefault();
                    AddLanguageChild();
                }
            }
        }

        private void AddChild()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(ChildXml);

            XmlNode xnode = xdoc.SelectSingleNode("//primary");
            if (xnode != null)
            {
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
        }


        private void AddLanguageChild()
        {
            Dictionary<string, object> childItems = new Dictionary<string, object>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(ChildXml);

            XmlNode xnode = xdoc.SelectSingleNode("//primary");

            if (xnode != null)
            {
                for (int i = 0; i < xnode.ChildNodes.Count; i++)
                {
                    if (xnode.ChildNodes[i].Name.StartsWith("text"))
                    {
                        string textValue = string.Empty;
                        foreach (XmlAttribute attr in xnode.ChildNodes[i].Attributes)
                        {
                            if (attr.Name.EndsWith(":text"))
                            {
                                textValue = attr.Value;
                            }
                        }

                        childItems.Add("text" + i.ToString(), textValue);
                    }
                    else if (xnode.ChildNodes[i].Name.StartsWith("callout"))
                    {
                        string[] quotes = new string[2];

                        foreach (XmlAttribute attr in xnode.ChildNodes[i].Attributes)
                        {
                            if (attr.Name.EndsWith(":header"))
                            {
                                quotes[0] = attr.Value;
                            }
                            if (attr.Name.EndsWith(":text"))
                            {
                                quotes[1] = attr.Value;
                            }
                        }

                        childItems.Add("quote" + i.ToString(), quotes);
                    }
                }
            }

            if (LocalDatasource.Children.Count == childItems.Count)
            {
                for (int i = 0; i < LocalDatasource.Children.Count; i++)
                {
                    Item childItemNew = dbContext.GetItem(LocalDatasource.Children[i].ID, Sitecore.Globalization.Language.Parse(Language));
                    if (childItemNew != null)
                    {
                        foreach (var dicItem in childItems)
                        {
                            if (dicItem.Key.StartsWith("text") && childItemNew.Fields["TextContent"] != null)
                            {
                                if (childItemNew.Fields["TextContent"].Value == null || string.IsNullOrEmpty(childItemNew.Fields["TextContent"].Value.ToString()))
                                {
                                    childItemNew.Editing.BeginEdit();
                                    childItemNew.Fields["TextContent"].Value = dicItem.Value.ToString();
                                    childItemNew.Editing.EndEdit();
                                }
                            }
                            else if (dicItem.Key.StartsWith("quote") && childItemNew.Fields["Quote"] != null)
                            {
                                if (childItemNew.Fields["Quote"].Value == null || string.IsNullOrEmpty(childItemNew.Fields["Quote"].Value.ToString()))
                                {
                                    string[] quoteValue = dicItem.Value as string[];
                                    childItemNew.Editing.BeginEdit();
                                    childItemNew.Fields["Quote"].Value = quoteValue[0];
                                    childItemNew.Fields["CiteOrigin"].Value = quoteValue[1];
                                    childItemNew.Editing.EndEdit();
                                }
                            }
                        }
                    }
                    else
                    {
                        //
                    }
                }

            }
            else
            {
                //
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