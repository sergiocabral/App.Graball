using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace Graball.General.Data
{
    /// <summary>
    /// Base para classes que manipulam tabelas.
    /// </summary>
    public class SQLiteTable<TTypeOfIdentity, TTypeOfTable>
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="sqlite">SQLite.</param>
        /// <param name="nameTable">Nome da tabela.</param>
        /// <param name="nameFieldIdentity">Nome do campo identificador.</param>
        public SQLiteTable(SQLite sqlite, string nameOfTable, string nameOfFieldIdentity)
        {
            SQLite = sqlite;
            NameOfTable = nameOfTable;
            NameOfFieldIdentity = nameOfFieldIdentity;
        }

        /// <summary>
        /// SQLite.
        /// </summary>
        protected SQLite SQLite { get; }

        /// <summary>
        /// SQLiteConnection.
        /// </summary>
        protected SQLiteConnection Connection { get => SQLite.Connection; }

        /// <summary>
        /// Nome da tabela
        /// </summary>
        protected string NameOfTable { get; }

        /// <summary>
        /// Nome do campo identificador
        /// </summary>
        protected string NameOfFieldIdentity { get; }

        /// <summary>
        /// Verifica se um registro existe
        /// </summary>
        /// <param name="id">Identificado</param>
        public virtual bool Exists(TTypeOfIdentity id)
        {
            using (var command = Connection.CreateCommand())
            {
                command.AddParameter("id", id);

                command.CommandText = $@"
SELECT COUNT(*) 
  FROM {NameOfTable} 
 WHERE {NameOfFieldIdentity} = :id;
".Trim();

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Apagar registro.
        /// </summary>
        /// <param name="id">Identificado</param>
        public virtual void Delete(TTypeOfIdentity id)
        {
            using (var command = Connection.CreateCommand())
            {
                command.AddParameter("id", id);

                command.CommandText = $@"
DELETE 
  FROM {NameOfTable} 
 WHERE {NameOfFieldIdentity} = :id;
".Trim();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Insere ou atualiza um registro.
        /// </summary>
        /// <param name="entity">Entidade</param>
        /// <returns>Insert code</returns>
        public virtual long? InsertOrUpdate(TTypeOfTable entity)
        {
            using (var command = Connection.CreateCommand())
            {
                PropertyInfo propertyIdentity = null;
                var fields = new List<string>();
                foreach (var property in typeof(TTypeOfTable).GetProperties())
                {
                    command.AddParameter(property.Name, property.GetValue(entity));
                    if (property.Name == NameOfFieldIdentity)
                    {
                        propertyIdentity = property;
                    }
                    fields.Add(property.Name);
                }

                var toInsert = !Exists((TTypeOfIdentity)propertyIdentity.GetValue(entity));
                if (toInsert)
                {
                    command.CommandText = $@"
INSERT 
  INTO {NameOfTable} (
    {string.Join(',', fields.ToArray())}
) VALUES (
    {string.Join(',', fields.Select(a => ":" + a).ToArray())}
);
".Trim();
                }
                else
                {
                    command.CommandText = $@"
UPDATE {NameOfTable}
SET
    {string.Join(',', fields.Select(a => a + " = :" + a).ToArray())}
WHERE
    {propertyIdentity.Name} = :{propertyIdentity.Name};
".Trim();
                }

                command.ExecuteNonQuery();

                return toInsert ? (long?)command.Connection.LastInsertRowId : null;
            }
        }

        /// <summary>
        /// Retorna o valor de um campo do primeiro registro.
        /// </summary>
        /// <param name="select">Campo retornado.</param>
        /// <param name="fields">Campos usados no filtro com sua máscra de comparação.</param>
        /// <param name="entity">Entidade</param>
        /// <param name="conjunction">Junção dos termos comparados: AND ou OR</param>
        /// <returns>Total de registros.</returns>
        public virtual T Value<T>(string select, IDictionary<string, string> fields = null, TTypeOfTable entity = default(TTypeOfTable), string conjunction = "AND")
        {
            using (var command = Connection.CreateCommand())
            {
                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        var property = typeof(TTypeOfTable).GetProperty(field.Key);
                        command.AddParameter(field.Key, property.GetValue(entity));
                    }
                }

                var where = fields == null || fields.Keys.Count == 0 ? "(1 = 1)" : string.Join(conjunction, fields.Select(a => "(" + string.Format(a.Value, a.Key, ":" + a.Key) + ")").ToArray());

                command.CommandText = $@"
SELECT {select}
  FROM {NameOfTable}
 WHERE {where}
 LIMIT 1;
".Trim();

                return (T)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Contabiliza o total de registros.
        /// </summary>
        /// <param name="fields">Campos usados no filtro com sua máscra de comparação.</param>
        /// <param name="entity">Entidade</param>
        /// <param name="conjunction">Junção dos termos comparados: AND ou OR</param>
        /// <returns>Total de registros.</returns>
        public virtual long Count(IDictionary<string, string> fields = null, TTypeOfTable entity = default(TTypeOfTable), string conjunction = "AND")
        {
            using (var command = Connection.CreateCommand())
            {
                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        var property = typeof(TTypeOfTable).GetProperty(field.Key);
                        command.AddParameter(field.Key, property.GetValue(entity));
                    }
                }

                var where = fields == null || fields.Keys.Count == 0 ? "(1 = 1)" : string.Join(conjunction, fields.Select(a => "(" + string.Format(a.Value, a.Key, ":" + a.Key) + ")").ToArray());

                command.CommandText = $@"
SELECT COUNT(*)
  FROM {NameOfTable}
 WHERE {where};
".Trim();

                return Convert.ToInt64(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Localiza domínios.
        /// </summary>
        /// <param name="reader">Méetodo que recebe cada registro. Retorna false para parar.</param>
        /// <param name="orderBy">Texto para o ORDER BY</param>
        /// <param name="entity">Entidade</param>
        /// <param name="fields">Campos usados no filtro com sua máscra de comparação.</param>
        /// <param name="conjunction">Junção dos termos comparados: AND ou OR</param>
        /// <returns>Total de registros.</returns>
        public virtual long Search(Func<SQLiteDataReader, bool> reader, IDictionary<string, string> fields = null, TTypeOfTable entity = default(TTypeOfTable), string orderBy = "1 ASC", string conjunction = "AND")
        {
            long count = 0;
            using (var command = Connection.CreateCommand())
            {
                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        var property = typeof(TTypeOfTable).GetProperty(field.Key);
                        command.AddParameter(field.Key, property.GetValue(entity));
                    }
                }

                var where = fields == null || fields.Keys.Count == 0 ? "(1 = 1)" : string.Join(conjunction, fields.Select(a => "(" + string.Format(a.Value, a.Key, ":" + a.Key) + ")").ToArray());

                command.CommandText = $@"
  SELECT *
    FROM {NameOfTable}
   WHERE {where}
ORDER BY {orderBy};
".Trim();

                using (var dataReader = command.ExecuteReader())
                {
                    var toContinue = true;
                    while (toContinue && dataReader.Read())
                    {
                        var i = -1;
                        toContinue = reader(dataReader);

                        count = toContinue ? count + 1 : Count(fields, entity, conjunction);
                    }
                }
            }
            return count;
        }
    }
}
