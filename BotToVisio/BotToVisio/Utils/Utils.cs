using LinkeD365.BotToVisio.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public static partial class Utils
    {
        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";

        private const string aiKey = "cc383234-dfdb-429a-a970-d17847361df3";
        private static AppInsights ai;

        public static AppInsights Ai
        {
            get
            {
                if (ai == null)
                {
                    ai = new AppInsights(aiEndpoint, aiKey, Assembly.GetExecutingAssembly());
                    //ai.WriteEvent("Control Loaded");
                }
                return ai;
            }
        }
        private static Package templatePackage;

        private static PackagePart templateDocument;

        private static PackagePart templatePages;
        private static PackagePart templatePage;

        public static XDocument XmlPage;

        private static JObject pvaObject;

        public static int ActionCount { get; internal set; }
        private static List<Message> _messages;
        public static List<Message> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = ((JArray)pvaObject["botMessages"]).Select(msg => new Message()
                    {
                        Id = msg["id"].ToString(),
                        Text = msg["channelContent"]["web"]["content"].ToString()
                    }).ToList();
                }
                return _messages;
            }

        }

        private static List<Node> _nodes;
        private static XElement connects;

        public static List<Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<Node>();
                    _nodes.AddRange(pvaObject["dialogs"].First()["messageNodes"]?.Select(msgNode => new MessageNode((JObject)msgNode))?? new List<MessageNode>());
                    _nodes.AddRange(pvaObject["dialogs"].First()["questionNodes"]?.Select(questNode => new QuestionNode((JObject)questNode)) ?? new List<QuestionNode>());
                    _nodes.AddRange(pvaObject["dialogs"].First()["dialogChangeNodes"]?.Select(dlgChgNode => new DialogChangeNode((JObject)dlgChgNode)) ?? new List<DialogChangeNode>());
                    _nodes.AddRange(pvaObject["dialogs"].First()["actionNodes"]?.Select(actNode => new ActionNode((JObject)actNode)) ?? new List<ActionNode>());

                }
                return _nodes;

            }
        }
        private static List<Variable> _variables;
        public static List<Variable> Variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = new List<Variable>();
                    _variables.AddRange(pvaObject["contextVariables"]?.Select(vr => new Variable((JObject)vr))??new List<Variable>());

                }
                return _variables;

            }
        }

        private static List<NamedEntity> _namedEntities;
        public static List<NamedEntity> NamedEntities
        {
            get
            {
                if (_namedEntities == null)
                {
                    _namedEntities = new List<NamedEntity>();
                    if (pvaObject["namedEntities"] != null) _namedEntities.AddRange(pvaObject["namedEntities"].Select(ne => new NamedEntity((JObject)ne)));
                    _namedEntities.AddRange(CustomTypes.SelectMany(ct => ct.Entities));

                }
                return _namedEntities;

            }
        }

        public static List<CustomType> CustomTypes { get; set; }

        public static List<Topic> Topics { get; set; }

        private static List<Action> _actions;
        public static List<Action> Actions
        {
            get
            {
                if(_actions == null)
                {
                    _actions = new List<Action>();
                    _actions.AddRange(pvaObject["actionDefinitions"]?.Select(ao => new Action((JObject)ao)) ?? new List<Action>());
                }
                return _actions;
            }
        }

        public static XElement Connects
        {
            get
            {
                if (connects == null)
                {
                    IEnumerable<XElement> elements =
                      from element in XmlPage.Descendants()
                      where element.Name.LocalName == "Connects"
                      select element;
                    if (!elements.Any())
                    {
                        IEnumerable<XElement> pageContents =
                      from element in XmlPage.Descendants()
                      where element.Name.LocalName == "PageContents"
                      select element;
                        connects = new XElement("Connects");
                        pageContents.FirstOrDefault().Add(connects);
                    }
                    else
                    {
                        connects = elements.FirstOrDefault();
                    }
                }
                return connects;
            }
        }

        public static List<string> VisioTemplates = new List<string>() { "Condition", "Message", "Trigger", "ConnectorTemplate" };

        public static void CreateVisio(Topic topic, string fileName, int topicCount)
        {
            if (templatePackage == null)
            {

                File.WriteAllBytes(fileName, Resources.VisioTemplate_PVA);

                templatePackage = Package.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                templateDocument = GetPackagePart(templatePackage, "http://schemas.microsoft.com/visio/2010/relationships/document");
                templatePages = GetPackagePart(templatePackage, templateDocument, "http://schemas.microsoft.com/visio/2010/relationships/pages");
                templatePage = GetPackagePart(templatePackage, templatePages, "http://schemas.microsoft.com/visio/2010/relationships/page");
            }
            connects = null;
            _nodes = null;
            _variables = null;
            _namedEntities = null;
            _messages = null;
            _actions = null;
            XmlPage = GetXMLFromPart(templatePage);

            pvaObject = JObject.Parse(topic.Canvas);
            Shapes = new List<BaseShape>();


            var trigger = CreateTrigger();
            Shapes.Add(trigger);

            var rootNode = Nodes.First(node => node.Id == pvaObject["dialogs"].First()["rootNodeId"].ToString());

            CreateNode(rootNode, trigger, 1, 1);

            RemoveTemplateShapes();

            CreateNewPage(topic.Name, topicCount);
        }

        private static void RemoveTemplateShapes()
        {
            foreach (var shapeName in Utils.VisioTemplates)
                Shapes.First().GetTemplateShape(shapeName).Remove();
        }

        private static Trigger CreateTrigger()
        {
            var trigger = new Trigger((JObject)pvaObject["intents"].First());
            return trigger;
        }

        internal static void CompleteVisio(string fileName)
        {
            RemoveTemplate();
            RecalcDocument(templatePackage);

            templatePackage.Close();
            templatePackage = null;
        }
    }
}
