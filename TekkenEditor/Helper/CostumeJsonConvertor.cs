using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekkenEditor.Helper
{
    class CostumeJsonConvertor : JsonConverter
    {

    private readonly Type[] _types;
 
     public CostumeJsonConvertor(params Type[] types)
     {
         _types = types;
     }
 
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JToken t = JToken.FromObject(value);

        if (t.Type != JTokenType.Object)
        {
            t.WriteTo(writer);
        }
        else
        {
                
                JObject o = (JObject)t;
                
                Bitmap thumbnail = ((CharacterCostume)value).Thumbnail;
                MemoryStream memoryStream = new MemoryStream();
                thumbnail.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                o.Add("Image", JToken.FromObject(memoryStream.ToArray(), serializer));
                o.WriteTo(writer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

            JObject o = JObject.Load(reader);


            reader = o.CreateReader();
            CharacterCostume target = new CharacterCostume();

            serializer.Populate(reader, target);


            if ((o.GetValue("Image")) != null)
            {
                byte[] b = (o.GetValue("Image")).ToObject<byte[]>();
                MemoryStream memoryStream = new MemoryStream(b);
                memoryStream.Position = 0;
                target.Thumbnail = new Bitmap(memoryStream);
            }

            return target;
        }

    public override bool CanRead
    {
        get { return true; }
    }

    public override bool CanConvert(Type objectType)
    {
        return _types.Any(t => t == objectType);
    }
}

}
