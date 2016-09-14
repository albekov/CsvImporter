namespace CsvImport.Database
{
    public class ImporterDbException : ImporterException
    {
        public ImporterDbException(string message)
            : base("Ошибка работы с базой данных: " + message)
        {
        }
    }
}