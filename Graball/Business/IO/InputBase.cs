﻿namespace Graball.Business.IO
{
    /// <summary>
    /// Classe base para recebedor de informações do usuário.
    /// </summary>
    public abstract class InputBase: InputInterface
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public abstract string Read();

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        public abstract bool HasRead();

        /// <summary>
        /// Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public abstract char ReadKey();
    }
}
