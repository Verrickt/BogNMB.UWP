using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using BogNMB.UWP.IncrementalLoading;
using BogNMB.UWP.Util;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class ForumViewModel : MyViewModelBase
    {

        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public int ID { get; private set; }

        private string _desc;

        private IncrementalLoadingCollection<PostLoader, PostViewModel> _posts;

        public string FooterText
        {
            get
            {
                return _footerText;
            }
            set
            {
                Set(ref _footerText, value);
            }
        }

        public string Desc
        {
            get { return _desc; }
            set { Set(ref _desc, value); }
        }

        private bool _onError = false;

        public RelayCommand RefreshCommand { get; private set; }

        protected override void OnApiModeChanged(ApiConfig config)
        {
            Posts = new IncrementalLoadingCollection<PostLoader, PostViewModel>(
                new PostLoader(this,config), 20,
                () => { _onError = false; FooterText = Bible.OnLoading; RefreshCommand.RaiseCanExecuteChanged(); }
                , () => { if (!_onError) FooterText = Bible.OnFinished; RefreshCommand.RaiseCanExecuteChanged(); },
                (ex) => { FooterText = Bible.OnError; _onError = true; RefreshCommand.RaiseCanExecuteChanged(); });
        }
        public ForumViewModel(Forum forum,ApiConfig config)
        {
            Name = forum.Name;
            ID = forum.Id;
            Posts = new IncrementalLoadingCollection<PostLoader, PostViewModel>(
                new PostLoader(this, config), 20,
                () => { _onError = false; FooterText = Bible.OnLoading; RefreshCommand.RaiseCanExecuteChanged(); }
                , () => { if (!_onError) FooterText = Bible.OnFinished; RefreshCommand.RaiseCanExecuteChanged(); },
                (ex) => { FooterText = Bible.OnError; _onError = true; RefreshCommand.RaiseCanExecuteChanged(); });
            RefreshCommand = new RelayCommand(() => { if (_onError) Posts.RetryFailed(); else Posts.RefreshAsync(); }, () => !Posts.IsLoading, true);
        }

        public IncrementalLoadingCollection<PostLoader, PostViewModel> Posts
        {
            get
            {
                return _posts;
            }
            set
            {
                Set(ref _posts, value);
            }
        }

        private PostViewModel _selectedPost;
        private string _footerText;

        public PostViewModel SelectedPost
        {
            get { return _selectedPost; }
            set { Set(ref _selectedPost, value); }
        }

    }

    public class PostLoader : IIncrementalSource<PostViewModel>
    {
        private readonly ForumViewModel _forum;
        private readonly ApiConfig _config;

        public PostLoader(ForumViewModel forum,ApiConfig config)
        {
            _forum = forum;
            this._config = config;
        }

        public async Task<IEnumerable<PostViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var pc = new PostController(_config);
            var posts = await pc.GetPostAsync(_forum.ID, pageIndex + 1);
            return posts.Select(i => new PostViewModel(i,_config));
        }
    }
}
