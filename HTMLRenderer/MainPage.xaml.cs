using AngleSharp;
using AngleSharp.Dom;
using HTMLParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HTMLRenderer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly BrowsingContext context;
        string str = "<!DOCTYPE html>\r\n<html>\r\n<body>\r\n\r\n<iframe srcdoc=\"<p>Hello world!</p><iframe srcdoc='2323'></iframe>\" src=\"/demo/demo_iframe_srcdoc.html\">\r\n  <p>Your browser does not support iframes.</p>\r\n</iframe>\r\nthis is text\r\n<p><b>\u6CE8\u91CA\uFF1A</b>\u6240\u6709\u4E3B\u6D41\u6D4F\u89C8\u5668\u90FD\u652F\u6301 srcdoc \u5C5E\u6027\uFF0C\u9664\u4E86 Internet Explorer\u3002</p>\r\n\r\n</body>\r\n</html>\r\n";
        public MainPage()
        {
            this.InitializeComponent();
            Html.Text = str;
        }
       
        public async Task Parse()
        {
            
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
           // Parse();
        }
    }
}
