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
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
