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
