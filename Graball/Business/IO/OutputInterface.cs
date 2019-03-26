using System.Collections.Generic;

namespace Graball.Business.IO
{
    /// <summary>
    /// Interface para exibidor de informações para o usuário.
    /// </summary>
    public interface OutputInterface
    {
        /// <summary>
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        OutputInterface Write(string format, params object[] arg);

        /// <summary>
        /// Escreve um texto formatado e adiciona nova uma linha.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        OutputInterface WriteLine(string format = "", params object[] args);

        /// <summary>
        /// Exibe um lista para seleção.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Opções.</param>
        /// <param name="format">Formatação da exibição.</param>
        /// <returns>Auto referência para a lista passada.</returns>
        IList<T> WriteOptionsToSelect<T>(IList<T> options, string format = null);
    }
}
