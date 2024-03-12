using System.IO;
using UnityEngine;

namespace NPSerialization
{
    public class JsonStream : IStream
    {
        public bool Load<T>(string path, out T obj)
        {
            string jsonString = File.ReadAllText(path);
            obj = JsonUtility.FromJson<T>(jsonString);

            return obj != null;
        }

        public void Save<T>(T obj, string path)
        {
            string jsonString = JsonUtility.ToJson(obj);
            File.WriteAllText(path, jsonString);
        }
    }
}
