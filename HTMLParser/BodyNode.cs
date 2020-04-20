using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class BodyNode : IAstNode
    {
        public T Accept<T,U>(IAstVisitor<T,U> visitor,U context)
        {
            return visitor.Visit(this,context);
        }

        public void Accept<U>(IAstVisitor<U> visitor,U context)
        {
             visitor.Visit(this,context);
        }

        public List<IAstNode> Children { get; private set; }
        public BodyNode(IHtmlBodyElement content)
        {
            Content = content;
            Children = AstHelper.TryCreateNodes(content.ChildNodes.ToList()).ToList();
        }

        public IHtmlBodyElement Content { get; private set; }
    }
}
