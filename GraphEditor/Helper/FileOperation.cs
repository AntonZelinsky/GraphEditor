using System;
using GraphEditor.Models;
using Microsoft.Win32;

namespace GraphEditor.Helper
{
    public static class FileOperation
    {
        private static string _initialDirectory;

        public static GraphModelSerialization Load()
        {
            var loadDialog = new OpenFileDialog
            {
                Filter = "Graph file (*.ge)|*.ge| All files (*.*)|*.*",
                InitialDirectory = _initialDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (loadDialog.ShowDialog() == true)
            {
                var model = GraphSerializer.Deserialization(loadDialog.FileName);
                _initialDirectory = loadDialog.FileName;
                model.FileName = loadDialog.FileName;
                return model;
            }
            return null;
        }

        public static void Save(GraphModelSerialization model)
        {       
            if(string.IsNullOrEmpty(model.FileName))
                SaveAs(model);
            else if(GraphSerializer.Serialization(model, model.FileName))
                model.Changed = false;    
        }

        public static void SaveAs(GraphModelSerialization model)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Graph file (*.ge)|*.ge| All files (*.*)|*.*",
                InitialDirectory = model.FileName ?? _initialDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (saveDialog.ShowDialog() == true)
            {
                _initialDirectory = saveDialog.FileName;
                model.FileName = saveDialog.FileName;
                Save(model);
            }
        }
    }
}