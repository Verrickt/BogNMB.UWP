using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLParser
{
    public class FontNode:IAstNode
    {
        public T Accept<T, U>(IAstVisitor<T, U> visitor, U context)
        {
            return visitor.Visit(this, context);
        }

        public void Accept<U>(IAstVisitor<U> visitor, U context)
        {
            visitor.Visit(this, context);
        }
        
        public string HexColor { get; private set; }
        public List<IAstNode> Children { get; }

        public FontNode(IElement element)
        {
            Children = AstHelper.TryCreateNodes(element.ChildNodes).ToList();
            HexColor = element.GetAttribute("color")??string.Empty;
        }
    }
}
