using System;
using Newtonsoft.Json;

namespace SharedCL.Extension
{
    public static class SystemExtension
    {
        public static T DeepClone<T>(this T source)
        {
            var jsonSettings = new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };
            var serialized = JsonConvert.SerializeObject(source, jsonSettings);
            return JsonConvert.DeserializeObject<T>(serialized, jsonSettings) ?? throw new InvalidOperationException();
        }
    }
}