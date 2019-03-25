using Graball.General.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Graball.Business.IO
{
    /// <summary>
    /// Gerenciador de outputs
    /// </summary>
    public sealed class OutputManager: ManagerBase<OutputInterface>, OutputInterface
    {
        /// <summary>
        /// Fila de mensagens não exibidas por não haver output.
        /// </summary>
        private IList<KeyValuePair<string, object[]>> Queue { get; } = new List<KeyValuePair<string, object[]>>();

        /// <summary>
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public OutputInterface Write(string format, params object[] args)
        {
            return Write(new KeyValuePair<string, object[]>(format, args));
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
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="text">Informações do texto.</param>
        /// <param name="force">Força a escrita. Ignora o Flushed</param>
        /// <returns>Auto referência.</returns>
        private OutputInterface Write(KeyValuePair<string, object[]> text, bool force = false)
        {
            if (!force && (Items.Count == 0 || Prevent))
            {
                Queue.Add(text);
                Flushed = false;
            }
            else
            {
                foreach (var item in Items)
                {
                    item.Write(text.Key, text.Value);
                }
            }
            return this;
        }

        /// <summary>
        /// Evita o output.
        /// </summary>
        public bool Prevent { get; set; }

        /// <summary>
        /// Sinaliza que não há intens para enviar (flush)
        /// </summary>
        public bool Flushed { get; private set; } = false;

        /// <summary>
        /// Solicita a escrita dos itens da fila de mensagens não exibidas.
        /// </summary>
        public void QueueFlush()
        {
            do
            {
                var count = Queue.Count;
                for (int i = 0; i < count; i++)
                {
                    Write(Queue[0], true);
                    Queue.RemoveAt(0);
                }
            } while (Queue.Count > 0);
            Flushed = true;
        }
    }
}
