using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UA.Integration.SDK
{
    public static class BsonExtensions
    {
        public static bool TryGet<T>(this BsonDocument doc, string key, out T value)
        {
            if (doc.Contains(key) && doc[key].IsBsonNull == false)
            {
                value = (T)Convert.ChangeType(doc[key].RawValue, typeof(T));
                return true;
            }
            value = default;
            return false;
        }
    }

}
