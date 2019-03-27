using Graball.General.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Graball.Business.IO
{
    /// <summary>
    /// Classe base para exibidor de informações para o usuário.
    /// </summary>
    public abstract class OutputBaseWithFormater: OutputInterface
    {
        /// <summary>
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public OutputInterface Write(string format, params object[] arg)
        {
            var text = string.Format(format, arg);

            FormaterHelper.Output(text, WriteNow);

            return this;
        }

        /// <summary>
        /// Escreve um texto formatado e adiciona nova uma linha.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public OutputInterface WriteLine(string format = "", params object[] args)
        {
            return Write(format + "\n", args);
        }

        /// <summary>
        /// Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected abstract void WriteNow(string text, char mark = (char)0);
    }
}
