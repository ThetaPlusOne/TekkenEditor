using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekkenEditor
{
    public class CharacterCostume 
    {
        [JsonIgnore]
        public bool IsEmpty { get; set; }
        private int _charId;

        private List<Item> _itemList = new List<Item>();

        private Bitmap _thumbnail;

        public int CharId
        {
            get
            {
                return _charId;
            }
            set
            {
                _charId = value;
            }
        }

        public List<Item> Items
        {
            get
            {
                if (_itemList == null) {
                    _itemList = new List<Item>();
                }
                return this._itemList;
            }

        }
        [JsonIgnore]
        public Bitmap Thumbnail
        {
            get
            {
                return this._thumbnail;
            }
            set
            {
                this._thumbnail = value;
            }
        }

    }

    public class Item
    {
        private UInt64 _itemId;
        private List<CustomColor> _colors;


        public UInt64 ItemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                this._itemId = value;
            }
        }

        public List<CustomColor> Colors
        {
            get
            {
                if (_colors == null) {
                    _colors = new List<CustomColor>();
                }
                return _colors;
            }
            set {
                _colors = value;
            }

        }


        public int getColorFlag(){
            int flag = 0;
            for (int i = 0; i < Colors.Count; i++) {
                if (Colors[i].InUse) {
                    flag |= (0x1) << i;
                }
            }
            return flag;

        }
    }
    public class CustomColor 
    {
        public bool InUse { get; set; }
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
