using Graball.Business.IO;

namespace Graball.Input.Console
{
    /// <summary>
    /// Output para janela de console.
    /// </summary>
    public class InputFile : InputBase
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead()
        {
            return System.Console.KeyAvailable;
        }
    }
}
