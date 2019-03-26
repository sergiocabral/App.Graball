using System;
using System.Collections.Generic;

namespace Graball.Business.IO
{
    /// <summary>
    /// Gerenciador de inputs
    /// </summary>
    public sealed class InputManager : ManagerBase<InputInterface>, InputInterface
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public string ReadLine()
        {
            string answer = "";
            foreach (var item in Items)
            {
                answer += item.ReadLine() + ". ";
            }
            return answer;
        }
    }
}
