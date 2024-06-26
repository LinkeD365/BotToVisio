﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LinkeD365.BotToVisio
{
    public abstract class BaseShape
    {

        protected double offsetX = 1.5; // inches
        protected double offsetY = 1.1;
        public JProperty Property { get; private set; }
        //protected XDocument xmlPage;

        protected XElement shapes;

        private XElement line;
        public double PinX { get; protected set; }
        public double PinY { get; protected set; }
        public int Id { get; protected set; }
        public string Guid { get; protected set; }

        public XElement GetTemplateShape(string name)
        {
            var selectedElements =
                from el in Shapes.Elements()
                where el.Attribute("NameU")?.Value == name
                select el;
            return selectedElements.DefaultIfEmpty(null).FirstOrDefault();
        }

        public XElement Line
        {
            get
            {
                if (line == null)
                {
                    var selectedElements =
                        from el in Shapes.Elements()
                        where el.Attribute("ID").Value == "35"
                        select el;
                    line = selectedElements.DefaultIfEmpty(null).FirstOrDefault();
                }

                return line;
            }
        }

        public XElement Shapes
        {
            get
            {
                if (shapes == null)
                {
                    var elements =
                        from element in Utils.XmlPage.Descendants()
                        where element.Name.LocalName == "Shapes"
                        select element;
                    // Return the selected elements to the calling code.
                    shapes = elements.FirstOrDefault();
                }

                return shapes;
            }
        }

        public string Name
        {
            get { if (Property == null) return "Line." + Id; else return Property.Name; }
        }

        public BaseShape(JProperty property)
        {
            Property = property;
        }

        public BaseShape()
        {
        }

        private XElement shape;
        protected int Children;
        protected int Current;
        private XElement sections;

        public XElement Shape
        {
            get => shape;
            set
            {
                shape = value;
                Shapes.Add(shape);
                SetId();
            }
        }

        public string PropertyName
        {
            get
            {
                if (Property == null) return string.Empty;
                string returnstring = Property.Name.Replace("__", "LiNkEd365").Replace("_", " ").Replace("LiNkEd365", "_").Replace("'", "&apos;");
                //var regext = new Regex("/(_{2,})|_/g, '$1'");
                //    string returnstring = regext.Replace(Property.Name, " ").Replace("'", "&apos;");
                return returnstring;
            }
        }

        private void SetId()
        {
            if (Shape.Attribute("ID") == null) return;
            Id = Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1;

            Shape.SetAttributeValue("ID", Id);

            foreach (var stencil in Shape.Descendants().Where(el => el.Attribute("ID") != null))
                stencil.SetAttributeValue("ID",
                    Shapes.Descendants().Where(el => el.Attribute("ID") != null).Max(x => int.Parse(x.Attribute("ID").Value)) + 1);
            //if (Shape.Elements().Any(el => el.Name.LocalName == "Shapes"))
            //{
            //    foreach(var stencilShape in Shape.Elements().Where(el => el.Name.LocalName == Shapes).)
            //}
        }

        public BaseShape ParentShape { get; protected set; }

        private XElement props;

        public XElement Props
        {
            get
            {
                if (props == null)
                {
                    var elements =
                        from element in Shape.Descendants()
                        where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Property"
                        select element;
                    if (!elements.Any())
                    {
                        props = new XElement("Section");
                        props.SetAttributeValue("N", "Property");
                        Shape.Add(props);
                    }
                    else
                        props = elements.FirstOrDefault();
                }

                return props;
            }
        }
        public XElement Connections
        {
            get
            {
                if (sections == null)
                {
                    var elements =
                        from element in Shape.Descendants()
                        where element.Name.LocalName == "Section" && element.Attribute("N").Value == "Connection"
                        select element;
                    if (!elements.Any())
                    {
                        sections = new XElement("Section");
                        sections.SetAttributeValue("N", "Connection");
                        Shape.Add(sections);
                    }
                    else
                        sections = elements.FirstOrDefault();
                }

                return sections;
            }
        }

        protected void CalcPosition()
        {
            PinY = ParentShape.PinY -
                   offsetY;
            PinX = ParentShape.PinX +
                   CalcX;
            SetPosition();
        }

        protected void SetPosition()
        {
            Shape.Elements().First(el => el.Attribute("N").Value == "PinY").SetAttributeValue("V", PinY);
            Shape.Elements().First(el => el.Attribute("N").Value == "PinX").SetAttributeValue("V", PinX);
        }
        protected double CalcX
        {
            get
            {
                if (Children == 1) return 0;
                double width = Children + 1;
                return (-(width / 2) + (double)Current / (Children + 1) * width) * offsetX;
            }
        }

        protected void AddLine()
        {
            var line = new Line();
            line.Connect(ParentShape, this, Current, Children);
        }

        public void AddLine(BaseShape child)
        {
            var line = new Line();
            line.Connect(child, this, 1, 1);
        }

        protected void AddProp(string name, string value)
        {
            //  if (Props.ha)
            var element = Props.Elements().FirstOrDefault(el => el.Attributes().Any(at => at.Name == "N" && at.Value == name));
            if (element != null) element.ReplaceWith(XElement.Parse("<Row N='" + name + "'> <Cell N='Value' V='" + value + "' U='STR'/></Row>"));
            else Props.Add(XElement.Parse("<Row N='" + name + "'> <Cell N='Value' V='" + value + "' U='STR'/></Row>"));
        }

        protected void AddType(string value)
        {
            AddProp("ActionType", value);
        }

        protected void AddText(string value)
        {
            var textElement = Shape.Descendants().Where(el => el.Name.LocalName == "Text").First();

            textElement.ReplaceWith(XElement.Parse($"<Text><![CDATA[{value}]]></Text>", LoadOptions.SetBaseUri));
        }

        protected void AddMessageText(string messageId)
        {
            Message message = Utils.Messages.First(msg => msg.Id == messageId);

            AddText(message.Text);
        }
    }
}
