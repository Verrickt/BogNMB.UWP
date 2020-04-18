using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1.Visitor
{
    public class Render
    {
        public int Level;
        public string Content;
        public List<Render> Children;
        public Render(int level,string content)
        {
            Children = new List<Render>();
            Level = level;
            Content = content;
        }

        public void Print()
        {
            for (int i = 0; i < Level; i++) Console.Write(" ");
            Console.WriteLine(Content);
            foreach (var item in Children)
            {
                item.Print();
            }
        }
    }
    public class RichTextBlockVisitor : INodeVisitor<Render>
    {
        int recursionLevel = 0;
        public Render Visit(BodyNode body)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "body");
            var childrens = body.Children.Select(i => i.Accept(this));
            render.Children.AddRange(childrens);
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(BrNode br)
        {

            recursionLevel += 1;
            var render = new Render(recursionLevel, "newline");
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(HTMLNode node)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "html");
            var body =  node.Body.Accept(this);
            render.Children.Add(body);
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(ImgNode node)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "img");
            render.Content = node.Content.InnerHtml.ToString();
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(OtherNode other)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "other");
            var childrens = other.Children.Select(i => i.Accept(this));
            render.Children.AddRange(childrens);
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(ParagraphNode node)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "paragraph");
            var childrens = node.Children.Select(i => i.Accept(this));
            render.Children.AddRange(childrens);
            recursionLevel -= 1;
            return render;
        }

        public Render Visit(HrefNode node)
        {
            recursionLevel += 1;
            var render = new Render(recursionLevel, "href");
            recursionLevel -= 1;
            return render;
        }
    }
}
