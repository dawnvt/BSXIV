namespace BSXIV.Utilities
{ 
    using System;
    using System.IO;

    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                    continue;

                var key = parts[0];

                var value = string.Join('=', parts.Skip(1));

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}