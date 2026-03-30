//using Newtonsoft.Json;
//using RevitFamilyManager.Models.ModelAPI.ModelAPIWEB.MyFamilies;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Data;

//namespace UITheme.Converter
//{
//  public class ParamsNullConverter : JsonConverter<Params>
//  {
//    public override Params? ReadJson(JsonReader reader, Type objectType, Params? existingValue, bool hasExistingValue, JsonSerializer serializer)
//    {
//      if (reader.TokenType == JsonToken.Null)
//        return null;

//      if (reader.TokenType == JsonToken.String && ((string)reader.Value).Trim().ToLower() == "null")
//        return null;

//      return serializer.Deserialize<Params>(reader);
//    }

//    public override void WriteJson(JsonWriter writer, Params? value, JsonSerializer serializer)
//    {
//      serializer.Serialize(writer, value);
//    }
//  }
//}
