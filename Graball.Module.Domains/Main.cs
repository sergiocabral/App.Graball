using Graball.Business.Module;
using Graball.General.Text;
using System.Reflection;

namespace Graball.Module.Domains
{
    public class Main : ModuleBase
    {
        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public override string Name { get => "Internet domains".Translate(); }

        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            ModuleInterface module;
            do
            {
                module = ChooseModule(Properties.Resources.ContextModuleProvider, "Service providers:");

                if (module != null)
                {
                    module.Run();
                }
            } while (module != null);
        }
    }
}
