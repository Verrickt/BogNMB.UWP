using HTMLParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace BogNMB.UWP.CustomControl
{
    public class RichTextBlockRenderer : IAstVisitor
    {
        private readonly Stack<Paragraph> _workingStack = new Stack<Paragraph>();
        private List<Block> _blocks = new List<Block>();


        public async Task<IReadOnlyList<Block>> RenderAsync(string html)
        {
            Debug.Assert(_workingStack.Count == 0);
            var root = await AstHelper.LoadHtmlAsync(html);
            root.Accept(this);
            Debug.Assert(_workingStack.Count == 0);
            var res = _blocks.ToArray();
            _blocks.Clear();
            return res;
        }

        void IAstVisitor.Visit(HTMLNode node)
        {
            _workingStack.Push(new Paragraph());
            node.Body.Accept(this);
            var c = _workingStack.Pop();
            _blocks.Add(c);
        }

        void IAstVisitor.Visit(BodyNode body)
        {
            foreach (var item in body.Children)
            {
                item.Accept(this);
            }
        }

        void IAstVisitor.Visit(OtherNode other)
        {
            foreach (var item in other.Children)
            {
                item.Accept(this);
            }
        }

        void IAstVisitor.Visit(BrNode br)
        {
            _workingStack.Peek().Inlines.Add(new LineBreak());
        }

        void IAstVisitor.Visit(ImgNode node)
        {
            _workingStack.Peek().Inlines.Add(new InlineUIContainer()
            {
                Child = new Border()
                {
                    Height = 20,
                    Width = 20,
                    Background = new SolidColorBrush(Colors.Red)
                }

            });
        }

        void IAstVisitor.Visit(ParagraphNode node)
        {
            _workingStack.Push(new Paragraph());
            var p = _workingStack.Pop();
            _blocks.Add(p);
        }

        void IAstVisitor.Visit(HrefNode node)
        {
            var h = new Hyperlink();
            h.Inlines.Add(new Run() { Text = node.Content.Text });
            var link = node.Content.GetAttribute("href");
            link = Regex.Unescape(link);
            link = link.Substring(1, link.Length - 2);
            h.NavigateUri = new Uri(link, UriKind.Absolute);
            _workingStack.Peek().Inlines.Add(h);
        }

        void IAstVisitor.Visit(TextNode node)
        {
            _workingStack.Peek().Inlines.Add(new Run() { Text = node.Text.Text });
        }
    }
}
