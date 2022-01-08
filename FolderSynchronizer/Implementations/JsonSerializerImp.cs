using FolderSynchronizer.Abstractions;
using System.Text.Json;

namespace FolderSynchronizer.Implementations
{
    public class JsonSerializerImp : ISerializer
    {
        public string Serialize<T>(T data)
        {
            var result = JsonSerializer.Serialize(data);
            return result;
        }

        public T Deserialize<T>(string data)
        {
            var result = JsonSerializer.Deserialize<T>(data);
            return result;
        }
    }
}