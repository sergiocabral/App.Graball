namespace Graball.Business
{
    /// <summary>
    /// Coleção de frases de uso comum.
    /// </summary>
    public static class Phrases
    {
        /// <summary>
        /// Módulo carregado: {0}
        /// </summary>
        public const string FILE_LOADED_ASSEMBLY = "System component loaded: {0}";

        /// <summary>
        /// Ocorreu um erro ao carregar o arquivo \"{0}\".
        /// </summary>
        public const string FILE_LOAD_ERROR = "There was an error loading the file \"{0}\".";

        /// <summary>
        /// O conteúdo do arquivo \"{0}\" não é válido.
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

        /// <summary>
        /// Escolha um ou deixe em branco para sair: 
        /// </summary>
        public const string CHOOSE_ONE = "Choose or input blank to exit: ";

        /// <summary>
        /// Escolha errada, cara
        /// </summary>
        public const string CHOOSE_WRONG = "Wrong choice, dude";

        /// <summary>
        /// (em branco)
        /// </summary>
        public const string CHOOSE_BLANK = "(blank)";

        /// <summary>
        /// Em desenvolvimento.
        /// </summary>
        public const string NOT_IMPLEMENTED = "Under development.";

        /// <summary>
        /// Pressione qualquer tecla para continuar.
        /// </summary>
        public const string PRESS_ANY_KEY = "Press any key to continue.";

        /// <summary>
        /// Recursos:
        /// </summary>
        public const string RESOURCES = "Resources:";

        /// <summary>
        /// Operações:
        /// </summary>
        public const string OPERATIONS = "Operations:";

        /// <summary>
        /// Para pausar use [P], para parar use [ESC].
        /// </summary>
        public const string LOOP_CONTROL = "To pause use [P], to stop use [ESC].";

        /// <summary>
        /// Execução cancelada pelo usuário.
        /// </summary>
        public const string LOOP_CANCELED= "Execution canceled by user.";

        /// <summary>
        /// Execução pausada pelo usuário. Pressione qualquer tecla para continuar.
        /// </summary>
        public const string LOOP_PAUSED = "Execution paused by the user. Press any key to continue.";
    }
}