using System;

namespace CsvImport
{
    public class ImporterException : Exception
    {
        public ImporterException(string message)
            : base(message)
        {
        }
    }
}