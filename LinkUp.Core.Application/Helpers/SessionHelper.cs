using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LinkUp.Core.Application.Helpers
{
    public static class SessionHelper
    {
        public static void Set<T>(this ISession session, string key, T value) => session.SetString(key, JsonSerializer.Serialize(value));
        public static T? Get<T>(this ISession session, string key)
        {
            var json = session.GetString(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}
