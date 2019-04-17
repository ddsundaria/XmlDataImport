using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SaudiAramco.Foundation.XmlDataImport.Extensions
{
    public static class ItemRendering
    {
        private static Database dbContext = Sitecore.Configuration.Factory.GetDatabase("master");
        private static Item localDatasource { set; get; }
        public static void AddRenderings(Item parentItem)
        {
            if (parentItem == null)
            {
                return;
            }

            if (parentItem.Children.Count == 0)
            {
                return;
            }
            else
            {
                localDatasource = parentItem.Children.FirstOrDefault();
            }

            if (localDatasource.Children.Count == 0)
            {
                return;
            }

            ResetRenderings(parentItem);

            // Get the default device
            DeviceRecords devices = parentItem.Database.Resources.Devices;
            DeviceItem defaultDevice = devices.GetAll().Where(d => d.Name.ToLower() == "default").First();

            var renderingsList = parentItem.Visualization.GetRenderings(defaultDevice, true);

            if (renderingsList == null)
            {
                return;
            }

            LayoutField layoutField = new LayoutField(parentItem.Fields[FieldIDs.FinalLayoutField]);
            LayoutDefinition layoutDefinition = LayoutDefinition.Parse(layoutField.Value);
            DeviceDefinition deviceDefinition = layoutDefinition.GetDevice(defaultDevice.ID.ToString());

            foreach (Item child in localDatasource.Children)
            {
                if (child.TemplateID.ToString() == NewsChild.Text)
                {
                    Guid newGuid = Guid.NewGuid();
                    string renderingUniqueID = newGuid.ToString().TrimStart('{').TrimEnd('}').ToLower();

                    if (layoutDefinition.Devices.Count > 0)
                    {
                        if (!RenderingAvailable(layoutDefinition, Renderings.BodyContent_Rendering))
                        {
                            RenderingDefinition textSubLayout = new RenderingDefinition();

                            textSubLayout.ItemID = Renderings.BodyContent_Rendering;
                            textSubLayout.Placeholder = "page-layout";
                            textSubLayout.UniqueId = renderingUniqueID;
                            deviceDefinition.AddRendering(textSubLayout);
                        }
                        else
                        {
                            var rendering = deviceDefinition.GetRendering(Renderings.BodyContent_Rendering);
                            renderingUniqueID = rendering.UniqueId;
                            renderingUniqueID = renderingUniqueID.TrimStart('{').TrimEnd('}').ToLower();
                        }
                    }

                    RenderingDefinition newRenderingDefinition = new RenderingDefinition();
                    newRenderingDefinition.ItemID = Renderings.Text;
                    string placeholderValue = !string.IsNullOrEmpty(renderingUniqueID) ? "/page-layout/body-content_" + renderingUniqueID : "/page-layout/body-content";
                    newRenderingDefinition.Placeholder = placeholderValue;
                    newRenderingDefinition.Datasource = child.ID.ToString();

                    deviceDefinition.AddRendering(newRenderingDefinition);
                }
                else if (child.TemplateID.ToString() == NewsChild.Quote)
                {
                    Guid newGuid = Guid.NewGuid();
                    string renderingUniqueID = newGuid.ToString().TrimStart('{').TrimEnd('}').ToLower();

                    if (layoutDefinition.Devices.Count > 0)
                    {
                        if (!RenderingAvailable(layoutDefinition, Renderings.BodyContent_Rendering))
                        {
                            RenderingDefinition quoteSubLayout = new RenderingDefinition();
                            quoteSubLayout.ItemID = Renderings.BodyContent_Rendering;
                            quoteSubLayout.Placeholder = "page-layout";
                            quoteSubLayout.UniqueId = renderingUniqueID;

                            deviceDefinition.AddRendering(quoteSubLayout);
                        }
                        else
                        {
                            var rendering = deviceDefinition.GetRendering(Renderings.BodyContent_Rendering);
                            renderingUniqueID = rendering.UniqueId;
                            renderingUniqueID = renderingUniqueID.TrimStart('{').TrimEnd('}').ToLower();
                        }
                    }

                    RenderingDefinition newRenderingDefinition = new RenderingDefinition();
                    newRenderingDefinition.ItemID = Renderings.Quote;

                    string placeholderValue = !string.IsNullOrEmpty(renderingUniqueID) ? "/page-layout/body-content_" + renderingUniqueID : "/page-layout/body-content";

                    newRenderingDefinition.Placeholder = placeholderValue;
                    newRenderingDefinition.Datasource = child.ID.ToString();

                    deviceDefinition.AddRendering(newRenderingDefinition);
                }
            }

            using (new SecurityDisabler())
            {
                parentItem.Editing.BeginEdit();
                if (parentItem.Editing.IsEditing)
                    parentItem["__Renderings"] = layoutDefinition.ToXml();
                parentItem.Editing.EndEdit();
            }
        }


        public static void ResetRenderings(Item item)
        {
            using (new SecurityDisabler())
            {
                if (!string.IsNullOrEmpty(item["__renderings"]))
                {
                    using (new EditContext(item))
                    {
                        item.Fields["__renderings"].Reset();
                    }
                }
            }
        }


        private static bool RenderingAvailable(LayoutDefinition layoutDefinition, string renderingId)
        {
            bool result = false;

            var renderings = ((Sitecore.Layouts.DeviceDefinition)(layoutDefinition.Devices[0])).Renderings;

            foreach (RenderingDefinition r in renderings)
            {
                if (r.ItemID == renderingId)
                    result = true;
            }

            return result;
        }

    }
}