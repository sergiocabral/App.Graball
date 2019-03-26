using Graball.Business.Module;
using System.Reflection;

namespace Graball.Input.File
{
    public class ModuleDomains : ModuleBase
    {
        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }
    }
}
