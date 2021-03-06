﻿using System.IO;
using Newtonsoft.Json;

namespace Ray.EssayNotes.AutoFac.Infrastructure.Helpers
{
    public static class JsonHelper
    {
        /// <summary>json格式化</summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string AsFormatJsonString(this string str)
        {
            var jsonSerializer = new JsonSerializer();
            var jsonTextReader = new JsonTextReader((TextReader)new StringReader(str));
            object obj = jsonSerializer.Deserialize((JsonReader)jsonTextReader);
            if (obj == null)
                return str;
            var stringWriter = new StringWriter();
            var jsonTextWriter1 = new JsonTextWriter((TextWriter)stringWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            JsonTextWriter jsonTextWriter2 = jsonTextWriter1;
            jsonSerializer.Serialize((JsonWriter)jsonTextWriter2, obj);
            return stringWriter.ToString();
        }
    }
}
