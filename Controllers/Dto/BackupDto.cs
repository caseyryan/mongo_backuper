namespace Controllers.Dto {
    public class BackupDto {
        // при выборе типа каждого из этих типов, нужно считать 
        // соответствующие переменные из конфигов
        public string StorageType { get; set; } // Значение из Enums.StorageType

    }
}