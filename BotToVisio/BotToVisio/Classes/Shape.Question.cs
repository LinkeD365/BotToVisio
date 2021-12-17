using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    internal class QuestionShape : BaseShape
    {
        public JToken QuestionObject { get; private set; }

        internal QuestionShape(JToken questObject, BaseShape parentShape, int current, int children) : base()
        {
            Shape = new XElement(GetTemplateShape("Message"));
            AddType("Question");
            QuestionObject = questObject;
            Utils.ActionCount++;
            Guid = QuestionObject["id"].ToString();
            Current = current;
            Children = children;
            ParentShape = parentShape;
            AddText();
            CalcPosition();
            AddLine();
        }

        private void AddText()
        {
            var variable = Utils.Variables.First(vr => vr.Id == QuestionObject["contextVariableId"].ToString());
            Message message = Utils.Messages.First(msg => msg.Id == variable.MessageId);
            var sb = new StringBuilder();
            sb.AppendLine(message.Text);
            sb.AppendLine($"Var Name: {variable.Name}");
            sb.AppendLine($"Type: {variable.Type}");
            if (variable.Choices.Any()) sb.AppendLine("Choices:");
            variable.Choices.ForEach(choice => sb.AppendLine(Utils.NamedEntities.First(ne => ne.Id == choice).Name));

            AddText(sb.ToString());
        }


    }
}
