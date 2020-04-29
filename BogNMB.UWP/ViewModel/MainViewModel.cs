using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class MyViewModelBase:ViewModelBase
    {
        public MyViewModelBase()
        {
            MessengerInstance.Register<PropertyChangedMessage<ApiConfig>>(this,
                (t) => {
                    OnApiModeChanged(t.NewValue);
                });
        }
        protected virtual void OnApiModeChanged(ApiConfig config)
        { }
    }

    public class MainViewModel : MyViewModelBase
    {
        private readonly LocalObjectStorageHelper _storageHelper=new LocalObjectStorageHelper();
        private const string forum_cache_key = "forums";
        private const string current_api_key = "current_api";
        private ObservableCollection<ForumViewModel> _forums;
        public ObservableCollection<ForumViewModel> Forums
        {
            get
            {
                return _forums;
            }
            set
            {
                Set(ref _forums, value);
            }
        }

        public ObservableCollection<ApiModeViewModel> ApiModes { get; }

        private ApiModeViewModel _currentApiMode;

        public ApiModeViewModel CurrentApiMode
        {
            get { return _currentApiMode; }
            set 
            {
                
                if (_currentApiMode!=null&&_currentApiMode!=value)
                {
                    MessengerInstance.Send(new PropertyChangedMessage<ApiConfig>(_currentApiMode?.ApiConfig
                    , value.ApiConfig, nameof(value.ApiConfig)));
                }
                Set(ref _currentApiMode, value);
            }
        }

        public RelayCommand RefreshForumsCommand { get;private set; }

        protected override void OnApiModeChanged(ApiConfig config)
        {
            //This needs UI context since we're dealing with ObservableCollection
            rebuildForumListAsync(config);
            //
            Task.Run(async() =>
            {
                //nothing big deal, thread pool should suffice
                await _storageHelper.SaveFileAsync(forum_cache_key, new List<Forum>());
                _storageHelper.Save(current_api_key, config);
            });
        }

        public MainViewModel()
        {

            Forums = new ObservableCollection<ForumViewModel>();
            RefreshForumsCommand = new RelayCommand(async() =>
            {
                var pocos = await new ForumController(CurrentApiMode.ApiConfig).GetForumsAsync();
                await _storageHelper.SaveFileAsync(forum_cache_key, pocos);
                _forums.Clear();
                pocos.ForEach(i => _forums.Add(new ForumViewModel(i,CurrentApiMode.ApiConfig)));
                SelectedForum = Forums.FirstOrDefault();
            });
            ApiModes = new ObservableCollection<ApiModeViewModel>(ApiModeViewModel.CreateApis());
        }
        public async Task InitializeAsync()
        {
            if (_storageHelper.KeyExists(current_api_key))
            {
                var config = _storageHelper.Read<ApiConfig>(current_api_key);
                CurrentApiMode = ApiModes.FirstOrDefault(i => i.ApiConfig == config);
            }
            if(CurrentApiMode==null)
            {
                CurrentApiMode = ApiModes.First();
                _storageHelper.Save(current_api_key, CurrentApiMode.ApiConfig);
            }
            await rebuildForumListAsync(CurrentApiMode.ApiConfig);
        }

        private async Task rebuildForumListAsync(ApiConfig config)
        {
            List<Forum> pocos = null;
            if (await _storageHelper.FileExistsAsync(forum_cache_key))
            {
                pocos = await _storageHelper.ReadFileAsync<List<Forum>>(forum_cache_key);
            }
            if (pocos.Count == 0)
            {
                pocos = await new ForumController(config).GetForumsAsync();
                await _storageHelper.SaveFileAsync(forum_cache_key, pocos);
            }
            _forums.Clear();
            pocos.ForEach(i => _forums.Add(new ForumViewModel(i, config)));
            SelectedForum = Forums.FirstOrDefault();
        }

        private ForumViewModel _selectedForum;

        public ForumViewModel SelectedForum
        {
            get { return _selectedForum; }
            set { Set(ref _selectedForum, value); }
        }

    }

    public class ApiModeViewModel:ViewModelBase
    {
        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { Set(ref _displayName, value); }
        }

        private string _imagePath;

        public string ImagePath
        {
            get { return _imagePath; }
            set { Set(ref _imagePath, value); }
        }

        public ApiConfig ApiConfig { get; set; }

        public ApiModeViewModel(ApiConfig api)
        {
            ApiConfig = api;
        }


        public static IEnumerable<ApiModeViewModel> CreateApis()
        {
            yield return new ApiModeViewModel(ApiConfig.Default) {DisplayName= "B岛-BOG匿名版",ImagePath= "/Assets/22.gif" };
            yield return new ApiModeViewModel(ApiConfig.Test) { DisplayName="新宝岛-BOG测试版",ImagePath="/Assets/33.gif"};
        }

    }
}
