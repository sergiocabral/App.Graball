using Graball.Business.Module;
using System.Reflection;

namespace Graball.Module.Domains
{
    public class ModuleDomains : ModuleBase
    {
        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            string option;
            do
            {
                option = ChoseOption(new string[] {
                    "registro.br",
                    "expireddomains.net",
                    "instantdomainsearch.com"
                }, "Service providers:").Value;
            } while (!string.IsNullOrWhiteSpace(option));
        }
    }
}
