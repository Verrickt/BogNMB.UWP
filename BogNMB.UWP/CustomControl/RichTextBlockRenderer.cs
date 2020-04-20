using BogNMB.API;
using BogNMB.API.Controllers;
using HTMLParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace BogNMB.UWP.CustomControl
{
    class ParsingContext
    {
        public Stack<Paragraph> Stack { get; private set; }
        public List<Block> Blocks { get; private set; }
        public int Level { get; private set; }
        public ParsingContext(int level)
        {
            Level = level;
            Stack = new Stack<Paragraph>();
            Blocks = new List<Block>();
        }
    }

    public class RichTextBlockRenderer : IAstVisitor<ParsingContext>
    {
        public async Task<IReadOnlyList<Block>> RenderAsync(string html)
        {
            var root = await AstHelper.LoadHtmlAsync(html).ConfigureAwait(false);
            var context = new ParsingContext(1);
            root.Content.Normalize();
            root.Accept(this, context);
            var res = context.Blocks.ToArray();
            return res;
        }

        void IAstVisitor<ParsingContext>.Visit(IFrameNode node, ParsingContext context)
        {
            var inner = node.Content.ContentHtml;
            var root = AstHelper.LoadHtmlAsync(inner).Result;
            var newContext = new ParsingContext(context.Level + 1);
            root.Accept(this, newContext);
            var panel = new InlineUIContainer();
            var rtb = new RichTextBlock();
            var bb = new Paragraph();
            bb.Inlines.Add(new Run() { Text = $"Level {newContext.Level}" });
            rtb.Blocks.Add(bb);
            foreach (var item in newContext.Blocks) rtb.Blocks.Add(item);
            var grid = new Grid()
            {
                Background = new SolidColorBrush(Colors.Red),
                Margin = new Thickness(12, 0, 0, 0)
            };
            grid.Children.Add(rtb);
            panel.Child = grid;
            var block = new Paragraph();
            block.Inlines.Add(panel);
            context.Blocks.Add(block);
        }

        void IAstVisitor<ParsingContext>.Visit(HTMLNode node, ParsingContext context)
        {
            context.Stack.Push(new Paragraph());
            node.Body.Accept(this, context);
            var c = context.Stack.Pop();
            context.Blocks.Add(c);
        }

        void IAstVisitor<ParsingContext>.Visit(BodyNode body, ParsingContext context)
        {
            foreach (var item in body.Children)
            {
                item.Accept(this, context);
            }
        }

        void IAstVisitor<ParsingContext>.Visit(OtherNode other, ParsingContext context)
        {
            foreach (var item in other.Children)
            {
                item.Accept(this, context);
            }
        }

        void IAstVisitor<ParsingContext>.Visit(BrNode br, ParsingContext context)
        {
            context.Stack.Peek().Inlines.Add(new LineBreak());
        }

        void IAstVisitor<ParsingContext>.Visit(ImgNode node, ParsingContext context)
        {
            context.Stack.Peek().Inlines.Add(new InlineUIContainer()
            {
                Child = new Border()
                {
                    Height = 20,
                    Width = 20,
                    Background = new SolidColorBrush(Colors.Red)
                }

            });
        }

        void IAstVisitor<ParsingContext>.Visit(ParagraphNode node, ParsingContext context)
        {
            context.Stack.Push(new Paragraph());
            foreach (var item in node.Children)
            {
                item.Accept(this, context);
            }
            var c = context.Stack.Pop();
            context.Blocks.Add(c);
        }

        void IAstVisitor<ParsingContext>.Visit(HrefNode node, ParsingContext context)
        {
            Hyperlink h = new Hyperlink();
            h.Inlines.Add(new Run() { Text = node.Content.Text });
            var link = node.Content.GetAttribute("href");
            link = Regex.Unescape(link);
            link = link.Substring(1, link.Length - 2);
            h.NavigateUri = new Uri(link, UriKind.Absolute);
            context.Stack.Peek().Inlines.Add(h);
        }

        void IAstVisitor<ParsingContext>.Visit(TextNode node, ParsingContext context)
        {
            var txt = node.Text.Text;
            if(Regex.IsMatch(txt, "^>>Po\\.\\d+$"))//^>>Po\.\d+$
            {
                string id = txt.Substring(txt.LastIndexOf('.')+1);
                if(int.TryParse(id,out var _))
                {
                    var rc = new ReplyController(ApiConfig.Test);
                    var task = rc.GetReplyAsync(id).ConfigureAwait(false);
                    var reply = task.GetAwaiter().GetResult();
                    //var reply = task.Result;
                    {
                        var inner = reply.Com;
                        var root = AstHelper.LoadHtmlAsync(inner).Result;
                        var newContext = new ParsingContext(context.Level + 1);
                        root.Accept(this, newContext);
                        var panel = new InlineUIContainer();
                        var rtb = new RichTextBlock();
                        var bb = new Paragraph();
                        bb.Inlines.Add(new Run() { Text = $"Level {newContext.Level}" });
                        rtb.Blocks.Add(bb);
                        foreach (var item in newContext.Blocks) rtb.Blocks.Add(item);
                        var grid = new Grid()
                        {
                            Background = new SolidColorBrush(Colors.Red),
                            Margin = new Thickness(12, 0, 0, 0)
                        };
                        grid.Children.Add(rtb);
                        panel.Child = grid;
                        var block = new Paragraph();
                        block.Inlines.Add(panel);
                        context.Blocks.Add(block);
                    }
                }
            }
            else
            {
                context.Stack.Peek().Inlines.Add(new Run() { Text = node.Text.Text });
            }
        }
    }
}
