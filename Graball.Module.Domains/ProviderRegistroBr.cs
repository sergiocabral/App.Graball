using Graball.Business.Module;
using Graball.General.IO;
using Graball.General.Web;
using Graball.Module.Domains.Data;
using Graball.Module.Domains.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                { "lista-processo-liberacao.txt", () => ParseList("lista-processo-liberacao.txt") },
                { "lista-processo-competitivo.txt", () => ParseList("lista-processo-competitivo.txt") }
            }, "View released domain lists");
        }

        /// <summary>
        /// Verifica lista
        /// </summary>
        /// <param name="list">Lista</param>
        private void ParseList(string list)
        {
            var url = $"https://registro.br/dominio/{list}";

            Output.WriteLine("Requesting url: {0}", url).WriteLine();

            ConsoleLoading.Active(true);

            var client = new WebClientWithCookie();
            var loaded = client.Load(url);

            ConsoleLoading.Active(false);

            Output.WriteLine("Check availability...").WriteLine();

            var updated = DateTime.MaxValue;
            foreach (Match match in Regex.Matches(loaded.Html, @"(?<=^#.*\s)[0-9]{4}-[0-9]{2}-[0-9]{2}(?=T[0-9]{2}:[0-9]{2}:[0-9]{2})", RegexOptions.Multiline))
            {
                var date = DateTime.ParseExact(match.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (updated > date)
                {
                    updated = date;
                }
            }
            Loop((IList<Match> matches) =>
            {
                ConsoleLoading.Active(true);

                var domain = new EntityDomain
                {
                    Fullname = matches[0].Value,
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

                matches.RemoveAt(0);
                if (matches.Count > 0)
                {
                    return matches;
                }
                else
                {
                    return null;
                }
            }, Regex.Matches(loaded.Html, @"^\w*\.\w*\.br$", RegexOptions.Multiline).ToList());
            Output.WriteLine();
        }
    }
}