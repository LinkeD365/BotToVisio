using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkeD365.BotToVisio
{
    public abstract class Node
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public JObject NodeDef { get; protected set; }

        public string Target { get; protected set; }

        public List<Route> Routes = new List<Route>();
    }

    public class MessageNode : Node
    {
        public MessageNode(JObject msgNode)
        {
            NodeDef = msgNode;
            Id = NodeDef["id"].ToString();
            Target = NodeDef["defaultTargetNodeId"]?.ToString() ?? string.Empty;
            if (NodeDef["routes"] != null) Routes.AddRange(((JArray)NodeDef["routes"])?.Select(rte => new Route((JObject)rte)) ?? null);
        }

    }

    public class QuestionNode : Node
    {
        public QuestionNode(JObject questNode)
        {
            NodeDef = questNode;
            Id = NodeDef["id"].ToString();
            Target = NodeDef["defaultTargetNodeId"]?.ToString() ?? string.Empty;
            if (NodeDef["routes"] != null) Routes.AddRange(((JArray)NodeDef["routes"])?.Select(rte => new Route((JObject)rte)) ?? null);

        }

    }

    public class Route : Node
    {
        public Route(JObject nodeDef)
        {
            NodeDef = nodeDef;
            Expression = NodeDef["expression"].ToString();
            Target = NodeDef["targetNodeId"]?.ToString() ?? String.Empty;

        }
        private String _expression;
        public string Expression
        {
            get { return _expression; }
            private set
            {
                _expression = value;
                string[] splits = _expression.Split(new string[] { ", " }, StringSplitOptions.None);
                string expression = string.Empty;
                for (int i = 0; i < splits.Count(); i += 2)
                {
                    expression += Environment.NewLine + "AND" + Environment.NewLine + CreateExp(splits[i], splits[i + 1]);
                }
                _expression = expression.Substring(7, expression.Length -7);
            }
        }

        private string CreateExp(string exp1, string exp2)
        {
            if (exp1.StartsWith("@and(")) exp1 = exp1.Substring(5, exp1.Length - 5);
            else exp1 = exp1.TrimStart('@');
            var variable = Utils.Variables.FirstOrDefault(v => exp1.Contains(v.Id));
            if (variable == null) return string.Empty;
            string rightSide = exp2.TrimEnd(')');
            if (!decimal.TryParse(rightSide, out var decimalValue)) // string
            {
                rightSide = Utils.NamedEntities.FirstOrDefault(ne => rightSide.Contains(ne.Id))?.Name ?? rightSide;
            }

            string returnString = string.Empty;
            switch (exp1)
            {
                case string s when s.StartsWith("lessOrEquals"):
                    returnString = $"{variable.Name} <= {rightSide}";
                    break;
                case string s when s.StartsWith("less"):
                    returnString = $"{variable.Name} < {rightSide}";
                    break;
                case String s when s.StartsWith("greater"):
                    returnString = $"{variable.Name} > {rightSide}";
                    break;
                case String s when s.StartsWith("equals"):
                    returnString = $"{variable.Name} = {rightSide}";
                    break;
                case String s when s.StartsWith("greaterOrEquals"):
                    returnString = $"{variable.Name} >= {rightSide}";
                    break;
                case String s when s.StartsWith("or(equals"):
                    returnString = $"{variable.Name} is empty";
                    break;
                case String s when s.StartsWith("not(or(equals"):
                    returnString = $"{variable.Name} is empty";
                    break;
            }
            return returnString;
        }
    }

    public class DialogChangeNode : Node
    {
        public string TargetTopic { get; set; }
        public DialogChangeNode(JObject dlgChgNode)
        {
            NodeDef = dlgChgNode;
            Id = NodeDef["id"].ToString();
            Target = String.Empty;
            TargetTopic = NodeDef["targetDialogId"].ToString();
        }
    }

    public class ActionNode : Node
    {
        public ActionNode(JObject actionObj)
        {
            NodeDef = actionObj;
            Id = NodeDef["id"].ToString();
            Target = NodeDef["defaultTargetNodeId"]?.ToString() ?? String.Empty;
        }
    }
}
