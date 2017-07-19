using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using TekkenEditor.Helper;

namespace TekkenEditor.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 
        private Page _currentPage;
        private IFileService _fileService;
        private IFrameNavigationService _frameNavigationService;
        private bool _isActive;
        public RelayCommand<object> OpenCommand { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }
        public Page CurrentPage {
            get {
                return _currentPage;
            }
            set {
                _currentPage = value;
                RaisePropertyChanged("CurrentPage");
            }
        }
        public bool IsActive {
            get {
                return _isActive;
            }
            set {
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }
        public MainViewModel(IFileService fileSerivce, IFrameNavigationService frameNavigationService)
        {
            _fileService = fileSerivce;
            _frameNavigationService = frameNavigationService;
            OpenCommand = new RelayCommand<object>((obj) => { OpenSave(obj); });
            SaveCommand = new RelayCommand<object>((obj) => { SaveFile(obj); });
        }
        private void OpenSave(object parm) {
            
            string path = _fileService.OpenFileDialog("Save(*.sav)|*.sav|All files (*.*)|*.*");
            
            if (path != null)
            {
                _frameNavigationService.NavigateTo("DefaultPage");
                SaveManager.LoadSave(path);
                _frameNavigationService.NavigateTo("SlotPage");
            }

            
        }
        private void SaveFile(object parm) {
            string path = _fileService.SaveFileDialog("Save(*.sav) | *.sav", "sav");
            if (path != null)
            {
                SaveManager.SaveDataToFile(path);
            }
        }
    }
}