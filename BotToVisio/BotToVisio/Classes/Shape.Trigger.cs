using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public class Trigger : BaseShape
    {
        public JObject TriggerObject { get; private set; }
        public Trigger(JObject property) : base()
        {
            Shape = new XElement(GetTemplateShape("Trigger"));
            TriggerObject = property;
            Utils.ActionCount++;
            string triggers = TriggerObject["triggerQueries"] == null ? string.Empty : string.Join(Environment.NewLine, ((JArray)TriggerObject["triggerQueries"]).Select(trig => trig.ToString()));
            AddText(triggers);

            PinX = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinX").Attribute("V").Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var tempPinX) ? tempPinX : 0.0;
            PinY = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinY").Attribute("V").Value, NumberStyles.Any,
                CultureInfo.InvariantCulture, out var tempPiny) ? tempPiny : 0.0;
        }
    }
}
