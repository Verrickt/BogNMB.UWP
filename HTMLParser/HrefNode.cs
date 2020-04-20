using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class HrefNode : IAstNode
    {
        public HrefNode(IHtmlAnchorElement content)
        {
            Content = content;
        }

        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        public IHtmlAnchorElement Content { get; private set; }
    }
}
