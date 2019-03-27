using Graball.General.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Graball.Module.Domains.Data
{
    /// <summary>
    /// Operações de banco de dados.
    /// </summary>
    internal static class Database
    {
        public static SQLite sqlite = null;
        /// <summary>
        /// Referência ao banco de dados SQLite.
        /// </summary>
        public static SQLite SQLite
        {
            get => sqlite;
            set
            {
                sqlite = value;
                TableDomain = new SQLiteTable<string, EntityDomainInterface>(sqlite, "Domain", "Fullname");
            }
        }

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
 TABLE Domain (
       Fullname  TEXT     PRIMARY KEY,
       Name      TEXT     NOT NULL,
       Suffix    TEXT     NOT NULL,
       TLD       TEXT     NOT NULL,
       Length    INT      NOT NULL,
       Status    TEXT     NOT NULL,
       Updated   DATETIME NOT NULL
)
".Trim();
                    command.ExecuteNonQuery();
                }
            }
        });

        public static SQLiteTable<string, EntityDomainInterface> TableDomain { get; private set; }
    }
}
