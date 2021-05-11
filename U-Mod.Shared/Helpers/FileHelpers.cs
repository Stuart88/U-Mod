using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace U_Mod.Shared.Helpers
{
    public static class FileHelpers
    {
        public static void SaveAsJson<T>(T item, string savePath)
        {
            string jsonString = JsonSerializer.Serialize(item);
            File.WriteAllText(savePath, jsonString);
        }
    }
}
