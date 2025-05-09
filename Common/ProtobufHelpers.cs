using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
   public  static class ProtobufHelpers
    {

        public static byte[] SerializeObject<T>(T record) where T : class
        {

            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, record);
                return stream.ToArray();
            }
        }
        public static T DeserializeObject<T>(byte[] data)
        {
            T obj = default(T);
            using (var stream = new MemoryStream(data))
            {
                obj = Serializer.Deserialize<T>(stream);
            }

            return obj;
        }
    }
}
