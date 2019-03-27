using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Data.SQLite;

namespace Graball.General.Data
{
    /// <summary>
    /// Extensão de métodos para SQLiteCommand
    /// </summary>
    public static class SQLiteCommandExtension
    {
        /// <summary>
        /// Adiciona parâmetro a um comando.
        /// </summary>
        /// <param name="command">Comando.</param>
        /// <param name="name">Nome do parâmetro.</param>
        /// <param name="value">Valor do parâmetro.</param>
        public static void AddParameter(this SQLiteCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

    }
}
