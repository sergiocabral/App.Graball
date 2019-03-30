using System;
using System.Data.SQLite;
using System.Reflection;

namespace Graball.General.Data
{
    /// <summary>
    /// Extensão de métodos para DataReader
    /// </summary>
    public static class SQLiteDataReaderExtension
    {
        /// <summary>
        /// Retorna o índice de uma coluna.
        /// </summary>
        /// <param name="reader">Data reader</param>
        /// <param name="field">Nome da coluna.</param>
        /// <returns>Índice</returns>
        public static int GetFieldIndex(this SQLiteDataReader reader, string field)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == field)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
