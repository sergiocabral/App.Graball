namespace Graball.Business.IO
{
    /// <summary>
    /// Classe base para exibidor de informações para o usuário.
    /// </summary>
    public abstract class OutputBase: OutputInterface
    {
        /// <summary>
        /// Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="arg">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public abstract OutputInterface Write(string format, params object[] arg);
    }
}
