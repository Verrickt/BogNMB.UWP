using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public class ImgNode : IAstNode
    {
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        public ImgNode(IHtmlImageElement content)
        {
            Content = content;
        }
       
        public IHtmlImageElement Content { get; private set; }
    }
}
