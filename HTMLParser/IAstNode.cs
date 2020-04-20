namespace HTMLParser
{
    public class Context { }
    public interface IAstNode
    {
        T Accept<T,U>(IAstVisitor<T,U> visitor,U context);
        void Accept<U>(IAstVisitor<U> visitor, U context);
    }
}
