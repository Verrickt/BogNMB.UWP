using AngleSharp;
using System;
using System.Collections.Generic;
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
        string str = "< a href=\\\"https://music.163.com/#/song?id=498286570\\\" title=\\\"https://music.163.com/#/song?id=498286570\\\" target=\\\"_blank\\\" rel=\\\"nofollow noreferrer\\\"><i class=\\\"iconfont icon-015\\\"></i>网页链接</a><br />あよ的声音真好听[〃∀〃]";
        public MainPage()
        {
            this.InitializeComponent();
            Html.Text = str;
            var config = Configuration.Default;
            context = new BrowsingContext(config);

        }

        public async Task Parse()
        {
            var document = await context.OpenAsync(req => req.Content(Html.Text));
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Parse();
        }
    }
}
