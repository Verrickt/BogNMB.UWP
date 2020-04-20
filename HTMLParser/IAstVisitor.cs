namespace HTMLParser
{
    public interface IAstVisitor<in U>
    {
        void Visit(HTMLNode node, U context);
        void Visit(BodyNode body, U context);
        void Visit(OtherNode other, U context);
        void Visit(BrNode br, U context);
        void Visit(ImgNode node, U context);
        void Visit(ParagraphNode node, U context);
        void Visit(HrefNode node, U context);
        void Visit(TextNode node, U context);
    }
    public interface IAstVisitor<T,in U>
    {
        T Visit(HTMLNode node,U context);
        T Visit(BodyNode body, U context);
        T Visit(OtherNode other, U context);
        T Visit(BrNode br, U context);
        T Visit(ImgNode node, U context);
        T Visit(ParagraphNode node, U context);
        T Visit(HrefNode node, U context);
        T Visit(TextNode node, U context);
    }
}
