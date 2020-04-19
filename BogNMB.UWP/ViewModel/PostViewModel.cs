using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using GalaSoft.MvvmLight;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Thread = BogNMB.API.POCOs.Thread;

namespace BogNMB.UWP.ViewModel
{
    class Config
    {
        public static ApiConfig ApiConfig => ApiConfig.Test;
    }
    public class PostViewModel : ViewModelBase
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
        public int ReplyCount { get; set; }
        public string No { get; set; }
        private readonly Post _post;
        private string _footerText;
        private string _thumbSrc;
        private string _fullImgSrc;
        public bool ShowImage { get; private set; }
        public string ImageSource { get;  set; }
        public string FooterText
        {
            get { return _footerText; }
            set { Set(ref _footerText, value); }
        }


        public PostViewModel(Post post)
        {
            _post = post;
            IsAdmin = post.Admin != 0;
            Saged = post.Sage == 1;
            Locked = post.Lock == 1;
            Top = post.Top == 1;
            Cookie = post.Id;
            Name = post.Name;
            Content = post.Com;
            ReplyCount = post.Hr + post.Replys.Count;
            Time = post.Time;
            No = post.No;
            _thumbSrc = post.Img;
            _fullImgSrc = post.Src;
            _loader = new ThreadLoader(_post);
            Threads = new IncrementalLoadingCollection<ThreadLoader, ThreadViewModel>(
                new ThreadLoader(this._post), 20,
                () => { FooterText = "丧尸->朱军->诸君->肥肥"; }
                , () => { FooterText = "加载完毕.点击刷新"; },
                (ex) => { FooterText = "加载出错.点击重试"; });
            ImageSource = "http:" + _thumbSrc;
            ShowImage = !string.IsNullOrEmpty((_thumbSrc ?? _fullImgSrc));
        }
        private IncrementalLoadingCollection<ThreadLoader, ThreadViewModel> _threads;
        private readonly ThreadLoader _loader;
        public IncrementalLoadingCollection<ThreadLoader, ThreadViewModel> Threads
        {
            get
            {
                return _threads;
            }
            private set
            {
                Set(ref _threads, value);
            }
        }

        public void Cancel()
        {
        }
    }

    public class TimedOutIncrementalLoadingCollection<TSource, IType> : IncrementalLoadingCollection<TSource, IType>
        where TSource : IIncrementalSource<IType>
    {
        public TimedOutIncrementalLoadingCollection(TSource source, int itemsPerPage = 20,
            Action onStartLoading = null, Action onEndLoading = null,
            Action<Exception> onError = null)
            : base(source, itemsPerPage,
                   onStartLoading, onEndLoading,
                   onError)
        {

        }
    }

    public class ThreadLoader : IIncrementalSource<ThreadViewModel>
    {
        private Post _post;
        public ThreadLoader(Post post)
        {
            _post = post;
        }
        public async Task<IEnumerable<ThreadViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            int totalReply = _post.Hr + _post.Replys.Count;
            if (pageIndex * pageSize > totalReply)
            {
                return Enumerable.Empty<ThreadViewModel>();
            }
            var pc = new ThreadController(Config.ApiConfig);
            var threads = await pc.GetThreadsAsync(_post.No, pageIndex+1);
            if (pageIndex == 0)
            {
                var SelfAsJson = (Thread)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(_post), typeof(Thread));
                threads.Insert(0, SelfAsJson);
            }
            return threads.Select(i => new ThreadViewModel(i, _post.Id));
        }
    }
}
