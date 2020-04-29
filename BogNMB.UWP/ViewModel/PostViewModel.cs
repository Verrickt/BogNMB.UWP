using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using BogNMB.UWP.IncrementalLoading;
using BogNMB.UWP.Model;
using BogNMB.UWP.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HTMLParser;
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
    public class PostViewModel : MyViewModelBase
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

        public RelayCommand RefreshCommand { get; private set; }
        private bool _onError;

        protected override void OnApiModeChanged(ApiConfig config)
        {
            Threads = new IncrementalLoadingCollection<ThreadLoader, ThreadViewModel>(
                new ThreadLoader(this._post,config), 20,
                () => { _onError = false; FooterText = Bible.OnLoading; RefreshCommand.RaiseCanExecuteChanged(); }
                , () => { if (!_onError) FooterText = Bible.OnFinished; RefreshCommand.RaiseCanExecuteChanged(); },
                (ex) => { FooterText = Bible.OnError; _onError = true; RefreshCommand.RaiseCanExecuteChanged(); });
        }

        public PostViewModel(Post post,ApiConfig config)
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
            Threads = new IncrementalLoadingCollection<ThreadLoader, ThreadViewModel>(
                            new ThreadLoader(this._post, config), 20,
                            () => { _onError = false; FooterText = Bible.OnLoading; RefreshCommand.RaiseCanExecuteChanged(); }
                            , () => { if (!_onError) FooterText = Bible.OnFinished; RefreshCommand.RaiseCanExecuteChanged(); },
                            (ex) => { FooterText = Bible.OnError; _onError = true; RefreshCommand.RaiseCanExecuteChanged(); });
            ImageSource = "http:" + _thumbSrc;
            ShowImage = !string.IsNullOrEmpty((_thumbSrc ?? _fullImgSrc));
            RefreshCommand = new RelayCommand(() => { if (_onError) Threads.RetryFailed(); else Threads.RefreshAsync(); }, () => !Threads.IsLoading, true);
        }
        private IncrementalLoadingCollection<ThreadLoader, ThreadViewModel> _threads;
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
        private ReferenceResolver resolver;
        private Post _post;
        private ApiConfig _config;
        public ThreadLoader(Post post,ApiConfig config)
        {
            _post = post;
            resolver = new ReferenceResolver();
            _config =config;
        }
        public async Task<IEnumerable<ThreadViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            int totalReply = _post.Hr + _post.Replys.Count;
            if (pageIndex * pageSize > totalReply)
            {
                return Enumerable.Empty<ThreadViewModel>();
            }
            var pc = new ThreadController(_config);
            var pocos = await pc.GetThreadsAsync(_post.No, pageIndex+1).ConfigureAwait(false);
            if (pageIndex == 0)
            {
                var SelfAsJson = (Thread)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(_post), typeof(Thread));
                pocos.Insert(0, SelfAsJson);
            }
            var threads = pocos.Select(i => new ThreadViewModel(i, _post.Id)).ToList();
            foreach (var t in threads) t.AstNodeTask = AstHelper.LoadHtmlAsync(t.Content);
            var result = await Task.WhenAll(threads.Select(i => i.AstNodeTask));
            foreach (var item in result)
            {
                resolver.Resolve(item, new ResolveContext() { Controller = new ReplyController(_config) });
            }
            return threads;
        }
    }
}
