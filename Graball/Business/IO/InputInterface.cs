namespace Graball.Business.IO
{ 
    /// <summary>
    /// Interface para recebedor de informações do usuário.
    /// </summary>
    public interface InputInterface
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        string ReadLine();
    }
}
