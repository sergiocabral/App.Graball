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
                { "Local database", NotImplemented },
                { "Services on the Internet", () => ChooseModule(Properties.Resources.ContextModuleProvider, "Services") },                
                { "Consult WHOIS", Whois },
            });
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

                        Output.WriteLine().WriteRaw(whois.Trim(), '#').WriteLine().WriteLine();
                        Output.WriteRaw(entity.ToString(), '*').WriteLine();
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
