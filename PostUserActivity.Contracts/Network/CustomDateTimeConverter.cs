using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PostUserActivity.Contracts.Network
{
    public class CustomDateTimeConverter : DateTimeConverterBase
    {
        //private const string DatePattern = "yyyy-MM-dd HH:mm:ss";
        private const string DatePattern = "dd.MM.yyyy HH:mm:ss";
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //dd.mm.yyyy hh24:mi:ss
            //writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));//.ToString("yyyy-MM-dd"));
            writer.WriteValue(((DateTime)value).ToString(DatePattern));//.ToString("yyyy-MM-dd"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTime.ParseExact(reader.Value.ToString(), DatePattern, CultureInfo.InvariantCulture);
        }

        #endregion
    }

    //  : JsonConverter
    public class ArmDataPackageTypeStringEnumConverter : StringEnumConverter
    {
        public ArmDataPackageTypeStringEnumConverter()
            : base(true)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }

        #region Overrides of StringEnumConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();

            if (typeof(ArmDataPackageType) == type)
            {
                var val = (ArmDataPackageType)value;
                switch (val)
                {
                    case ArmDataPackageType.FullFrame:
                        writer.WriteValue("fullframe");
                        break;
                    case ArmDataPackageType.Preview:
                        writer.WriteValue("preview");
                        break;
                    case ArmDataPackageType.Log:
                        writer.WriteValue("log");
                        break;
                }
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }            
        }

        #endregion
    }

}
