namespace NPSerialization
{
    public interface IStream
    {
        public bool Save(string path);
        public bool Load(string path);
    }
}
