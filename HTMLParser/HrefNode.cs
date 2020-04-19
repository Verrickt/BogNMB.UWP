using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class HrefNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public HrefNode(IHtmlAnchorElement content)
        {
            Content = content;
        }
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public IHtmlAnchorElement Content { get; private set; }
    }
}
