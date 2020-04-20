using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class ParagraphNode : IAstNode
    {
       
        public List<IAstNode> Children { get; private set; }
        public ParagraphNode(IHtmlParagraphElement content)
        {
            Content = content;
            Children = AstHelper.TryCreateNodes(content.ChildNodes.ToList()).ToList();
        }
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        public IHtmlParagraphElement Content { get; private set; }
    }
}
