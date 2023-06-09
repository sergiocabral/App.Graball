﻿namespace Graball.Business.IO
{ 
    /// <summary>
    /// Interface para recebedor de informações do usuário.
    /// </summary>
    public interface InputInterface
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        string Read();

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        bool HasRead();

        /// <summary>
        /// Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        char ReadKey();
    }
}
