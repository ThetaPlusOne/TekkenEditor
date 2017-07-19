using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using TekkenEditor.Helper;
namespace TekkenEditor.ViewModel
{
    public class SlotPageViewModel : ViewModelBase
    {
        private String[] _characterList;
        private ObservableCollection<CharacterCostume> _costumeList;
        private String _selectedCharName;
        private CharacterCostume _selectedCostume;
        private int _slotIndex;
        private IFileService _fileService;
        private IFrameNavigationService _frameNavigationService;
        public RelayCommand<object> EditCommand { get; set; }
        public RelayCommand<object> SaveJsonCommand { get; set; }
        public RelayCommand<object> SaveJsonWithImgCommand { get; set; }
        public RelayCommand<object> LoadJsonCommand { get; set; }

        public RelayCommand<object> LoadedCommand { get; set; }

        public String[] CharacterList { get { return _characterList; } }

        public int SlotIndex {
            get {
                return _slotIndex;
            }
            set {
                _slotIndex = value;
                RaisePropertyChanged("SlotIndex");
            }

        }
        public CharacterCostume SelectedCosume
        {
            get
            {
                return _selectedCostume;
            }
            set
            {
                _selectedCostume = value;
                RaisePropertyChanged("SelectedCosume");
            }
        }

        public ObservableCollection<CharacterCostume> CostumeList
        {
            get
            {
                return _costumeList;
            }
            set
            {
                _costumeList = value;
                RaisePropertyChanged("CostumeList");
            }
        }

        public String SelectedCharName
        {
            get
            {
                return _selectedCharName;
            }
            set
            {
                _selectedCharName = value;
                onCharacterSelect();

                RaisePropertyChanged("SelectedCharName");
            }

        }
        public SlotPageViewModel(IFileService fileSerivce, IFrameNavigationService frameNavigationService)
        {
            _fileService = fileSerivce;
            _frameNavigationService = frameNavigationService;
            EditCommand = new RelayCommand<object>((obj) => openCharacterSlot(obj));
            SaveJsonCommand = new RelayCommand<object>((obj) => saveCharacterSlot(false));
            SaveJsonWithImgCommand = new RelayCommand<object>((obj) => saveCharacterSlot(true));
            LoadJsonCommand = new RelayCommand<object>((obj) => loadCharacterSlot(obj));
            LoadedCommand = new RelayCommand<object>((obj) => onLoaded(obj));

            _characterList = new String[SaveConstant.CHARACTER_LIST.Length];
            Array.Copy(SaveConstant.CHARACTER_LIST, _characterList, SaveConstant.CHARACTER_LIST.Length);
            Array.Sort(_characterList); 
        }



        public void onCharacterSelect() {
            if (SelectedCharName != null) {
                CharacterCostume[] rstList = SaveManager.getCostumeList(SelectedCharName);
                
                if (rstList != null) {
                    CostumeList = new ObservableCollection<CharacterCostume>( rstList);
                }
            }
        }
        public void openCharacterSlot(object parm) { 
            if(SelectedCosume != null && !SelectedCosume.IsEmpty){
                object[] p = new object[2];
                p[0] = SelectedCosume;
                p[1] = SlotIndex;
                _frameNavigationService.NavigateTo("EditPage", p);
            }


        }

        public void saveCharacterSlot(bool flag) {
            if (SelectedCosume != null && !SelectedCosume.IsEmpty)
            {
                string path = _fileService.SaveFileDialog("(*.json) | *.json", "json");
                if (path != null) {
                    string output = "";
                    if (flag)
                    {
                        output = JsonConvert.SerializeObject(SelectedCosume, Formatting.Indented, new CostumeJsonConvertor(typeof(CharacterCostume)));
                    }
                    else {
                        output = JsonConvert.SerializeObject(SelectedCosume);
                    }
                    
                    File.WriteAllText(path,output);
                }
            }

        }

        public void loadCharacterSlot(object parm)
        {
            if (SlotIndex >= 0)
            {
                string path = _fileService.OpenFileDialog("(*.json)|*.json|All files (*.*)|*.*");
                if (path != null)
                {
                    CharacterCostume costume = JsonConvert.DeserializeObject<CharacterCostume>(File.ReadAllText(path), new CostumeJsonConvertor(typeof(CharacterCostume)));
                    if (costume != null) {
                        if (costume.Thumbnail == null)
                        {
                            costume.Thumbnail = new System.Drawing.Bitmap(TekkenEditor.Properties.Resources._ugli);
                        }
                        
                        SaveManager.insertCostume(costume, SlotIndex);
                        CostumeList[SlotIndex] = costume;
                    }
                    
                }
            }
        }


        public void onLoaded(object parm)
        {
            CostumeList = null;
        }
    }
}
