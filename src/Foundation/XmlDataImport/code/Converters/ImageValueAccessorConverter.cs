using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;


namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class ImageValueAccessorConverter : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{3A7ED110-5FFB-42A7-A784-93105F21252F}");

        public ImageValueAccessorConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            var accessor = base.Convert(source);
            ItemModel referenceAsModel = base.GetReferenceAsModel(source, "Field");

            accessor.ValueWriter = new ImageValueWriter(base.GetStringValue(referenceAsModel, ItemModel.ItemName))
            {
                Field = GetStringValue(referenceAsModel, ItemModel.ItemName)
            };

            return accessor;
        }

    }
}
