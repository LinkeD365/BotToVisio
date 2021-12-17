using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public class ActionShape : BaseShape
    {
        public JToken ActionObject { get; private set; }

        internal ActionShape(JToken actionObject, BaseShape parentShape, int current, int children) : base()
        {
            ActionObject = actionObject;
            ParentShape = parentShape;
            Current = current;
            Children = children;
            Shape = new XElement(GetTemplateShape("Topic"));
            Guid = ActionObject["id"].ToString();
            Utils.ActionCount++;
            CalcPosition();
            AddLine();
            AddType("Call Action");

            AddText();
        }

        private void AddText()
        {
            StringBuilder sb = new StringBuilder();
            if (ActionObject["inputParameterVariableIdMap"] != null)
            {
                sb.AppendLine($"{ActionObject["inputParameterVariableIdMap"].Count()} Input Parameter" + (ActionObject["inputParameterVariableIdMap"].Count() > 1 ? "s" : ""));
                foreach (JProperty inputParam in ActionObject["inputParameterVariableIdMap"])
                {
                    sb.AppendLine(inputParam.Name + " gets value from " + Utils.Variables.First(var => var.Id == inputParam.Value.ToString().Replace("{", string.Empty).Replace("}", string.Empty)).Name);
                }
            }
            sb.AppendLine("Flow: " + Utils.Actions.First(act => act.Id == 
                        Utils.Variables.First(var => var.Id == ActionObject["contextVariableId"].ToString()).ActionId).Name);

            if (ActionObject["outputExpressionVariableIds"] != null)
            {
                sb.AppendLine($"{ActionObject["outputExpressionVariableIds"].Count()} Output Parameter" + (ActionObject["outputExpressionVariableIds"].Count() > 1 ? "s" : ""));
                foreach (JToken outputParam in ActionObject["outputExpressionVariableIds"])
                {
                    sb.AppendLine("Gives value to " + Utils.Variables.First(var => var.Id == outputParam.ToString()).Name);
                }
            }

            AddText(sb.ToString());
            //sb.AppendLine(Utils.Actions.First)
            // AddText(Utils.Topics.First(top => top.SchemaName == ActionObject["targetDialogId"].ToString()).Name);
        }
    }
}
