using HTMLParser;
using System;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BogNMB.UWP.CustomControl
{
    public class RichTextBlockExtension:DependencyObject
    {





        public static IAstNode GetRootNode(DependencyObject obj)
        {
            return (IAstNode)obj.GetValue(RootNodeProperty);
        }

        public static void SetRootNode(DependencyObject obj, IAstNode value)
        {
            obj.SetValue(RootNodeProperty, value);
        }

        // Using a DependencyProperty as the backing store for RootNode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RootNodeProperty =
            DependencyProperty.RegisterAttached("RootNode", typeof(IAstNode), typeof(RichTextBlockExtension), new PropertyMetadata("",new PropertyChangedCallback(OnNodeChanged)));

        private static void OnNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBlock rtb && e.NewValue is IAstNode root)
            {
                var renderer = new RichTextBlockRenderer();
                var res = renderer.Render(root);
                rtb.Blocks.Clear();
                foreach (var item in res) rtb.Blocks.Add(item);
            }
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(RichTextBlockExtension), new PropertyMetadata("",new PropertyChangedCallback(OnChanged))
                );

        private static async void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var html = e.NewValue.ToString();
            if (d is RichTextBlock rtb)
            {
                var renderer = new RichTextBlockRenderer();
                var res = await renderer.RenderAsync(html);
                rtb.Blocks.Clear();
                foreach (var item in res) rtb.Blocks.Add(item);
            }
        }
    }
}
