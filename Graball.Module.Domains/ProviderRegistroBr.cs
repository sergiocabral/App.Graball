using Graball.Business;
using Graball.Business.Module;
using System;
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
            ChooseOption(new Dictionary<string, Action>() {
                { "View released domain lists", NotImplemented },
                { "Consult WHOIS", NotImplemented }
            });
        }
    }
}