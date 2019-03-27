using Graball.Business.Module;
using Graball.General.Text;
using Graball.Module.Domains.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Reflection;

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
            KeyValuePair<int, string> option;
            do
            {
                option = ChooseOption<string>(new string[]
                {
                    "Search on internet",
                    "Search the local database"
                }, "Operations:");

                switch (option.Key)
                {
                    case 0:
                        CollectDataFromInternet();
                        break;
                    case 1:
                        QueryLocalDatabase();
                        break;
                }
            } while (option.Key >= 0);
        }

        /// <summary>
        /// Opções para carregar dados da internet.
        /// </summary>
        public void CollectDataFromInternet()
        {
            ModuleInterface module;
            do
            {
                module = ChooseModule(Properties.Resources.ContextModuleProvider, "Data origin:");

                if (module != null)
                {
                    module.Run();
                }
            } while (module != null);
        }

        /// <summary>
        /// Operaçoes de consulta ao banco de dados local.
        /// </summary>
        public void QueryLocalDatabase()
        {
            NotImplemented();
        }
    }
}
