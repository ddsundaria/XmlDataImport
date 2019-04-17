using SaudiAramco.Foundation.XmlDataImport.Models;
using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Reader;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;

namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class XmlValueAccessorConverter : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{33211C52-A503-4E5A-8D00-0E7CDF0B844D}");

        public XmlValueAccessorConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            var accessor = base.Convert(source);
            if (accessor == null)
            {
                return null;
            }
            var path = base.GetStringValue(source, XmlValueAccessorItemModel.XPath);
            var isAttribute = base.GetBoolValue(source, XmlValueAccessorItemModel.IsAttribute);
            
            //
            //unless a reader or writer is explicitly set use the property value
            if (accessor.ValueReader == null)
            {
                accessor.ValueReader = new XmlValueReader(path, isAttribute);
            }
            if (accessor.ValueWriter == null)
            {
                accessor.ValueWriter = new XmlValueWriter(path, isAttribute);
            }
            return accessor;
        }
    }
}
