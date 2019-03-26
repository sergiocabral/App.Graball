using Graball.Business.IO;
using System;
using System.Collections.Generic;

namespace Graball.Input.CommandLine
{
    /// <summary>
    /// Input pela linha de comando.
    /// </summary>
    public class InputCommandLine : InputBase
    {
        public List<string> CommandLineArgs { get; } = new List<string>(Environment.GetCommandLineArgs());

        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
            if (CommandLineArgs.Count == 0)
            {
                return null;
            }
            else
            {
                var answer = CommandLineArgs[0];
                CommandLineArgs.RemoveAt(0);
                return answer;
            }
        }

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead() => CommandLineArgs.Count > 0;
    }
}
