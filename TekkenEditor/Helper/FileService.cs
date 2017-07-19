using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TekkenEditor.Helper
{
    public interface IFileService
    {
        String OpenFileDialog(String filter);
        String SaveFileDialog(String filter, String defExt);
    }
    class FileService : IFileService
    {
        public string FileName { get; set; }


        public string OpenFileDialog(String filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public string SaveFileDialog(String filter, String defExt)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.DefaultExt = defExt;

            if (saveFileDialog.ShowDialog() == true) {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}
