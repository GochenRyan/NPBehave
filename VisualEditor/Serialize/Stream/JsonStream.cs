using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace NPSerialization
{
    public class JsonStream : IStream
    {
        public bool Load<T>(string path, out T obj)
        {
            string jsonString = File.ReadAllText(path);
            obj = JsonConvert.DeserializeObject<T>(jsonString);

            return obj != null;
        }

        public void Save<T>(T obj, string path)
        {
            string jsonString = JsonConvert.SerializeObject(obj); 
            File.WriteAllText(path, jsonString);
        }
    }
}
