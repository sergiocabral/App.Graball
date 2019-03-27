using Graball.Business.Module;
using Graball.General.Text;
using System.Reflection;

namespace Graball.Module.Domains
{
    public class ProviderExpiredDomainsNet : ModuleBase
    {
        /// <summary>
        /// Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public override string Context { get => Properties.Resources.ContextModuleProvider; }

        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public override string Name { get => "expireddomains.net"; }

        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            NotImplemented();
        }
    }
}
