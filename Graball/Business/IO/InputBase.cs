using System;

namespace Graball.Business.IO
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
        public string ReadLine()
        {
            return Console.ReadKey(true).Key.ToString();
        }
    }
}
