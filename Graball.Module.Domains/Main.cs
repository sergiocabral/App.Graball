using Graball.Business.Module;
using Graball.General.IO;
using Graball.General.Text;
using Graball.Module.Domains.Data;
using Graball.Module.Domains.Util;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Module.Domains
{
    public class Main : ModuleBase
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public Main()
        {
            Database.SQLite = SQLite;
        }
 
        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public override string Name { get => "Internet domains".Translate(); }

        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Lista de ações para ajustes na estrutura e invremento da versão do banco.
        /// </summary>
        protected override IList<Action<SQLiteConnection>> SQLiteStructures => Database.SQLiteStructures;

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            ChooseOption(new Dictionary<string, Action>() {
                { "Local database", LocalDatabase },                
                { "Consult WHOIS", Whois },
                { "Consult WHOIS with Brute Force", WhoisBruteForce },
                { "Services on the Internet", () => ChooseModule(Properties.Resources.ContextModuleProvider, "Services") },
            });
        }

        /// <summary>
        /// Consulta Whois usando força bruta
        /// </summary>
        private void WhoisBruteForce()
        {
            var mode = ChooseOption<string>(new List<string>() {
                { "WHOIS" },
                { "instantdomainsearch.com" },
            }, "Query modes:").Key;
            if (mode < 0) { return; }

            var suffix = InputText("Enter suffix:").ToLower();
            if (string.IsNullOrWhiteSpace(suffix)) { return; }
            if (suffix[0] != '.')
            {
                suffix = "." + suffix;
            }

            Output.WriteLine();
            var initial = InputText("Initial domain:");
            if (string.IsNullOrWhiteSpace(initial)) { return; }

            Output.WriteLine();
            if (mode == 0)
            {
                WhoisBruteForceWhois(initial, suffix);
            }
            else
            {
                WhoisBruteForceWebsiteInstantDomainSearchCom(initial, suffix);
            }
            Output.WriteLine();
        }

        /// <summary>
        /// Consulta Whois usando força bruta no modo comum. Consultando lotes no WebsiteInstantDomainSearchCom
        /// </summary>
        /// <param name="initial">Nome inicial</param>
        /// <param name="suffix">TLD</param>
        /// <param name="block">Tamanho do bloco para consultar.</param>
        private void WhoisBruteForceWebsiteInstantDomainSearchCom(string initial, string suffix, int block = 1000)
        {
            var list = new List<string>();
            string whois = string.Empty;
            var i = 0;
            Loop((string current) =>
            {
                if (list.Count == 0)
                {
                    while (list.Count < block)
                    {
                        list.Add(current + suffix);
                        current = Domain.Next(current);
                    };
                    whois = Domain.WhoisByWebsiteInstantDomainSearchCom(list.ToArray());
                }
                    
                var entity = new EntityDomain
                {
                    Fullname = list[0],
                    Status = Domain.GetStatus(Regex.Match(whois, list[0].Replace(".", "\\.") + @"[^\}]*", RegexOptions.Singleline).Value),
                    Updated = DateTime.Now
                };
                list.RemoveAt(0);

                Database.TableDomain.InsertOrUpdate(entity);
                Output.Write(entity.ToString(i++ == 0, Domain.Status.Available));

                return current;
            }, initial);
        }

        /// <summary>
        /// Consulta Whois usando força bruta no modo comum. Consultando cada nome no servidor WHOIS
        /// </summary>
        /// <param name="initial">Nome inicial</param>
        /// <param name="suffix">TLD</param>
        private void WhoisBruteForceWhois(string initial, string suffix)
        {
            var i = 0;
            Loop((string current) =>
            {
                var entity = new EntityDomain(current + suffix);
                var values = Database.TableDomain.Values(new Dictionary<string, string> { { "Fullname", "{0} = {1}" } }, entity);

                if (values == null || ((DateTime)values["Updated"]).Date < DateTime.Now.Date.AddDays(-30))
                {
                    var whois = Domain.Whois(entity.Fullname, Domain.WhoisService.Normal);
                    if (whois == null)
                    {
                        Output.WriteLine("!" + "Could not query suffix: {0}".Translate(), suffix.ToUpper());
                        return null;
                    }
                    entity.Status = Domain.GetStatus(whois);
                    entity.Updated = DateTime.Now;
                    Database.TableDomain.InsertOrUpdate(entity);
                }
                else
                {
                    entity.Status = (Domain.Status)Enum.Parse(typeof(Domain.Status), (string)values["Status"]);
                    entity.Updated = (DateTime)values["Updated"];
                }
                Output.Write(entity.ToString(i++ == 0, Domain.Status.Available));
                return Domain.Next(current);
            }, initial);
        }

        /// <summary>
        /// Consulta no banco de dados local
        /// </summary>
        private void LocalDatabase()
        {
            do
            {
                var input = InputText("Enter your search term or length of name:").Split(' ');
                var hasNumber = Regex.IsMatch(input[0], @"^[0-9]{1,2}$");
                var hasText = !hasNumber || input.Length > 1;
                var number = hasNumber ? int.Parse(input[0]) : -1;
                var text = input.Length == 1 ? input[0] : input[1];

                if (!string.IsNullOrWhiteSpace(text))
                {
                    ConsoleLoading.Active(true);
                    Output.WriteLine();

                    var fields = new Dictionary<string, string>() { { "Status", "{0} = {1}" }, };
                    if (hasText) { fields["Fullname"] = "LOWER({0}) LIKE LOWER({1})"; }
                    if (hasNumber) { fields["Length"] = "{0} = {1}"; }

                    var i = 0;
                    var count = 
                        Database.TableDomain.Search(
                            Loop((SQLiteDataReader reader) => {
                                var entity = new EntityDomain(reader);
                                Output.Write(entity.ToString(i++ == 0, Domain.Status.WaitingRelease | Domain.Status.Reserved, hasText ? text : null));
                            }),
                            fields,
                            new EntityDomain
                            {
                                Fullname = text[0] == '.' ? $"%{text}" : $"%{text}%",
                                Length = number,
                                Status = Domain.Status.Available
                            }, 
                            "Fullname ASC");

                    if (i > 0)
                    {
                        Output.WriteLine();
                    }
                    Output.WriteLine("#" + "The search returned {0:n0} results.".Translate(), count);
                    Output.WriteLine();
                }
                else
                {
                    break;
                }
            } while (true);
        }

        /// <summary>
        /// Consulta o Whois de um domínios.
        /// </summary>
        private void Whois()
        {
            do
            {
                var domain = InputText("WHOIS Consultation. Enter the domain:");
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    ConsoleLoading.Active(true);
                    var whois = Domain.Whois(domain);
                    var whoisKeys = Domain.ExtractWhoisKeys(whois);
                    ConsoleLoading.Active(false);
                    if (whois != null)
                    {
                        var entity = new EntityDomain
                        {
                            Fullname = domain,
                            Status = Domain.GetStatus(whois)
                        };
                        Database.TableDomain.InsertOrUpdate(entity);

                        Output.WriteLine().WriteLine("#${0}", whois.Trim()).WriteLine();
                        Output.WriteLine("*$" + entity.ToString());
                    }
                    else
                    {
                        Output.WriteLine().WriteLine("!" + "Query for the TLD {0} not implemented.".Translate(), new EntityDomain { Fullname = domain }.TLD).WriteLine();
                    }
                }
                else
                {
                    break;
                }
            } while (true);
        }
    }
}
