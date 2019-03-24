﻿namespace Graball.Business
{
    /// <summary>
    /// Coleção de frases de uso comum.
    /// </summary>
    public static class Phrases
    {
        /// <summary>
        /// Módulo carregado: {0}
        /// </summary>
        public const string FILE_LOADED_ASSEMBLY = "Module loaded: {0}";

        /// <summary>
        /// Ocorreu um erro ao carregar o arquivo "{0}".
        /// </summary>
        public const string FILE_LOAD_ERROR = "There was an error loading the file \"{0}\".";

        /// <summary>
        /// O conteúdo do arquivo "{0}" não é válido.
        /// </summary>
        public const string FILE_CONTENT_INVALID = "The contents of the file \"{0}\" are not valid.";

        /// <summary>
        /// Ocorreu um erro ao gravar o arquivo \"{0}\".
        /// </summary>
        public const string FILE_WRITE_ERROR = "There was an error writing file \"{0}\".";

        /// <summary>
        /// Ocorreu um erro ao apagar o arquivo \"{0}\".
        /// </summary>
        public const string FILE_DELETE_ERROR = "There was an error deleting file \"{0}\".";
    }
}