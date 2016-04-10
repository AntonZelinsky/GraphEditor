using System;
using GraphEditor.Models;
using Microsoft.Win32;

namespace GraphEditor.Helper
{
    public static class FileOperation
    {
        private static string _pathFile;

        public static GraphModelSerialization Load()
        {
            var loadDialog = new OpenFileDialog
            {
                Filter = "Graph file (*.ge) | *.ge | All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (loadDialog.ShowDialog() == true)
            {
                var model = GraphSerializer.Deserialization(loadDialog.FileName);     
                _pathFile = loadDialog.InitialDirectory;
                return model;
            }
            return null;
        }

        public static void Save(GraphModelSerialization model)
        {       
            if(string.IsNullOrEmpty(_pathFile))
                SaveAs(model);
            GraphSerializer.Serialization(model, _pathFile);
            model.Changed = false; 
        }

        public static void SaveAs(GraphModelSerialization model)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Graph file (*.ge) | *.ge | All files (*.*)|*.*",
                InitialDirectory = _pathFile ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (saveDialog.ShowDialog() == true)
            {                                       
                _pathFile = saveDialog.FileName;
                Save(model);
            }
        }
    }
}