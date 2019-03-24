using System.IO;
using System.Reflection;

namespace Graball.General.Reflection
{
    /// <summary>
    /// Entensão de métodos para: Assembly
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// Retorna o nome descritivo para um assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <returns>Nome descritivo.</returns>
        public static string Description(this Assembly assembly)
        {
            var name = assembly.GetName();
            return string.Format("{0} version {1}.{2}", name.Name, name.Version.Major, name.Version.Minor);
        }

        /// <summary>
        /// Lê um recurso do assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="name">Nome do recurso.</param>
        /// <returns>Conteúdo do recurso.</returns>
        public static string ResourceString(this Assembly assembly, string name)
        {
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = stream != null ? new StreamReader(stream) : null)
            {
                return reader?.ReadToEnd();
            }
        }

        /// <summary>
        /// Lê um recurso do assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="name">Nome do recurso.</param>
        /// <returns>Conteúdo do recurso.</returns>
        public static byte[] ResourceBinary(this Assembly assembly, string name)
        {
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = stream != null ? new BinaryReader(stream) : null)
            {
                return reader?.ReadBytes((int)stream.Length);
            }
        }
    }
}
