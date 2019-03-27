using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Graball.Module.Domains
{
    /// <summary>
    /// Operações de banco de dados.
    /// </summary>
    internal static class Database
    {
        /// <summary>
        /// Lista de ações para ajustes na estrutura e invremento da versão do banco.
        /// </summary>
        public static IList<Action<SQLiteConnection>> Structures => new List<Action<SQLiteConnection>>(new Action<SQLiteConnection>[] {
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
