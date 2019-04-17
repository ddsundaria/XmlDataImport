using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;


namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class ImageValueAccessorConverterML : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{3A7ED110-5FFB-42A7-A784-93105F21252F}");

        public ImageValueAccessorConverterML(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            var accessor = base.Convert(source);
            ItemModel referenceAsModel = base.GetReferenceAsModel(source, "Field");
            string language = this.GetLanguage(source);

            accessor.ValueWriter = new ImageValueWriterML(base.GetStringValue(referenceAsModel, ItemModel.ItemName), language)
            {
                Field = GetStringValue(referenceAsModel, ItemModel.ItemName)
            };

            return accessor;
        }

        protected virtual string GetLanguage(ItemModel source)
        {
            return this.GetStringValue(source, "Languages");
        }
    }
}
