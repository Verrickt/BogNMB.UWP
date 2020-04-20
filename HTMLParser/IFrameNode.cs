using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class IFrameNode : IAstNode
    {
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        public IFrameNode(IHtmlInlineFrameElement content)
        {
            Content = content;
        }

        public IHtmlInlineFrameElement Content { get; private set; }
    }
}
