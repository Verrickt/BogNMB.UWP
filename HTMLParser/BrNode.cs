using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class BrNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public BrNode(IHtmlBreakRowElement content)
        {
            Content = content;
        }
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public IHtmlBreakRowElement Content { get; set; }

    }
}
