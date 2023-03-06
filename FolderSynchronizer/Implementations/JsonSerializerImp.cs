using FolderSynchronizer.Abstractions;
using System.Text.Json;

namespace FolderSynchronizer.Implementations
{
    public class JsonSerializerImp : IJsonSerializer
    {
        public string Serialize<T>(T data)
        {
            var result = JsonSerializer.Serialize(data);
            return result;
        }

        public T Deserialize<T>(string data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<T>(data, options);
            return result;
        }
    }
}