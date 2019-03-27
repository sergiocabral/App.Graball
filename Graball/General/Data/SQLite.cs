using System;
using System.Collections.Generic;
using System.IO;

namespace Graball.General.Data
{
    /// <summary>
    /// Banco de dados SQLite
    /// </summary>
    public class SQLite
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="filename">Arquivo em disco.</param>
        public SQLite(string filename)
        {
            Filename = filename;

            var directory = new FileInfo(filename).Directory;
            if (!directory.Exists) { directory.Create(); }

            Connection = new System.Data.SQLite.SQLiteConnection($"Data Source={filename}");
            Connection.Open();
            CreateInitialStructure();
        }

        /// <summary>
        /// Nome do arquivo.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Conexão.
        /// </summary>
        public System.Data.SQLite.SQLiteConnection Connection { get; }

        /// <summary>
        /// Lista de ações para ajustes na estrutura e invremento da versão do banco.
        /// </summary>
        private IList<Action<System.Data.SQLite.SQLiteConnection>> Structures { get; } = new List<Action<System.Data.SQLite.SQLiteConnection>>();

        /// <summary>
        /// Adiciona estrutura para ser criada e incrementar a versão do banco.
        /// </summary>
        /// <param name="createStructure"></param>
        public void AddStructure(Action<System.Data.SQLite.SQLiteConnection> createStructure)
        {
            Structures.Add(createStructure);
            if (Structures.Count > Version)
            {
                createStructure(Connection);
                Version++;
            }
        }

        /// <summary>
        /// Versão atual do banco de dados.
        /// </summary>
        public int Version
        {
            get
            {
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = "SELECT version FROM database";
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
            private set
            {
                using (var command = Connection.CreateCommand())
                {
                    if (value == 0)
                    {
                        command.CommandText = @"INSERT INTO database (version) VALUES(:version);";
                    }
                    else
                    {
                        command.CommandText = @"UPDATE database SET version = :version;";
                    }

                    var parameterVersion = command.CreateParameter();
                    parameterVersion.ParameterName = "version";
                    parameterVersion.DbType = System.Data.DbType.Int32;
                    parameterVersion.Value = value;
                    command.Parameters.Add(parameterVersion);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Cria a estrutura inicial do banco de dados.
        /// </summary>
        /// <returns>Versão atual do banco de dados.</returns>
        private int CreateInitialStructure()
        {
            using (var command = Connection.CreateCommand())
            {
                try
                {
                    return Version;
                }
                catch
                {
                    command.CommandText = @"CREATE TABLE database (version INT);";
                    command.ExecuteNonQuery();

                    Version = 0;

                    return 0;
                }
            }
        }
    }
}
