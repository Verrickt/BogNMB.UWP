using BogNMB.API.Controllers;
using HTMLParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BogNMB.UWP.Model
{
    public class ResolveContext
    {
        public IAstNode Parent { get; set; }
        public ReplyController Controller { get; set; }
    }
    public class ReferenceResolver : IAstVisitor<ResolveContext>
    {
        public void Resolve(IAstNode root,ResolveContext context)
        {
            root.Accept(this, context);
        }
        void IAstVisitor<ResolveContext>.Visit(HTMLNode node, ResolveContext context)
        {
            context.Parent = node;
            node.Body.Accept(this, context);
        }

        void IAstVisitor<ResolveContext>.Visit(BodyNode body, ResolveContext context)
        {
            context.Parent = body;
            foreach (var item in body.Children)
                item.Accept(this, context);
        }

        void IAstVisitor<ResolveContext>.Visit(OtherNode other, ResolveContext context)
        {
            context.Parent = other;
            foreach (var item in other.Children)
                item.Accept(this, context);
        }

        void IAstVisitor<ResolveContext>.Visit(BrNode br, ResolveContext context)
        {
            return;
        }

        void IAstVisitor<ResolveContext>.Visit(ImgNode node, ResolveContext context)
        {
            return;
        }

        void IAstVisitor<ResolveContext>.Visit(ParagraphNode node, ResolveContext context)
        {
            context.Parent = node;
            foreach (var item in node.Children) node.Accept(this, context);
        }

        void IAstVisitor<ResolveContext>.Visit(HrefNode node, ResolveContext context)
        {
            return;
        }

        void IAstVisitor<ResolveContext>.Visit(TextNode node, ResolveContext context)
        {
            var txt = node.Text.Text;
            if (Regex.IsMatch(txt, "^>>Po\\.\\d+$"))//^>>Po\.\d+$
            {
                string id = txt.Substring(txt.LastIndexOf('.') + 1);
                if (int.TryParse(id, out var _))
                {
                    var reply = context.Controller.GetReplyAsync(id).ConfigureAwait(false)
                        .GetAwaiter().GetResult();
                    node.Refer = AstHelper.LoadHtmlAsync(reply.Com).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            return;
        }

        void IAstVisitor<ResolveContext>.Visit(IFrameNode node, ResolveContext context)
        {
            return;
        }

        void IAstVisitor<ResolveContext>.Visit(FontNode node, ResolveContext context)
        {
            context.Parent = node;
            foreach (var item in node.Children)
                item.Accept(this, context);
        }
    }
}
