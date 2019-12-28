using System;
using System.IO;
using System.IO.Compression;
using Extensions;
using Microsoft.Extensions.Logging;

namespace Services.Packagers {

    /// упаковывает директорию в архив и сохраняет в backupDirectory
    public class ZipPacker {

        private ILogger m_Logger;

        public ZipPacker(ILoggerFactory loggerFactory) {
            m_Logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public void PackAndSave(string dataDirectoryPath, string backupDirectoryPath) 
        {
            if (!Directory.Exists(dataDirectoryPath)) {
                var message = $"Не найдена директория {dataDirectoryPath}";
                m_Logger.LogCritical(message);
                throw new DirectoryNotFoundException(message);
            }
            if (!Directory.Exists(backupDirectoryPath)) {
                var message = $"Не найдена директория {backupDirectoryPath}";
                m_Logger.LogCritical(message);
                throw new DirectoryNotFoundException(message);
            }
            try {
                var inputDirectory = new DirectoryInfo(dataDirectoryPath);
                var dateNow = DateTime.Now;
                var zipName = string.Format("{0}_{1}.zip", inputDirectory.Name, dateNow.ToTimeStamp());
                var outputFilePath = Path.Combine(backupDirectoryPath, zipName);

                DirectoryInfo dataDirectoryInfo = new DirectoryInfo(dataDirectoryPath);
                int substringStart = dataDirectoryInfo.FullName.Length + 1;
                using (FileStream zipArchive = new FileStream(outputFilePath, FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create))
                    {
                        CreateEntries(archive, dataDirectoryInfo.GetFileSystemInfos(), substringStart);
                    }
                }
                m_Logger.LogInformation("Создана резервная копия ");
            } catch (Exception e) {
                m_Logger.LogCritical(e.Message);
            }
            
        }
        private void CreateEntries(ZipArchive archive, FileSystemInfo[] fileSystemInfos, int substringStart) {
            foreach (var info in fileSystemInfos) {
                if (info is FileInfo fileInfo) {
                    var relPath = fileInfo.FullName.Substring(substringStart);
                    ZipArchiveEntry zipEntry = archive.CreateEntry(relPath);
                    using (Stream stream = zipEntry.Open())
                    using (StreamReader reader = new StreamReader(fileInfo.FullName))
                    {
                        reader.BaseStream.CopyTo(stream);
                    }

                } else if (info is DirectoryInfo dirInfo) {
                    CreateEntries(archive, dirInfo.GetFileSystemInfos(), substringStart);
                }
            }
        }


    }

}