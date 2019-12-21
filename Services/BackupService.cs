using System;
using System.Threading;
using System.Threading.Tasks;
using Enums;
using Microsoft.Extensions.Hosting;

namespace Services {
    /// служба для периодического создания резервных копий директории
    public class BackupService : IHostedService
    {
        private Timer m_Timer;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: заменить 1 на запрос времени из конфига и сделать интервал достаточно большим
            ClearTimer();
            m_Timer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }
        
        private void OnTimer(object state) 
        {
            MakeBackup();
        }

        public async Task<string> MakeBackup(StorageType storageType = StorageType.Local) 
        {
            Console.WriteLine($"MAKING A BACKUP NOW AT {storageType}");
            return string.Empty;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            ClearTimer();
            return Task.CompletedTask;
        }
        private void ClearTimer() {
            m_Timer?.Dispose();
            m_Timer = null;
            Console.WriteLine("TIMER STOPPED");
        }

    }
}
