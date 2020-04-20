using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;

namespace HTMLParser
{
    public class OtherNode : IAstNode
    {
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        public List<IAstNode> Children { get; private set; }
        public OtherNode(INode content)
        {
            Content = content;
            Children = AstHelper.TryCreateNodes(content.ChildNodes.ToList()).ToList();
        }
     
        public INode Content { get; private set; }
    }
}
