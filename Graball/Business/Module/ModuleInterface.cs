using Graball.Business.IO;

namespace Graball.Business.Module
{
    /// <summary>
    /// Interface para módulos do sistema.
    /// </summary>
    public interface ModuleInterface
    {
        /// <summary>
        /// Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        string Context { get; }

        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Traduções em formato JSON.
        /// </summary>
        string Translates { get; }

        /// <summary>
        /// Define o output padrão.
        /// </summary>
        /// <param name="output">Instância.</param>
        void SetOutput(OutputInterface output);

        /// <summary>
        /// Define o input padrão.
        /// </summary>
        /// <param name="input">Instância.</param>
        void SetInput(InputInterface input);

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        void Run();
    }
}
