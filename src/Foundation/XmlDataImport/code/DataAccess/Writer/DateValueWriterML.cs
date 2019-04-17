using Sitecore.Services.Core.Model;
using Sitecore.DataExchange.DataAccess;
using System;
using System.Globalization;
using System.Collections.Generic;
using Sitecore.DataExchange.DataAccess.Writers;

namespace SaudiAramco.Foundation.XmlDataImport.DataAccess.Writer
{
    public class DateValueWriterML : IValueWriter
    {
        public string Field { get; set; }
        string Language { set; get; }
        public DateValueWriterML(string fieldName, string language) : base()
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

            if (value != null)
            {
                string dateValue = ((string)value).Remove(0, 6);
                DateTime dt2 = Sitecore.DateUtil.ParseDateTime(dateValue, DateTime.UtcNow, CultureInfo.InvariantCulture);
                dt2 = Sitecore.Common.DateTimeExtensions.SpecifyKind(dt2, DateTimeKind.Utc);
                value = Sitecore.DateUtil.ToIsoDate(dt2);
            }
          
            if (model.ContainsKey(Language) && model[Language].ContainsKey(Field))
            {
                model[Language][Field] = value;
            }
            else
            {
                model[Language].Add(Field, value);
            }

            return true;
        }
    }
}
