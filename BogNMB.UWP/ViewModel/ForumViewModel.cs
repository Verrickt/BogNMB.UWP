using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class ForumViewModel : ViewModelBase
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

        public ForumViewModel(Forum forum)
        {
            Name = forum.Name;
            ID = forum.Id;
            Posts = new IncrementalLoadingCollection<PostLoader, PostViewModel>(
                new PostLoader(this), 20,
                () => { _onError = false; FooterText = "丧尸->朱军->诸君->肥肥";RefreshCommand.RaiseCanExecuteChanged(); }
                , () => { if(!_onError) FooterText = "加载完毕.点击刷新"; RefreshCommand.RaiseCanExecuteChanged(); },
                (ex) => { _onError = true; FooterText = "加载出错.点击重试"; RefreshCommand.RaiseCanExecuteChanged(); });
            RefreshCommand = new RelayCommand(()=> { Posts.RefreshAsync(); },()=> !Posts.IsLoading,true);
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

        public PostLoader(ForumViewModel forum)
        {
            _forum = forum;
        }

        public async Task<IEnumerable<PostViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var pc = new PostController(Config.ApiConfig);
            var posts = await pc.GetPostAsync(_forum.ID, pageIndex + 1);
            return posts.Select(i => new PostViewModel(i));
        }
    }
}
