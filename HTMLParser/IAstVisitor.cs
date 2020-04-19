namespace HTMLParser
{
    public interface IAstVisitor
    {
        void Visit(HTMLNode node);
        void Visit(BodyNode body);
        void Visit(OtherNode other);
        void Visit(BrNode br);
        void Visit(ImgNode node);
        void Visit(ParagraphNode node);
        void Visit(HrefNode node);
        void Visit(TextNode node);
    }
    public interface IAstVisitor<T>
    {
        T Visit(HTMLNode node);
        T Visit(BodyNode body);
        T Visit(OtherNode other);
        T Visit(BrNode br);
        T Visit(ImgNode node);
        T Visit(ParagraphNode node);
        T Visit(HrefNode node);
        T Visit(TextNode node);
    }
}
