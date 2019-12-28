using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enums;
using Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Packagers;

namespace Services {
    /// служба для периодического создания резервных копий директории
    public class BackupService : IHostedService
    {
        private Timer m_Timer;
        private IConfiguration m_Configuration;
        private string[] m_DataDirectories;
        private string[] m_BackupDirectories;
        /// если все бекапы нужно помещать в одну директорию, то будет true
        /// если false, То индекс директории в массиве m_BackupDirectories будет таким же
        /// как индекс директории с данными
        private bool m_IsSingleBackupDir = true;
        private ZipPacker m_ZipPacker;
        private ILogger m_Logger;
        /// периодичность бекапа
        private int m_BackupDefaultIntervalMinutes = 0;

        public BackupService(IConfiguration configuration, ILoggerFactory loggerFactory) 
        {
            m_Configuration = configuration;
            m_Logger = loggerFactory.CreateLogger(GetType().Name);
            m_DataDirectories = m_Configuration["DataDirs"].Split(';', StringSplitOptions.RemoveEmptyEntries);
            m_BackupDirectories = m_Configuration["BackupDirs"].Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (!int.TryParse(configuration["BackupDefaultIntervalMinutes"], out m_BackupDefaultIntervalMinutes)) {
                m_BackupDefaultIntervalMinutes = 60 * 24;
            }

            if (m_BackupDirectories.Length == 0) 
            {
                throw new Exception("Не задана директория сохранения резервных копий");
            } 
            else if (m_BackupDirectories.Length > 1) 
            {
                if (m_DataDirectories.Length != m_BackupDirectories.Length) {
                    throw new Exception(
                        @"Количество директорий резервных копий должно быть 
                        либо равно 1, либо точно совпадать c количеством директорий даных"
                    );
                }
                m_IsSingleBackupDir = false;
            }
             // нельзя, чтобы бекапы делались в ту же директорию, что и сами данные
            // чтобы при следующем бекапе не паковать сами бекапы в архив
            foreach (var dataPath in m_DataDirectories) 
            {
                var dataDir = new DirectoryInfo(dataPath);
                foreach (var backupPath in m_BackupDirectories) 
                {
                    var backupDir = new DirectoryInfo(backupPath);
                    if (dataDir.ContainsDirectory(backupDir)) 
                    {
                        throw new Exception(
                            "Директория для сохранения бэкапов не должна быть внутри директории с данными"
                        );
                    }
                }
            }
            m_ZipPacker = new ZipPacker(loggerFactory);
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            ClearTimer();
            // по умолчанию интервал 1440 минут или 24 часа
            m_Timer = new Timer(OnTimer, null, TimeSpan.Zero, 
                    TimeSpan.FromMinutes(m_BackupDefaultIntervalMinutes));
            return Task.CompletedTask;
        }

        public string GetBackupsListingAsJson() {
            return GetBackupListings().ToJson();
        }

        /// для каждой директории бекапов возвращает листинг
        private Dictionary<string, string[]> GetBackupListings() 
        {
            var dic = new Dictionary<string, string[]>();
            foreach (var backupDirPath in m_BackupDirectories) 
            {
                var directoryInfo = new DirectoryInfo(backupDirPath);
                dic[directoryInfo.Name] = directoryInfo.GetFiles().Select(v => v.Name).ToArray();
            }
            return dic;
        } 
        
        private void OnTimer(object state) 
        {
            Task.Run(async () => await MakeBackup());
        }

        /// по умолчанию сохраняет базу во все доступные хранилища
        public async Task<string> MakeBackup() 
        {
            return await MakeBackup(new [] {
                StorageType.Local, 
                StorageType.GoogleDrive, 
                StorageType.YandexDisk
            });
        }

        public async Task<string> MakeBackup(StorageType storageType) 
        {
            return await MakeBackup(new [] {storageType});
        }

        public async Task<string> MakeBackup(StorageType[] storageTypes) 
        {
            foreach (var storageType in storageTypes) 
            {
                switch (storageType) 
                {
                    case StorageType.Local:
                        SaveToLocalStorage();
                        break;
                    case StorageType.GoogleDrive:
                        SaveToGoogleDrive();
                        break;
                    case StorageType.YandexDisk:
                        SaveToYandexDisc();
                        break;
                }
            }
            return string.Empty;
        }
        private void SaveToLocalStorage() 
        {
            for (var i = 0; i < m_DataDirectories.Length; i++) 
            {
                string dataDir = m_DataDirectories[i];
                string backupDir;
                if (m_IsSingleBackupDir) 
                {
                    backupDir = m_BackupDirectories[0];
                } 
                else 
                {
                    backupDir = m_BackupDirectories[i];
                }
                m_ZipPacker.PackAndSave(dataDir, backupDir);
            }
        }
        private void SaveToGoogleDrive() 
        {

        }
        private void SaveToYandexDisc() 
        {

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            ClearTimer();
            return Task.CompletedTask;
        }
        private void ClearTimer() 
        {
            m_Timer?.Dispose();
            m_Timer = null;
            Console.WriteLine("TIMER STOPPED");
        }

    }
}
