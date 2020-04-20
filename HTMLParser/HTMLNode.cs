using System.Linq;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class HTMLNode : IAstNode
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
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
    }
}
