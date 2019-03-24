using Graball.Business.IO;

namespace Graball.Output.Console
{
    /// <summary>
    /// Output para janela de console.
    /// </summary>
    public class OutputConsole : OutputBase
    {
        /// <summary>
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public override OutputInterface Write(string format, params object[] args)
        {
            return this;
        }
    }
}
