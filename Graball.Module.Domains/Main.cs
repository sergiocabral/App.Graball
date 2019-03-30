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
                { "Services on the Internet", () => ChooseModule(Properties.Resources.ContextModuleProvider, "Services") },                
                { "Consult WHOIS", Whois },
            });
        }

        /// <summary>
        /// Consulta no banco de dados local
        /// </summary>
        private void LocalDatabase()
        {
            do
            {
                var domain = InputText("Enter your search term:");
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    ConsoleLoading.Active(true);
                    Output.WriteLine();
                    var count = Database.TableDomain.Search(Loop((SQLiteDataReader reader) =>
                    {
                        var entity = new EntityDomain(reader);
                        Output.WriteLine("{2} ** {0} {1}", entity.Fullname.ToLower().Replace(domain.ToLower(), $"*{domain.ToLower()}*").PadRight(40), entity.Status, entity.Status == Domain.Status.Available ? "#" : "");
                    }), new Dictionary<string, string>()
                    {
                        { "Fullname", "LOWER({0}) LIKE LOWER({1})" },
                        { "Status", "{0} <> {1}" }
                    }, new EntityDomain
                    {
                        Fullname = $"%{domain}%",
                        Status = Domain.Status.Registered
                    }, "Fullname ASC");

                    if (count == 0)
                    {
                        Output.WriteLine("#" + "The search returned no results.".Translate());
                    }
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

                        Output.WriteLine().WriteLine("#$" + whois.Trim()).WriteLine();
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
