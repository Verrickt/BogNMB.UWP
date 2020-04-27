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
        }
        public async Task InitializeAsync()
        {
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
}
