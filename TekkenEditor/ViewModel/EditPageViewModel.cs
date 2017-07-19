using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TekkenEditor.Helper;

namespace TekkenEditor.ViewModel
{
    public class EditPageViewModel : ViewModelBase
    {
        private IFileService _fileService;
        private CharacterCostume _costume;
        private int _slot;
        private Bitmap _thumbnail;
        private ObservableCollection<ItemWraper> _wrappedItems;

        public RelayCommand<object> ChangeImageCommand { get; set; }
        public RelayCommand<object> DoneCommand { get; set; }
        public RelayCommand<object> BackCommand { get; set; }
        public RelayCommand<object> LoadedCommand { get; set; }

        public int Slot{
            get
            {
                return _slot;
            }
        }
        public ObservableCollection<ItemWraper> WrappedItems
        {
            get
            {
                return _wrappedItems;
            }
            set
            {
                _wrappedItems = value;
                RaisePropertyChanged("WrappedItems");
            }

        }
        public Bitmap Thumbnail {
            get {
                return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                RaisePropertyChanged("Thumbnail");
            }
        }
        
        private IFrameNavigationService _frameNavigationService;
        public EditPageViewModel(IFileService fileSerivce, IFrameNavigationService frameNavigationService)
        {
            _fileService = fileSerivce;
            _frameNavigationService = frameNavigationService;
            _costume = (CharacterCostume)((object[])_frameNavigationService.Parameter)[0];
            _slot = (int)((object[])_frameNavigationService.Parameter)[1];

            Thumbnail = _costume.Thumbnail;

            WrappedItems = new ObservableCollection<ItemWraper>();

            LoadedCommand =  new RelayCommand<object>((obj) => onLoaded(obj));
            DoneCommand = new RelayCommand<object>((obj) => onDoneClick(obj));
            ChangeImageCommand = new RelayCommand<object>((obj) => onChangeImageClick(obj));
            BackCommand = new RelayCommand<object>((obj) => _frameNavigationService.GoBack());
        }

        public void onLoaded(object parm){

            _costume = (CharacterCostume)((object[])_frameNavigationService.Parameter)[0];
            _slot = (int)((object[])_frameNavigationService.Parameter)[1];

            Thumbnail = _costume.Thumbnail;

            WrappedItems.Clear();

            for (int i = 0; i < _costume.Items.Count; i++)
            {
                ItemWraper iw = new ItemWraper(_costume.Items[i]);
                
                iw.Header = SaveConstant.ITEM_LIST[i];
                WrappedItems.Add(iw);

            }
        }
        public void onDoneClick(object parm)
        {
            _costume.Thumbnail = Thumbnail;
            for (int i = 0; i < _costume.Items.Count; i++) {
                _costume.Items[i].ItemId = WrappedItems[i].ItemId;
                for (int j = 0; j < _costume.Items[i].Colors.Count; j++) {
                    _costume.Items[i].Colors[j] = WrappedItems[i].Colors[j];
                }
                

            }
            SaveManager.insertCostume(_costume, _slot);
            _frameNavigationService.GoBack();
        }
        public void onChangeImageClick(object parm)
        {
            string path = _fileService.OpenFileDialog("Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*");
            if (path != null)
            {
                Bitmap tmp = new Bitmap(path);
                if (tmp != null)
                {
                    Thumbnail = tmp;
                }
            }
        }
    }

    public class ItemWraper
    {
        public String Header { get; set; }


        public UInt64 ItemId { get; set; }
        public ObservableCollection<CustomColor> Colors { get; set;}
        public ItemWraper(Item Item)
        {
            ItemId = Item.ItemId;
            Colors = new ObservableCollection<CustomColor>();
            for (int i = 0; i < Item.Colors.Count; i++) {
                CustomColor copy = new CustomColor();
                copy.InUse = Item.Colors[i].InUse;
                copy.A = Item.Colors[i].A;
                copy.R = Item.Colors[i].R;
                copy.B = Item.Colors[i].B;
                copy.G = Item.Colors[i].G;
                Colors.Add(copy);
            }
        }
        public CustomColor SelectedColor { get; set; }
    }
   
}
