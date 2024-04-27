using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace LinkeD365.BotToVisio
{
    public class Message
    {
        public string Id { get; set; }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                foreach (Variable variable in Utils.Variables.Where(vr => value.Contains($"{{{vr.Id}}}")))
                {
                    _text = _text.Replace(variable.Id, variable.Name);
                }
            }
        }
    }

    public class Variable
    {
        public string Name { get; set; }

        public string Id { get; set; }
        public string ActionId { get; private set; }
        public string Type { get; set; }

        public string MessageId { get; set; }

        public JObject VarNode { get; private set; }

        public List<string> Choices { get; set; } = new List<string>();
        public Variable()
        { }

        public Variable(JObject varNode)
        {
            VarNode = varNode;

            Name = VarNode["displayName"]?.ToString()??string.Empty;
            Id = VarNode["id"].ToString();
            ActionId = VarNode["actionId"]?.ToString() ?? string.Empty;
            Type = VarNode["actionId"] != null ? "Action" : VarNode["systemType"]?.ToString() ?? VarNode["systemVariableTypes"]?.First().ToString() ?? Utils.CustomTypes.First(ct => ct.Id == VarNode["customTypeId"].ToString()).Name;
            MessageId = VarNode["messages"]?["getValue"]?.First().ToString() ?? string.Empty;
            Choices.AddRange(VarNode["namedEntityOptions"]?.Select(jt => jt.ToString()) ?? new string[] { });
        }
    }

    public class NamedEntity
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public JObject NENode { get; private set; }
        public NamedEntity()
        { }

        public NamedEntity(JObject neNode)
        {
            NENode = neNode;

            Name = NENode["displayName"].ToString();
            Id = NENode["id"].ToString();

        }
    }

    public class CustomType
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public JObject CTNode { get; set; }
        public List<NamedEntity> Entities { get; set; }

        public CustomType() { }
        public CustomType(JObject ctNode)
        {
            CTNode = ctNode;
            Name = CTNode["closedListTypes"].First()["displayName"].ToString();
            Id = CTNode["closedListTypes"].First()["id"].ToString();

            Entities = CTNode["namedEntities"].Select(ne => new NamedEntity((JObject)ne)).ToList();
        }
    }

    public class Action
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public Action(JObject actionNode)
        {
            Name = actionNode["displayName"].ToString();
            Id = actionNode["id"].ToString();
        }
    }
}
