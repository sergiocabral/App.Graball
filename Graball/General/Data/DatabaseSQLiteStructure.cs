using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Graball.General.Data
{
    /// <summary>
    /// Criação de estrutura do banco de dados.
    /// </summary>
    public abstract class DatabaseSQLiteStructure
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="database">Instância do banco de dados.</param>
        public DatabaseSQLiteStructure(DatabaseSQLite database)
        {
            Database = database;
            foreach (var create in ToCreate)
            {
                database.AddStructure(create);
            }
        }

        /// <summary>
        /// Banco de dados.
        /// </summary>
        public DatabaseSQLite Database { get; }

        /// <summary>
        /// Lista de estruturas para criar.
        /// </summary>
        public abstract Action<SQLiteConnection>[] ToCreate { get; }
    }
}
