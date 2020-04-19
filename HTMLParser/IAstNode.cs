namespace HTMLParser
{
    public interface IAstNode
    {
        T Accept<T>(IAstVisitor<T> visitor);
        void Accept(IAstVisitor visitor);
    }
}
