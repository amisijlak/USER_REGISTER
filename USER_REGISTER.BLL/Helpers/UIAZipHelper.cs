using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Helpers
{
    public static class USER_REGISTERZipHelper
    {
        private static string FILE_NAME = "data.txt";

        public static void ExtractZipData<T>(string tempFolderPath, string zipArchiveBase64, Action<List<T>> processRecordsFtn, int processBatchSize
            , bool deserializeData = true, Action<string> logOffendingData = null) where T : class
        {
            DeleteFolder(tempFolderPath);

            try
            {
                Directory.CreateDirectory(tempFolderPath);

                var zipPath = Path.Combine(tempFolderPath, "archive.zip");

                //create zipfile
                using (var stream = new FileStream(zipPath, FileMode.CreateNew))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(Convert.FromBase64String(zipArchiveBase64));
                        writer.Flush();
                        writer.Close();
                    }
                }

                //extract zip data
                ZipFile.ExtractToDirectory(zipPath, tempFolderPath);

                var filePath = Path.Combine(tempFolderPath, FILE_NAME);

                //read archive contents
                using (var reader = new StreamReader(filePath))
                {
                    List<T> data = new List<T>();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        try
                        {
                            if (!string.IsNullOrEmpty(line)) data.Add(deserializeData ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(line)
                            : (T)Convert.ChangeType(line, typeof(T)));
                        }
                        catch (Exception f)
                        {
                            logOffendingData?.Invoke($"Data: {line}");
                            throw f;
                        }

                        if (data.Count == processBatchSize)
                        {
                            processRecordsFtn(data);
                            data = new List<T>();
                        }
                    }

                    if (data.Any()) processRecordsFtn(data);
                }
            }
            finally
            {
                DeleteFolder(tempFolderPath);
            }
        }

        private static void DeleteFolder(string path)
        {
            try
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            catch (Exception e) { }
        }

        public static string GenerateZip<T>(string tempFolderPath, Action<Action<List<T>>> processRecordsFtn, bool serializeData = true) where T : class
        {
            DeleteFolder(tempFolderPath);

            try
            {
                Directory.CreateDirectory(tempFolderPath);

                var innerTempPath = Path.Combine(tempFolderPath, "temp");

                Directory.CreateDirectory(innerTempPath);

                var filePath = Path.Combine(innerTempPath, FILE_NAME);

                using (var writer = new StreamWriter(filePath))
                {
                    processRecordsFtn(data =>
                    {
                        foreach (var item in data)
                        {
                            writer.WriteLine(serializeData ? Newtonsoft.Json.JsonConvert.SerializeObject(item) : item?.ToString());
                        }
                    });

                    writer.Flush();
                    writer.Close();
                }

                var zipPath = Path.Combine(tempFolderPath, "archive.zip");

                ZipFile.CreateFromDirectory(innerTempPath, zipPath, CompressionLevel.Optimal, false);

                using (var stream = new FileStream(zipPath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        return Convert.ToBase64String(reader.ReadBytes((int)reader.BaseStream.Length));
                    }
                }
            }
            finally
            {
                DeleteFolder(tempFolderPath);
            }
        }
    }
}
