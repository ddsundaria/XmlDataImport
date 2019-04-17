using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;


namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class DateValueAccessorConverter : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{56C8BC26-F8CA-4826-B1FB-7E760AF76D9F}");

        public DateValueAccessorConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            var accessor = base.Convert(source);
            ItemModel referenceAsModel = base.GetReferenceAsModel(source, "Field");

            accessor.ValueWriter = new DateValueWriter(base.GetStringValue(referenceAsModel, ItemModel.ItemName))
            {
                Field = GetStringValue(referenceAsModel, ItemModel.ItemName)
            };

            return accessor;
        }

    }
}
