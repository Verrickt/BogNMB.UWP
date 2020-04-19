using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;

namespace HTMLParser
{
    public class OtherNode : IAstNode
    {
        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
        public List<IAstNode> Children { get; private set; }
        public OtherNode(INode content)
        {
            Content = content;
            Children = AstHelper.TryCreateNodes(content.ChildNodes.ToList()).ToList();
        }
        public void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
        public INode Content { get; private set; }
    }
}
