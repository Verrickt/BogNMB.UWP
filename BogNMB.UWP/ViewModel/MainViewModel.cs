using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class MainViewModel : ViewModelBase
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
                if(_currentApiMode!=null&&_currentApiMode!=value)
                {
                    Task.Run(() => { _storageHelper.Save(current_api_key, value); });
                }
                Set(ref _currentApiMode, value);
            }
        }


        public RelayCommand RefreshForumsCommand { get;private set; }

        public MainViewModel()
        {
            Forums = new ObservableCollection<ForumViewModel>();
            RefreshForumsCommand = new RelayCommand(async() =>
            {
                var pocos = await new ForumController(Config.ApiConfig).GetForumsAsync();
                await _storageHelper.SaveFileAsync(forum_cache_key, pocos);
                _forums.Clear();
                pocos.ForEach(i => _forums.Add(new ForumViewModel(i)));
                SelectedForum = Forums.FirstOrDefault();
            });
            ApiModes = new ObservableCollection<ApiModeViewModel>(ApiModeViewModel.CreateApis());
        }
        public async Task InitializeAsync()
        {
            if(_storageHelper.KeyExists(current_api_key))
            {
                var model = _storageHelper.Read<ApiModeViewModel>(current_api_key);
                CurrentApiMode = ApiModes.First(i => i.ApiConfig == model.ApiConfig);
            }
            else
            {
                CurrentApiMode = ApiModes.First();
            }
            _storageHelper.Save(current_api_key, CurrentApiMode);
            List<Forum> pocos;
            if(await _storageHelper.FileExistsAsync(forum_cache_key))
            {
                pocos = await _storageHelper.ReadFileAsync<List<Forum>>(forum_cache_key);
            }
            else
            {
                pocos = await new ForumController(Config.ApiConfig).GetForumsAsync();
                await _storageHelper.SaveFileAsync(forum_cache_key, pocos);
            }
            pocos.ForEach(i => _forums.Add(new ForumViewModel(i)));
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
