using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.DataAccess;
using System;
using System.Globalization;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Text.RegularExpressions;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer
{
    public class TagValueWriterML : IValueWriter
    {
        public string Field { get; set; }
        public string Filter { get; set; }
        public Guid TagRoot { get; set; }
        string Language { set; get; }
        public TagValueWriterML(string fieldName, string language) : base()
        {
            Field = fieldName;
            Language = language;
        }

        public CanWriteResult CanWrite(object target, object value, DataAccessContext context)
        {
            return CanWriteResult.PositiveResult(true);
        }

        public bool Write(object target, object value, DataAccessContext context)
        {
            Dictionary<string, ItemModel> model = (Dictionary<string, ItemModel>)target;
            
            if (model == null)
                return false;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            if (value != null && Filter != null)
            {
                if (Filter == "sa-tags" && value.ToString().Contains("sa-tags"))
                {
                    string[] tagList = value.ToString().Replace("[", "").Replace("]", "").Replace("sa-tags:", "").Split(',');

                    List<Tag> lstTags = new List<Tag>();

                    foreach (var i in tagList)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            Tag tag = new Tag();
                            tag.Name = i;
                            tag.Available = false;
                            lstTags.Add(tag);
                        }
                    }

                    Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");

                    var items = dbContext.GetItem(TagRoots.TagsRoot).Axes.GetDescendants();

                    foreach (var tag in lstTags)
                    {
                        foreach (var item in items)
                        {
                            if (tag.Name.ToLower() == item.Name.ToLower())
                            {
                                tag.ItemId = item.ID.ToString();
                                tag.Available = true;
                            }
                        }

                        if (!tag.Available && !string.IsNullOrEmpty(tag.Name))
                        {
                            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                            using (new SecurityDisabler())
                            {
                                Item parentItem = dbContext.Items[TagRoots.TagsRoot];
                                TemplateItem template = dbContext.GetTemplate(Templates.Tag);

                                string tagName = Regex.Replace(tag.Name, "[^a-zA-Z0-9_]+", "-");
                                tagName = tagName.Trim('-');
                                tagName = tagName.Trim(' ');

                                Item newItem = parentItem.Add(tagName, template);
                                if (newItem != null)
                                {
                                    newItem.Editing.BeginEdit();
                                    newItem.Appearance.DisplayName = ti.ToTitleCase(tagName);
                                    newItem.Editing.EndEdit();
                                }
                                tag.ItemId = newItem.ID.ToString();
                                tag.Available = true;
                            }
                        }
                    }

                    string tagvalues = string.Empty;

                    if (lstTags.Count > 0)
                    {
                        foreach (var tag in lstTags)
                        {
                            tagvalues = tagvalues + tag.ItemId + "|";
                        }

                        value = tagvalues.Remove(tagvalues.Length - 1, 1);
                    }
                }
                else if (Filter == "sa-locations" && value.ToString().Contains("sa-locations"))
                {
                    string[] tagList = value.ToString().Replace("[", "").Replace("]", "").Replace("sa-locations:", "").Split(',');

                    List<Tag> lstTags = new List<Tag>();

                    foreach (var i in tagList)
                    {
                        if (!string.IsNullOrEmpty(i))
                        {
                            Tag tag = new Tag();
                            tag.Name = i;
                            tag.Available = false;
                            lstTags.Add(tag);
                        }
                    }

                    Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");

                    var items = dbContext.GetItem(TagRoots.LocationTagsRoot).Axes.GetDescendants();

                    foreach (var tag in lstTags)
                    {
                        foreach (var item in items)
                        {
                            if (tag.Name.ToLower() == item.Name.ToLower())
                            {
                                tag.ItemId = item.ID.ToString();
                                tag.Available = true;
                            }
                        }

                        if (!tag.Available && !string.IsNullOrEmpty(tag.Name))
                        {
                            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                            using (new SecurityDisabler())
                            {
                                Item parentItem = dbContext.Items[TagRoots.LocationTagsRoot];
                                TemplateItem template = dbContext.GetTemplate(Templates.Tag);

                                Item newItem = parentItem.Add(tag.Name.ToLower(), template);
                                if (newItem != null)
                                {
                                    newItem.Editing.BeginEdit();
                                    newItem.Appearance.DisplayName = ti.ToTitleCase(tag.Name.Replace("-", " "));
                                    newItem.Editing.EndEdit();
                                }
                                tag.ItemId = newItem.ID.ToString();
                                tag.Available = true;
                            }
                        }
                    }

                    string tagvalues = string.Empty;

                    if (lstTags.Count > 0)
                    {
                        foreach (var tag in lstTags)
                        {
                            tagvalues = tagvalues + tag.ItemId + "|";
                        }

                        value = tagvalues.Remove(tagvalues.Length - 1, 1);
                    }
                }
                else
                {
                    Loggers.Logger log = new Loggers.Logger();

                    object ItemId = null;
                    object ItemName = null;

                    model[Language].TryGetValue("ItemID", out ItemId);
                    model[Language].TryGetValue("ItemName", out ItemName);

                    log.Error(string.Format("{0} - {1}, Invalid tag : {2}", ItemId, ItemName, value));

                    value = string.Empty;
                }
            }

            model[Language][Field] = value;

            return true;
        }
    }
}
