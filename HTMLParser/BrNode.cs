using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class BrNode : IAstNode
    {
        public BrNode(IHtmlBreakRowElement content)
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
        
        public IHtmlBreakRowElement Content { get; set; }

    }
}
