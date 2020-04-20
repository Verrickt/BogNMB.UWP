using BogNMB.API.POCOs;
using GalaSoft.MvvmLight;
using HTMLParser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class ThreadViewModel:ViewModelBase
    {
        public bool IsAdmin { get; }
        public bool Saged { get; }
        public bool Locked { get; }
        public string Content { get; }
        public string Cookie { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public bool Top { get; set; }
        public string No { get; set; }
        public bool IsPo { get; set; }
        private string _thumbSrc;
        private string _fullImgSrc;
        public bool ShowImage { get; private set; }
        public string ImageSource { get; set; }

        public Task<HTMLNode> AstNodeTask { get; set; }
        public ThreadViewModel(Thread thread,string poCookie)
        {
            IsAdmin = thread.Admin != 0;
            Saged = thread.Sage == 1;
            Locked = thread.Lock == 1;
            Top = thread.Top == 1;
            Cookie = thread.Id;
            Name = thread.Name;
            Content = thread.Com;
            Time = thread.Time;
            No = thread.No;
            IsPo = poCookie == thread.Id;
            _thumbSrc = thread.Img;
            _fullImgSrc = thread.Src;
            ImageSource = "http:" + _thumbSrc;
            ShowImage = !string.IsNullOrEmpty((_thumbSrc ?? _fullImgSrc));
        }
    }
}
