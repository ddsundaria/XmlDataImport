﻿using Sitecore.DataExchange.Converters.DataAccess.ValueAccessors;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
using Sitecore.DataExchange.DataAccess;
using SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer;

namespace SaudiAramco.Foundation.XmlDataImport.Converters
{
    public class TagValueAccessorConverter : ValueAccessorConverter
    {
        private static readonly Guid TemplateId = Guid.Parse("{6D258015-FFAD-4392-807D-7B56478B60E0}");

        public TagValueAccessorConverter(IItemModelRepository repository) : base(repository)
        {
            this.SupportedTemplateIds.Add(TemplateId);
        }

        public override IValueAccessor Convert(ItemModel source)
        {
            var accessor = base.Convert(source);
            ItemModel field = base.GetReferenceAsModel(source, "Field");
            var filter = base.GetStringValue(source, "Filter");
            var tagRoot = base.GetReferenceAsModel(source, "Tag Repository Root");

            accessor.ValueWriter = new TagValueWriter(base.GetStringValue(field, ItemModel.ItemName))
            {
                Field = GetStringValue(field, ItemModel.ItemName),
                Filter = filter,
                TagRoot = GetGuidValue(tagRoot, "ItemID")
            };

            return accessor;
        }
    }
}
