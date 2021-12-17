using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkeD365.BotToVisio
{
    internal class ConditionShape:BaseShape
    {

        public Route Route { get; private set; }
        internal ConditionShape(Route route, BaseShape parentShape, int current, int children) : base()
        {
            Route = route;
            Shape = new System.Xml.Linq.XElement(GetTemplateShape("Condition"));
            AddType("Condition");
            Utils.ActionCount++;
            Current = current;
            Children = children;
            ParentShape = parentShape;
            
            CalcPosition();
            AddLine();
            AddText(Route.Expression);
        }

        public ConditionShape(BaseShape parentShape, int childCount)
        {
            Shape = new System.Xml.Linq.XElement(GetTemplateShape("Condition"));
            AddType("Condition");
            Utils.ActionCount++;
            Current = childCount;
            Children = childCount;
            ParentShape = parentShape;

            CalcPosition();
            AddLine();
            AddText("All other conditions");
        }
    }
}
