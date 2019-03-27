using System;
using System.Data.SQLite;

namespace Graball.General.Data
{
    /// <summary>
    /// Criação de estrutura do banco de dados.
    /// </summary>
    public abstract class SQLiteStructure
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="database">Instância do banco de dados.</param>
        public SQLiteStructure(SQLite database)
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
        public SQLite Database { get; }

        /// <summary>
        /// Lista de estruturas para criar.
        /// </summary>
        public abstract Action<SQLiteConnection>[] ToCreate { get; }
    }
}
