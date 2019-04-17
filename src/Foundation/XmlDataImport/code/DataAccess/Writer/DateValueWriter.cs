using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.DataAccess;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer
{
    public class DateValueWriter : IValueWriter
    {
        public string Field { get; set; }
        public DateValueWriter(string fieldName) : base()
        {
            Field = fieldName;
        }

        public CanWriteResult CanWrite(object target, object value, DataAccessContext context)
        {
            return CanWriteResult.PositiveResult(true);
        }

        public bool Write(object target, object value, DataAccessContext context)
        {
            //Dictionary<string, ItemModel> model = (Dictionary<string,ItemModel>)target;
            ItemModel model = (ItemModel)target;
            if (model == null)
                return false;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            if (value != null)
            {
                string dateValue = ((string)value).Remove(0, 6);
                DateTime dt2 = Sitecore.DateUtil.ParseDateTime(dateValue, DateTime.UtcNow, CultureInfo.InvariantCulture);
                dt2 = Sitecore.Common.DateTimeExtensions.SpecifyKind(dt2, DateTimeKind.Utc);
                value = Sitecore.DateUtil.ToIsoDate(dt2);
            }

            model[Field] = value;

            return true;
        }
    }
}
