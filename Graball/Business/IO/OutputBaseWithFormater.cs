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

        /// <summary>
        /// Exibe um lista para seleção.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Opções.</param>
        /// <param name="format">Formatação da exibição.</param>
        /// <returns>Auto referência para a lista passada.</returns>
        public IList<T> WriteOptionsToSelect<T>(IList<T> options, string format = null)
        {
            format = format ?? " {1}) {0}";
            var padding = options.Count.ToString().Length;
            var i = 0;
            foreach (var option in options)
            {
                string text = null;
                if (typeof(T) != typeof(string) && typeof(IEnumerable).IsInstanceOfType(option))
                {
                    foreach (var first in (IEnumerable)option)
                    {
                        text = Convert.ToString(first);
                        break;
                    }                    
                }
                else
                {
                    text = Convert.ToString(option);
                }
                WriteLine(format, text, (++i).ToString().PadLeft(padding));
            }
            return options;
        }
    }
}
