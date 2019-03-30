using Graball.Business.Module;
using Graball.General.IO;
using Graball.General.Web;
using Graball.Module.Domains.Data;
using Graball.Module.Domains.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

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
                { "lista-processo-liberacao.txt", () => ParseList("lista-processo-liberacao.txt") },
                { "lista-processo-competitivo.txt", () => ParseList("lista-processo-competitivo.txt") }
            });
        }

        /// <summary>
        /// Verifica lista
        /// </summary>
        /// <param name="list">Lista</param>
        private void ParseList(string list)
        {
            ConsoleLoading.Active(true);

            var client = new WebClientWithCookie();
            var loaded = client.Load($"https://registro.br/dominio/{list}");

            ConsoleLoading.Active(false);

            var updated = DateTime.MaxValue;
            foreach (Match match in Regex.Matches(loaded.Html, @"(?<=^#.*\s)[0-9]{4}-[0-9]{2}-[0-9]{2}(?=T[0-9]{2}:[0-9]{2}:[0-9]{2})", RegexOptions.Multiline))
            {
                var date = DateTime.ParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (updated > date)
                {
                    updated = date;
                }
            }
            var matches = Regex.Matches(loaded.Html, @"^\w*\.\w*\.br$", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                ConsoleLoading.Active(true);

                var domain = new EntityDomain {
                    Fullname = match.Value,
                    Updated = updated                    
                };

                var status = Database.TableDomain.Value<string>("Status", new Dictionary<string, string>() {
                    { "Fullname", "{0} = {1}" },
                    { "Updated", "{0} >= {1}" }
                }, domain);

                if (!string.IsNullOrWhiteSpace(status))
                {
                    domain.Status = status;
                }
                else
                {
                    domain.Status = Domain.GetStatus(domain.Fullname, domain.TLD).ToString();
                    Database.TableDomain.InsertOrUpdate(domain);
                }

                ConsoleLoading.Active(false);
                Output.WriteLine("#{0} {1}", domain.Fullname.PadRight(40), domain.Status);
            }
            Output.WriteLine();
        }
    }
}