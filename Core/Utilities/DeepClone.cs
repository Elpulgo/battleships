using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace Core.Utilities
{

    public static class Extensions
    {
        public static T DeepClone<T>(this T obj)
        {
            if (!typeof(T).IsSerializable)
                throw new ArgumentException("Type {0} is not serializable", typeof(T).Name);

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}