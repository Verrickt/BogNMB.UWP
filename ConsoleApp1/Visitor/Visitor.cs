using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace ConsoleApp1.Visitor
{
    public static class ExtensionMethods
    {
        public static bool IsDescenentOf(this Type t1,Type t2)
        {
            Queue<Type> queue = new Queue<Type>();
            queue.Enqueue(t1);
            while (queue.Count>0)
            {
                var t = queue.Dequeue();
                if (t == t2) return true;
                else t.GetInterfaces().ToList().ForEach(i => queue.Enqueue(i));
            }
            return false;
        }
        public static U Cast<U>(this object t) where U : IElement
        {
            if (t is U u) return u;
            throw new InvalidCastException();
        }
    }
    public interface INode
    {
        public T Accept<T>(INodeVisitor<T> visitor);
    }

    public static class INodeHelper
    {
        private static Dictionary<Type, Type> _map;
        static INodeHelper()
        {
            _map = new Dictionary<Type, Type>()
            {
                {typeof(IHtmlHtmlElement),typeof(HTMLNode) },
                {typeof(IHtmlBodyElement),typeof(BodyNode) },
                {typeof(IHtmlAnchorElement),typeof(HrefNode) },
                {typeof(IHtmlImageElement),typeof(ImgNode) },
                {typeof(IHtmlParagraphElement),typeof(ParagraphNode) },
                {typeof(IHtmlBreakRowElement),typeof(BrNode) },
            };
        }

        internal static INode Create(IElement element)
        {
            INode node;
            var result = _map.Keys.SingleOrDefault(k => element.GetType().IsDescenentOf(k));
            if (result!=null)
            {
                node = (INode)Activator.CreateInstance(_map[result], new[] { element });
            }
            else
            {
                node = new OtherNode(element);
            }
            return node;
        }
        internal static bool TryCreate(IElement element,out INode node)
        {
            bool result = _map.TryGetValue(element.GetType(), out var target);
            if (result)
            {
                node =(INode)Activator.CreateInstance(target, new[] { element });
            }
            else
            {
                node = new OtherNode(element);
            }
            return result;
        }
        internal static IEnumerable<INode> TryCreateNodes(IEnumerable<IElement> elements)
        {
            Type t = null;
            return elements.Select(i => Create(i));
        }
    }
    public interface INodeVisitor<T>
    {
        public T Visit(HTMLNode node);
        public T Visit(BodyNode body);
        public T Visit(OtherNode other);
        public T Visit(BrNode br);
        public T Visit(ImgNode node);
        public T Visit(ParagraphNode node);

        public T Visit(HrefNode node);
    }

    public class HTMLNode : INode
    {
        public BodyNode Body { get; private set; }
        public HTMLNode(IHtmlHtmlElement content)
        {
            Content = content;
            var bdElem = Content.QuerySelector("body")
                            .Cast<IHtmlBodyElement>();
            Body = new BodyNode(bdElem);
        }

        public IHtmlHtmlElement Content { get; set; }

        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    public class BodyNode : INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public List<INode> Children { get; private set; }
        public BodyNode(IHtmlBodyElement content)
        {
            Content = content;
            Children = INodeHelper.TryCreateNodes(content.Children).ToList();
        }

        public IHtmlBodyElement Content { get; private set; }
    }
    public class OtherNode:INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public List<INode> Children { get; private set; }
        public OtherNode(IElement content)
        {
            Content = content;
            Children = INodeHelper.TryCreateNodes(content.Children).ToList();
        }

        public IElement Content { get; private set; }
    }
    public class BrNode:INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public BrNode(IHtmlBreakRowElement content)
        {
            Content = content;
        }

        public IHtmlBreakRowElement Content { get; set; }

    }
    public class HrefNode : INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public HrefNode(IHtmlAnchorElement content)
        {
            Content = content;
        }
         
        public IHtmlAnchorElement Content { get; private set; }
    }
    public class ImgNode : INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public ImgNode(IHtmlImageElement content)
        {
            Content = content;
        }

        public IHtmlImageElement Content { get; private set; }
    }
    public class ParagraphNode : INode
    {
        public T Accept<T>(INodeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public List<INode> Children { get; private set; }
        public ParagraphNode(IHtmlParagraphElement content)
        {
            Content = content;
            Children = INodeHelper.TryCreateNodes(content.Children).ToList();
        }

        public IHtmlParagraphElement Content { get; private set; }
    }
}
