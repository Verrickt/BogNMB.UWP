using AngleSharp.Dom;

namespace HTMLParser
{
    public class TextNode : IAstNode
    {
        public TextNode(IText textNode)
        {
            Text = textNode;
        }

        public IText Text { get; private set; }
        public HTMLNode Refer { get; set; }

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
