﻿using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using BogNMB.UWP.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HTMLParser;
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

        public RelayCommand RefreshCommand { get; private set; }
        private bool _onError;
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
                () => { _onError = false; FooterText = "丧尸->朱军->诸君->肥肥";RefreshCommand.RaiseCanExecuteChanged(); }
                , () => { if(!_onError) FooterText = "加载完毕.点击刷新"; RefreshCommand.RaiseCanExecuteChanged(); },
                (ex) => { FooterText = "加载出错.点击重试";_onError = true; RefreshCommand.RaiseCanExecuteChanged(); });
            ImageSource = "http:" + _thumbSrc;
            ShowImage = !string.IsNullOrEmpty((_thumbSrc ?? _fullImgSrc));
            RefreshCommand = new RelayCommand(() => { Threads.RefreshAsync(); }, () => !Threads.IsLoading, true);
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
        public ThreadLoader(Post post)
        {
            _post = post;
            resolver = new ReferenceResolver();
        }
        public async Task<IEnumerable<ThreadViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            int totalReply = _post.Hr + _post.Replys.Count;
            if (pageIndex * pageSize > totalReply)
            {
                return Enumerable.Empty<ThreadViewModel>();
            }
            var pc = new ThreadController(Config.ApiConfig);
            var pocos = await pc.GetThreadsAsync(_post.No, pageIndex+1);
            if (pageIndex == 0)
            {
                var SelfAsJson = (Thread)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(_post), typeof(Thread));
                pocos.Insert(0, SelfAsJson);
            }
            var threads = pocos.Select(i => new ThreadViewModel(i, _post.Id)).ToList();
            var ui = Task.Yield();
            await Task.Delay(10).ConfigureAwait(false);
            foreach (var t in threads) t.AstNodeTask = AstHelper.LoadHtmlAsync(t.Content);
            var result = await Task.WhenAll(threads.Select(i => i.AstNodeTask));
            foreach (var item in result)
            {
                resolver.Resolve(item, new ResolveContext() { Controller = new ReplyController(Config.ApiConfig) });
            }
            await ui;
            return threads;
        }
    }
}
