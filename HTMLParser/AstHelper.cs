using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace HTMLParser
{
    public static class AstHelper
    {
        public static async Task<HTMLNode> FromHtml(string str)
        {
            var config = Configuration.Default;
            var context = new BrowsingContext(config);
            var document = await context.OpenAsync(req => req.Content(str));
            var documentNode = new HTMLNode((IHtmlHtmlElement)document.DocumentElement);
            return documentNode;
        }
        private static Dictionary<Type, Type> _map;
        static AstHelper()
        {
            _map = new Dictionary<Type, Type>()
            {
                {typeof(IHtmlHtmlElement),typeof(HTMLNode) },
                {typeof(IHtmlBodyElement),typeof(BodyNode) },
                {typeof(IHtmlAnchorElement),typeof(HrefNode) },
                {typeof(IHtmlImageElement),typeof(ImgNode) },
                {typeof(IHtmlParagraphElement),typeof(ParagraphNode) },
                {typeof(IHtmlBreakRowElement),typeof(BrNode) },
                {typeof(IText),typeof(TextNode) },
                {typeof(IHtmlInlineFrameElement),typeof(IFrameNode) }
            };
        }

        internal static IAstNode CreateNodes(INode element)
        {
            IAstNode node;
            var result = _map.Keys.SingleOrDefault(k => element.GetType().IsDescenentOf(k));
            if (result != null)
            {
                node = (IAstNode)Activator.CreateInstance(_map[result], new[] { element });
            }
            else
            {
                node = new OtherNode(element);
            }
            return node;
        }
        internal static IEnumerable<IAstNode> TryCreateNodes(IEnumerable<INode> elements)
        {
            Type t = null;
            return elements.Select(i => CreateNodes(i));
        }
    }
}
