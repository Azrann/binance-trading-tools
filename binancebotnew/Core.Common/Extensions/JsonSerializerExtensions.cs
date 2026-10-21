using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Core.Common.Extensions
{
    public static class JsonConvertExt
    {
        public static string SerializeObject(object obj)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = '\'';

                JsonSerializer ser = new JsonSerializer {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
                ser.Serialize(writer, obj);
            }

            return sb.ToString();
        }
    }
}
