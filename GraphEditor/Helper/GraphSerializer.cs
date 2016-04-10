using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using GraphEditor.Models;

namespace GraphEditor.Helper
{
    public static class GraphSerializer
    {
        private static readonly DataContractJsonSerializer JsonFormatter = new DataContractJsonSerializer(typeof(GraphModelSerialization));

        public static void Serialization(GraphModelSerialization model, string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    JsonFormatter.WriteObject(fs, model);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: Invalid file", e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static GraphModelSerialization Deserialization(string path)
        {
            try
            {  
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    var model = (GraphModelSerialization)JsonFormatter.ReadObject(fs);
                    return model;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error: Invalid file", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }  
}