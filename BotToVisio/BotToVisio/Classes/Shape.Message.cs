using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace LinkeD365.BotToVisio
{
    internal class MessageShape : BaseShape
    {
        public JToken MessageObject { get; private set; }

        internal MessageShape(JToken messageObject, BaseShape parentShape, int current, int children) : base()
        {
            Shape = new System.Xml.Linq.XElement(GetTemplateShape("Message"));
            AddType("Message");
            MessageObject = messageObject;
            Utils.ActionCount++;
            Guid = MessageObject["id"].ToString();
            Current = current;
            Children = children;
            ParentShape = parentShape;

            CalcPosition();
            AddLine();

            AddText();
        }

        private void AddText()
        {

            AddMessageText(MessageObject["botMessageId"].ToString());
        }
    }
}
