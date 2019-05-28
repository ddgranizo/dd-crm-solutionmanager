using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolutionManagerUI.Utilities
{
    public static class FileDialogManager
    {

        public static string SelectFile(string defaultPath = null)
        {
            var file = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(defaultPath))
                {
                    openFileDialog.InitialDirectory = defaultPath;
                }
                openFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file = openFileDialog.FileName;
                }
            }
            return file;
        }

        public static string SelectPath(string defaultPath = null)
        {
            string path = null;
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
                {
                    fbd.SelectedPath = defaultPath;
                }
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }
            return path;
        }
    }
}
