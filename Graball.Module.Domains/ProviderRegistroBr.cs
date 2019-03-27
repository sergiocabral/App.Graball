using Graball.Business.Module;
using Graball.General.IO;
using Graball.General.Web;
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
                { "View released domain lists", ViewReleasedLists },
                { "Consult WHOIS", NotImplemented }
            });
        }

        /// <summary>
        /// Lista de domínios liberados.
        /// </summary>
        private void ViewReleasedLists()
        {
            ChooseOption(new Dictionary<string, Action>() {
                { "lista-processo-liberacao.txt", () => ShowList("lista-processo-liberacao.txt") },
                { "lista-processo-competitivo.txt", () => ShowList("lista-processo-competitivo.txt") }
            });
        }

        /// <summary>
        /// Exibe a lista de domínios
        /// </summary>
        /// <param name="list">Lista</param>
        private void ShowList(string list)
        {
            ConsoleLoading.Active(true);

            var client = new WebClientWithCookie();
            var loaded = client.Load($"https://registro.br/dominio/{list}");

            ConsoleLoading.Active(false);

            Output.WriteRaw(loaded.Html, '#').WriteLine();
        }
    }
}