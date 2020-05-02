using BogNMB.API;
using BogNMB.API.Controllers;
using HTMLParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Drawing;
using Color = Windows.UI.Color;
using Rectangle = Windows.UI.Xaml.Shapes.Rectangle;

namespace BogNMB.UWP.CustomControl
{
    public static class BlockCollectionExtension
    {
        public static void Trim(this List<Paragraph> collection)
        {
            if (collection.Count == 1) return;
            foreach (var para in collection.OfType<Paragraph>())
            {
                if (para.Inlines.Count != 0) continue;
                collection.Remove(para);
            }
            if(collection.Count==0) { collection.Add(new Paragraph()); }
        }
    }
    class ParsingContext
    {
        public Stack<Paragraph> Stack { get; private set; }
        public List<Paragraph> Blocks { get; private set; }
        public int Level { get; private set; }
        public Stack<SolidColorBrush> Colors { get; private set; }

        public ParsingContext(int level)
        {
            Level = level;
            Stack = new Stack<Paragraph>();
            Blocks = new List<Paragraph>();
            Colors = new Stack<SolidColorBrush>();
        }
    }

    public class RichTextBlockRenderer : IAstVisitor<ParsingContext>
    {
        private static readonly ColorConverter colorConverter = new ColorConverter();

        public IReadOnlyList<Block> Render(IAstNode node)
        {
            var context = new ParsingContext(1);
            node.Accept(this, context);
            context.Blocks.Trim();
            return context.Blocks;
        }
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
            Run run = new Run() { Text = node.Content.Text };
            if (context.Colors.Count > 0) run.Foreground = context.Colors.Peek();
            h.Inlines.Add(run);
            var link = node.Content.GetAttribute("href");
            link = Regex.Unescape(link);
            link = link.Substring(1, link.Length - 2);
            h.NavigateUri = new Uri(link, UriKind.Absolute);
            context.Stack.Peek().Inlines.Add(h);
        }

        void IAstVisitor<ParsingContext>.Visit(TextNode node, ParsingContext context)
        {

            if (node.HasReferer)
            {
                {
                    var hyper = new Hyperlink();
                    Run run = new Run() { Text = node.Text.Text };
                    if (context.Colors.Count > 0) run.Foreground = context.Colors.Peek();
                    hyper.Inlines.Add(run);
                    context.Stack.Peek().Inlines.Add(hyper);
                }

                if (node.Refer != null)
                {
                    var root = node.Refer;
                    var rtb = new RichTextBlock();
                    rtb.Margin = new Thickness(8, 4, 8, 4);
                    var newContext = new ParsingContext(context.Level + 1);
                    if (context.Colors.Count > 0)
                    {
                        newContext.Colors.Push(context.Colors.Peek());
                    }
                    var panel = new InlineUIContainer();
                    root.Accept(this, newContext);
                    newContext.Blocks.Trim();

                    foreach (var item in newContext.Blocks) rtb.Blocks.Add(item);

                    var grid = new Grid()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Background = new SolidColorBrush((Color)App.Current.Resources["SystemAccentColorLight2"])
                };
                    var rect = new Rectangle();
                    grid.Children.Add(rect);
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    rect.Width = 8;
                    rect.VerticalAlignment = VerticalAlignment.Stretch;
                    rect.Fill = new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
                    grid.Children.Add(rtb);
                    Grid.SetColumn(rect, 0);
                    Grid.SetColumn(rtb, 1);
                    var stretch = new StretchContentControl();
                    stretch.Content = grid;
                    panel.Child = stretch;
                    context.Stack.Peek().Inlines.Add(new LineBreak());
                    context.Stack.Peek().Inlines.Add(panel);
                }
                else
                {
                    context.Stack.Peek().Inlines.Add(new Run() { Text = " 引用解析失败", Foreground = new SolidColorBrush(Colors.Red) });
                    context.Stack.Peek().Inlines.Add(new LineBreak());
                }
            }
            else
            {
                Run run = new Run() { Text = node.Text.Text };
                if (context.Colors.Count > 0) run.Foreground = context.Colors.Peek();
                context.Stack.Peek().Inlines.Add(run);
            }
        }

        void IAstVisitor<ParsingContext>.Visit(FontNode node, ParsingContext context)
        {
            bool flag = false;
            if (node.HexColor.Length > 0)
            {
                flag = true;
                var c = (System.Drawing.Color)colorConverter.ConvertFromString(node.HexColor);
                var color = Color.FromArgb(c.A, c.R, c.G, c.B);
                context.Colors.Push(new SolidColorBrush(color));

            }
            foreach (var item in node.Children)
            {
                item.Accept(this, context);
            }
            if (flag)
            {
                context.Colors.Pop();
            }
        }
    }
}
