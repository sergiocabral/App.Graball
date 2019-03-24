using System;
using System.Collections.Generic;

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
            if (Items.Count == 0)
            {
                Queue.Add(new KeyValuePair<string, object[]>(format, args));
            }
            else
            {

            }

            return this;
        }

        /// <summary>
        /// Solicita a escrita dos itens da fila de mensagens não exibidas.
        /// </summary>
        public void QueueFlush()
        {
            var count = Queue.Count;
            for (int i = 0; i < count; i++)
            {
                Write(Queue[0].Key, Queue[0].Value);
                Queue.RemoveAt(0);
            }
        }
    }
}
