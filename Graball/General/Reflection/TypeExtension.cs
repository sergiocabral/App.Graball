using System;
using System.Text.RegularExpressions;

namespace Graball.General.Reflection
{
    /// <summary>
    /// Entensão de métodos para: Type
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Retorna o nome do namespace.
        /// </summary>
        /// <param name="type">Tipo.</param>
        /// <returns>Nome descritivo.</returns>
        public static string GetNamespace(this Type type)
        {
            return Regex.Match(type.FullName, @".*(?=\." + type.Name + "$)").Value;
        }
    }
}
