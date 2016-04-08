using System.IO;
using System.Runtime.Serialization.Json;
using GraphEditor.Models;

namespace GraphEditor.Helper
{
    public static class GraphSerialization
    {
        private static readonly DataContractJsonSerializer JsonFormatter = new DataContractJsonSerializer(typeof(GraphModel));

        public static void Serialization(GraphModel model)
        {                                                                                                
            using (var fs = new FileStream("graph.json", FileMode.OpenOrCreate))
            {
                JsonFormatter.WriteObject(fs, model);
            }
        }

        public static GraphModel Deserialization()
        {   
            using (var fs = new FileStream("graph.json", FileMode.OpenOrCreate))
            {
                var model = (GraphModel)JsonFormatter.ReadObject(fs);
                return model;
            }
        }
    }  
}