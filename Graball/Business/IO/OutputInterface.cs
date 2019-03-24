namespace Graball.Business.IO
{
    /// <summary>
    /// Interface para exibidor de informações para o usuário.
    /// Implementar esses marcados ao escrever texto:
    /// Pode iniciar a frase ("#Por exemplo") ou conter palavras ("Outro *exemplo* aqui.")
    ///     Título:     ^
	///     Destacado:  *
	///     Detalhe:    _
	///     Dica:       #
	///     Erro:       !
	///     Pergunta:   ?
	///     Resposta:   @
	///     Nova linha: \n
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
    }
}
