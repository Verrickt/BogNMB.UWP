using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class ParagraphNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public List<IAstNode> Children { get; private set; }
        public ParagraphNode(IHtmlParagraphElement content)
        {
            Content = content;
            Children = AstHelper.TryCreateNodes(content.ChildNodes.ToList()).ToList();
        }
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public IHtmlParagraphElement Content { get; private set; }
    }
}
