using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public class LowerCasePropertyNameJsonReader : JsonTextReader
    {
        public LowerCasePropertyNameJsonReader(TextReader textReader)
            : base(textReader)
        {
        }

        public override object Value
        {
            get
            {
                if (TokenType == JsonToken.PropertyName)
                {
                    var name = (string)base.Value;
                    return char.ToLowerInvariant(name[0]) + name.Substring(1);
                    // return ((string)base.Value).ToLower();
                }
                return base.Value;
                
            }
        }
    }

    public static class JsonHelper
    {
        public static JObject ParseLower(string json)
        {
            using (TextReader textReader = new StringReader(json))
            using (JsonReader jsonReader = new LowerCasePropertyNameJsonReader(textReader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<JObject>(jsonReader);
            }
        }
    }
}
