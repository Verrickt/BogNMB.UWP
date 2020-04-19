using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class ImgNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public ImgNode(IHtmlImageElement content)
        {
            Content = content;
        }
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public IHtmlImageElement Content { get; private set; }
    }
}
