using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class BodyNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public void Accept(IAstVisitor visitor)
        {
             visitor.Visit(this);
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
