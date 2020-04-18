using BogNMB.API.Controllers;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace BogNMB.UWP.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
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

        public MainViewModel()
        {
            Forums = new ObservableCollection<ForumViewModel>();
        }
        public async Task InitializeAsync()
        {
            var pocos = await new ForumController(Config.ApiConfig).GetForumsAsync();
            pocos.ForEach(i => _forums.Add(new ForumViewModel(i)));
        }

        private ForumViewModel _selectedForum;

        public ForumViewModel SelectedForum
        {
            get { return _selectedForum; }
            set { Set(ref _selectedForum, value); }
        }

    }
}
