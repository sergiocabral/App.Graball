using Graball.General.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Graball.Module.Domains
{
    /// <summary>
    /// Operações de banco de dados.
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Referência ao banco de dados SQLite.
        /// </summary>
        public static SQLite SQLite { get; set; }

        /// <summary>
        /// Lista de ações para ajustes na estrutura e invremento da versão do banco.
        /// </summary>
        public static IList<Action<SQLiteConnection>> SQLiteStructures => new List<Action<SQLiteConnection>>(new Action<SQLiteConnection>[] {
            (SQLiteConnection connection) =>
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
CREATE
 TABLE domain (
       fullname  TEXT     PRIMARY KEY,
       name      TEXT     NOT NULL,
       suffix    TEXT     NOT NULL,
       tld       TEXT     NOT NULL,
       length    INT      NOT NULL,
       status    TEXT     NOT NULL,
       updated   DATETIME NOT NULL
)
".Trim();
                    command.ExecuteNonQuery();
                }
            }
        });
    }
}
