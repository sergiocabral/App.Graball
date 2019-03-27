namespace Graball.Business.IO
{
    /// <summary>
    /// Gerenciador de inputs
    /// </summary>
    public sealed class InputManager : ManagerBase<InputInterface>, InputInterface
    {
        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public string Read()
        {
            do
            {
                foreach (var item in Items)
                {
                    if (item.HasRead())
                    {
                        return item.Read();
                    }
                }
            } while (true);
        }

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        public bool HasRead()
        {
            foreach (var item in Items)
            {
                if (item.HasRead())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public char ReadKey()
        {
            do
            {
                foreach (var item in Items)
                {
                    if (item.HasRead())
                    {
                        return item.ReadKey();
                    }
                }
            } while (true);
        }
    }
}
