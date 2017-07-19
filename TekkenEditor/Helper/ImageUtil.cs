using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TekkenEditor
{
    class ImageUtil
    {
        public static Bitmap createImage(byte[] data)
        {

            Bitmap bitmap = new Bitmap(SaveConstant.IMG_WIDTH, SaveConstant.IMG_HEIGHT);
            int i = 0;
            for (int y = 0; y < SaveConstant.IMG_HEIGHT; y++)
                for (int x = 0; x < SaveConstant.IMG_WIDTH; x++)
                {
                    bitmap.SetPixel(x, y, Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i]));
                    i = i + 4;

                }
            return bitmap;
        }

        public static byte[] compressImage(Bitmap bitmap)
        {
            byte[] rawData = new byte[SaveConstant.IMG_HEIGHT * SaveConstant.IMG_WIDTH * 4];
            int i = 0;
            for (int y = 0; y < SaveConstant.IMG_HEIGHT; y++)
                for (int x = 0; x < SaveConstant.IMG_WIDTH; x++)
                {
                    Color p = bitmap.GetPixel(x, y);
                    rawData[i + 3] = p.A;
                    rawData[i + 2] = p.R;
                    rawData[i + 1] = p.G;
                    rawData[i] = p.B;
                    i = i + 4;

                }

            return Ionic.Zlib.ZlibStream.CompressBuffer(rawData);

        }
    }


}
