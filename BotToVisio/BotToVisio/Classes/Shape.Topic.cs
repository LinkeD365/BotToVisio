using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public class TopicShape : BaseShape
    {
        public JToken TopicObject { get; private set; }

        internal TopicShape(JToken topicObject, BaseShape parentShape, int current, int children): base()
        {
            TopicObject = topicObject;
            ParentShape = parentShape;
            Current = current;
            Children = children;
            Shape = new XElement(GetTemplateShape("Topic"));
            Guid = TopicObject["id"].ToString();
            Utils.ActionCount++;
            CalcPosition();
            AddLine();
            AddType("Call Topic");

            AddText();
        }

        private void AddText()
        {
            AddText(Utils.Topics.First(top => top.SchemaName == TopicObject["targetDialogId"].ToString()).Name);
        }
    }
}
