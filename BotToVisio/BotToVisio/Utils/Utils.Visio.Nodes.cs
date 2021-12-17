using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkeD365.BotToVisio
{
    public static partial class Utils
    {
        public static List<BaseShape> Shapes { get; private set; } = new List<BaseShape>();

        private static void CreateNode(Node node, BaseShape parentShape, int current, int childCount)
        {
            BaseShape nodeShape;
            switch (node)
            {
                case MessageNode _:
                    nodeShape = new MessageShape(node.NodeDef, parentShape, current, childCount);
                    break;
                case QuestionNode _:
                    nodeShape = new QuestionShape(node.NodeDef, parentShape, current, childCount);
                    break;
                case DialogChangeNode _:
                    nodeShape = new TopicShape(node.NodeDef, parentShape, current, childCount);
                    break;
                case ActionNode _:
                    nodeShape = new ActionShape(node.NodeDef, parentShape, current, childCount);
                    break;
                default:
                    return;

            }

            Shapes.Add(nodeShape);

            if (node.Routes.Any())
            {
                int curChild = 1;
                foreach (var route in node.Routes)
                {
                    CreateCondition(route, nodeShape, curChild, node.Routes.Count + (node.Target == String.Empty ? 0 : 1));
                    curChild++;
                }
                if (node.Target != string.Empty) CreateCondition(node.Target, nodeShape, node.Routes.Count + 1);
            }
            else if (node.Target != string.Empty) AddChild(node, nodeShape);

        }

        private static void CreateCondition(string target, BaseShape nodeShape, int childCount)
        {
            var conditionShape = new ConditionShape(nodeShape, childCount);
            AddChild(target, conditionShape);
        }

        private static void AddChild(Node node, BaseShape nodeShape)
        {
            AddChild(node.Target, nodeShape);
        }

        private static void AddChild(string target, BaseShape nodeShape)
        {
            var existingShape = Shapes.FirstOrDefault(s => s.Guid == target);
            if (existingShape == null) CreateNode(Utils.Nodes.First(nd => nd.Id == target), nodeShape, 1, 1);
            else existingShape.AddLine(nodeShape);
        }

        private static void CreateCondition(Route route, BaseShape parent, int curChild, int childCount)
        {
            var conditionShape = new ConditionShape(route, parent, curChild, childCount);
            if (route.Target != string.Empty) AddChild(route, conditionShape);
        }
    }
}
