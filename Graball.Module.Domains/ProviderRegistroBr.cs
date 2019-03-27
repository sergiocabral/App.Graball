using Graball.Business;
using Graball.Business.Module;
using System.Collections.Generic;
using System.Reflection;

namespace Graball.Module.Domains
{
    public class ProviderRegistroBr : ModuleBase
    {
        /// <summary>
        /// Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public override string Context { get => Properties.Resources.ContextModuleProvider; }

        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public override string Name { get => "registro.br"; }

        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            do
            {
                switch (ChooseOption<string>(new string[]
                {
                    "View released domain lists",
                    "Consult WHOIS"
                }).Key)
                {
                    case -1:
                        return;
                    case 0:
                        ViewReleasedLists();
                        break;
                    case 1:
                        ConsultWHOIS();
                        break;
                }
            } while (true);
        }

        /// <summary>
        /// Operaçoes de consulta ao banco de dados local.
        /// </summary>
        public void ViewReleasedLists()
        {
            NotImplemented();
        }

        /// <summary>
        /// Operaçoes de consulta ao banco de dados local.
        /// </summary>
        public void ConsultWHOIS()
        {
            NotImplemented();
        }
    }
}