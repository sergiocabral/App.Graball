using System;
using System.IO;

namespace Graball.Business
{
    /// <summary>
    /// Definições em geral de uso do programa.
    /// </summary>
    public static class Definitions { 
    
        /// <summary>
        /// Caminho do executável.
        /// </summary>
        public static DirectoryInfo DirectoryForExecutable { get => new DirectoryInfo(Environment.CurrentDirectory); }

        /// <summary>
        /// Caminho para arquivos gerados para o usuário.
        /// </summary>
        public static DirectoryInfo DirectoryForUserData { get => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, Properties.Resources.DirectoryNameUserData)); }

        /// <summary>
        /// Máscara para localizar arquivos de tradução.
        /// </summary>
        public static string FileMaskForTranslates { get => Properties.Resources.FileMaskTranslate; }

        /// <summary>
        /// Máscara para localizar arquivos de Input.
        /// </summary>
        public static string FileMaskForInput { get => Properties.Resources.FileMaskInput; }

        /// <summary>
        /// Máscara para localizar arquivos de Output.
        /// </summary>
        public static string FileMaskForOutput { get => Properties.Resources.FileMaskOutput; }
    }
}
