using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace BogNMB.UWP.CustomControl
{
    public class StretchContentControl:ContentControl
    {
        private ContentPresenter preserenter;
        protected override void OnApplyTemplate()
        {
            preserenter = GetTemplateChild("preserenter") as ContentPresenter;
            base.OnApplyTemplate();
        }
        protected override Size MeasureOverride(Size availableSize)
        {
          //  var size = new Size(availableSize.Width - (Margin.Left + Margin.Right), availableSize.Height - (Margin.Top + Margin.Bottom));
            preserenter.Measure(availableSize);
            return new Size(availableSize.Width, 
                preserenter.DesiredSize.Height+(Margin.Top+Margin.Bottom));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rect = new Rect(Margin.Left, Margin.Top, finalSize.Width - (Margin.Left + Margin.Right), finalSize.Height - (Margin.Top + Margin.Bottom));
            preserenter.Arrange(rect);
            return finalSize;

        }
    }
}
