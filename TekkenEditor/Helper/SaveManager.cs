using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TekkenEditor
{
    class SaveManager
    {
        private static byte[] data;

        static Dictionary<String, int> characterDict = SaveConstant.CHARACTER_LIST.Select((value, index) => new { value, index })
              .ToDictionary(pair => pair.value, pair => pair.index);

        private SaveManager() { }


        public static void LoadSave(String path) {
            data = EncryptionUtil.Decrypt(File.ReadAllBytes(path));
        }
        public static void SaveDataToFile(String path) {
            if (data != null) {
               File.WriteAllBytes(path, EncryptionUtil.Encrypt(data));
            }
        }

        private static void InsertImage(int charId, int slot, Bitmap bitmap)
        {


            int imageOffset = SaveConstant.START_OFFSET + SaveConstant.CHARACTER_BLOCK_SIZE * charId + SaveConstant.CHARACTER_HEADER_SIZE + SaveConstant.SUM_ITEM_SIZE + (SaveConstant.IMAGE_META_SIZE + SaveConstant.MAX_IMAGE_SIZE) * slot;

            byte[] compressedData =ImageUtil.compressImage(bitmap);


            data[imageOffset] = (byte)(compressedData.Length & 0xFF);
            data[imageOffset + 1] = (byte)(compressedData.Length >> 8 & 0xFF);
            data[imageOffset + 2] = (byte)(compressedData.Length >> 16 & 0xFF);
            data[imageOffset + 3] = (byte)(compressedData.Length >> 24 & 0xFF);
            data[imageOffset + 3] = (byte)(compressedData.Length >> 24 & 0xFF);


            Array.Copy(compressedData, 0, data, imageOffset + SaveConstant.IMAGE_META_SIZE, compressedData.Length);


        }
        private static Bitmap DecompressImageSlot(int charId, int slot)
        {

            int imageOffset = SaveConstant.START_OFFSET + SaveConstant.CHARACTER_BLOCK_SIZE * charId + SaveConstant.CHARACTER_HEADER_SIZE + SaveConstant.SUM_ITEM_SIZE + (SaveConstant.IMAGE_META_SIZE + SaveConstant.MAX_IMAGE_SIZE) * slot;
            int imageLen = BitConverter.ToInt32(data, imageOffset);
            byte[] compressedData = new byte[imageLen];
            Array.Copy(data, imageOffset + SaveConstant.IMAGE_META_SIZE, compressedData, 0, imageLen);
            byte[] uncompressedData = Ionic.Zlib.ZlibStream.UncompressBuffer(compressedData);

            return ImageUtil.createImage(uncompressedData);
        }


        public static CharacterCostume[] getCostumeList(String charName) {
            if (!characterDict.ContainsKey(charName)){
                return null;
            }
            int charId = characterDict[charName];

            int characterOffset = SaveConstant.START_OFFSET + SaveConstant.CHARACTER_BLOCK_SIZE * charId;

            int slotFlag = BitConverter.ToInt32(data,characterOffset + SaveConstant.SLOT_COUNTER_OFFSET);
            CharacterCostume[] costumeList = new CharacterCostume[SaveConstant.SLOT_NUM];

            for (int i = 0; i < SaveConstant.SLOT_NUM; i++) {
                if (((slotFlag >> i) & 0x1) == 1)
                {
                    costumeList[i] = createCostume(charId, i);
                }else {
                    costumeList[i] = new CharacterCostume();
                    costumeList[i].IsEmpty = true;
                    costumeList[i].Thumbnail = new Bitmap(TekkenEditor.Properties.Resources._default);
                }

            }
            return costumeList;
        }

        private static CharacterCostume createCostume(int charId, int slot)
        {
            int characterOffset = SaveConstant.START_OFFSET + SaveConstant.CHARACTER_BLOCK_SIZE * charId;
            int costumeOffset = characterOffset + SaveConstant.CHARACTER_HEADER_SIZE + (SaveConstant.ITEM_BLOCK_SIZE * SaveConstant.ITEM_BLOCK_NUM) * slot;

            CharacterCostume costume = new CharacterCostume();
            costume.CharId = charId;

            for (int i = 0; i < SaveConstant.ITEM_BLOCK_NUM; i++)
            {
                Item item = new Item();
                item.ItemId = BitConverter.ToUInt64(data, costumeOffset + SaveConstant.ITEM_BLOCK_SIZE * i);
                int colorflag = BitConverter.ToInt32(data, costumeOffset + SaveConstant.ITEM_BLOCK_SIZE
                    * i + SaveConstant.ITEM_ID_SIZE);


                int colorOffset = costumeOffset + SaveConstant.ITEM_ID_SIZE + SaveConstant.ITEM_FLAG_SIZE + SaveConstant.ITEM_BLOCK_SIZE * i;
                for (int j = 0; j < SaveConstant.ITEM_COLOR_NUM; j++)
                {
                    item.Colors.Add(new CustomColor());
                    if (((colorflag >> j) & 0x1) == 1)
                    {
                        Int32 c = BitConverter.ToInt32(data, colorOffset);

                        item.Colors[j].A = (byte)(c >> 24);
                        item.Colors[j].R = (byte)(c >> 16);
                        item.Colors[j].G = (byte)(c >> 8);
                        item.Colors[j].B = (byte)c;
                        item.Colors[j].InUse = true;
                    }
                    colorOffset += SaveConstant.ITEM_COLOR_SIZE;
                }

                costume.Items.Add(item);
                
            }

            costume.Thumbnail = DecompressImageSlot(charId, slot);
            costume.IsEmpty = false;
            return costume;
        }


        public static void insertCostume(CharacterCostume costume, int slot) {
            int characterOffset = SaveConstant.START_OFFSET + SaveConstant.CHARACTER_BLOCK_SIZE * costume.CharId;
            int costumeOffset = characterOffset + SaveConstant.CHARACTER_HEADER_SIZE + (SaveConstant.ITEM_BLOCK_SIZE * SaveConstant.ITEM_BLOCK_NUM) * slot;

            MemoryStream ms = new MemoryStream(data, true);
            
            for (int i = 0; i < SaveConstant.ITEM_BLOCK_NUM; i++)
            {
                ms.Seek(costumeOffset + SaveConstant.ITEM_BLOCK_SIZE * i, SeekOrigin.Begin);
                Item item = costume.Items[i];
                Byte[] buf = BitConverter.GetBytes(item.ItemId);

                ms.Write(buf ,0, buf.Length);

                buf = BitConverter.GetBytes(item.getColorFlag());

                ms.Write(buf, 0, buf.Length);


                for (int j = 0; j < item.Colors.Count; j++) {
                    ms.WriteByte(item.Colors[j].B);
                    ms.WriteByte(item.Colors[j].G);
                    ms.WriteByte(item.Colors[j].R);
                    ms.WriteByte(item.Colors[j].A);
                }

            }
            ms.Close();
            if (slot >= 8)
            {
                data[characterOffset + 3] = (byte)(data[characterOffset + 3] | (1 << (slot - 8)));
            }
            else
            {
                data[characterOffset + 2] = (byte)(data[characterOffset + 2] | (1 << (slot)));
            }
            InsertImage(costume.CharId, slot, costume.Thumbnail);
        }

    }

}
