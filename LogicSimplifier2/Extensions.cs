using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace LogicSimplifier2
{
    public static class Extensions
    {
        public static IEnumerable<int> GetTrueIndices(this bool[] arr)
        {
            return arr.Select((b, i) => (b, i)).Where(p => p.b).Select(p => p.i);
        }

        public static string OutputDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        public static void Serialize(this object o, string filename)
        {
            FileInfo file = new FileInfo(Path.Combine(OutputDirectory, filename));
            if (!file.Directory.Exists) file.Directory.Create();
            string s = JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            File.WriteAllText(file.FullName, s);
        }
    }
}
