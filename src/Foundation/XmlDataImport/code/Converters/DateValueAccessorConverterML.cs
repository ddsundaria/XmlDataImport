using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;


namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class DateValueAccessorConverterML : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{56C8BC26-F8CA-4826-B1FB-7E760AF76D9F}");

        public DateValueAccessorConverterML(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            IValueAccessor valueAccessor = base.Convert(source);
            if (valueAccessor == null)
                return (IValueAccessor)null;
            ItemModel referenceAsModel = this.GetReferenceAsModel(source, "Field");
            if (referenceAsModel == null)
                return (IValueAccessor)null;
            string fieldName = referenceAsModel["ItemName"] as string;
            if (string.IsNullOrWhiteSpace(fieldName))
                return (IValueAccessor)null;
            string language = this.GetLanguage(source);

            valueAccessor.ValueWriter = new DateValueWriterML(fieldName, language)
            {
                Field = GetStringValue(referenceAsModel, ItemModel.ItemName)
            };

            return valueAccessor;
        }

        protected virtual string GetLanguage(ItemModel source)
        {
            return this.GetStringValue(source, "Languages");
        }

    }
}
