using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Graball.General.Reflection
{
    /// <summary>
    /// Funções de auxílio relacionados ao assembly.
    /// </summary>
    public static class AssemblyHelper
    {

        /// <summary>
        /// Carrega um assembly em arquivo.
        /// </summary>
        /// <param name="filename">Arquivo do assembly.</param>
        public static Assembly Load(string filename)
        {
            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(filename);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Carrega um assembly e retorna uma classe instanciada.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="typeToCreateInstance">Nome do tipo de instância para se criar.</param>
        /// <returns>Assembly carregado e instância criada (se não informada retorna null).</returns>
        public static object Load(Assembly assembly, string typeToCreateInstance)
        {
            try
            {
                var type = assembly.GetType(typeToCreateInstance);
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Carrega um assembly e retorna as classe de determinado tipo instanciadas.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="type">Tipo de instância para se localizar e criar.</param>
        /// <returns>Lista de assembly carregado e instância criada (se não informada retorna null).</returns>
        public static object[] Load(Assembly assembly, Type type)
        {
            var result = new List<object>();

            foreach (var typeToTest in assembly.GetTypes())
            {
                if (type.IsAssignableFrom(typeToTest))
                {
                    result.Add(Activator.CreateInstance(typeToTest));
                }
            }

            return result.ToArray();
        }
    }
}
