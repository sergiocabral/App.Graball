using Graball.Business.IO;
using Graball.General.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Business.Module
{
    /// <summary>
    /// Class base para módulos do sistema.
    /// </summary>
    public abstract class ModuleBase: ModuleInterface
    {
        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected abstract Assembly ClassAssembly { get; }

        private string name = null;
        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (name == null)
                {
                    name = Regex.Match(this.GetType().FullName, @"[^\.]*(?=\.[^\.]*$)").Value;
                }
                return name;
            }
        }

        public string translates = null;
        /// <summary>
        /// Traduções em formato JSON.
        /// </summary>
        public virtual string Translates
        {
            get
            {
                if (translates == null)
                {
                    translates = Regex.Match(this.GetType().FullName, @".*(?=\.[^\.]*$)").Value;
                    translates = ClassAssembly.GetResourceString(translates + ".Properties.Translates.json");
                }
                return translates;
            }
        }

        /// <summary>
        /// Output padrão.
        /// </summary>
        protected OutputInterface Output { get; private set; }

        /// <summary>
        /// Define o output padrão.
        /// </summary>
        /// <param name="output">Instância.</param>
        public void SetOutput(OutputInterface output) => Output = output;

        /// <summary>
        /// Input padrão.
        /// </summary>
        protected InputInterface Input { get; private set; }

        /// <summary>
        /// Define o input padrão.
        /// </summary>
        /// <param name="input">Instância.</param>
        public void SetInput(InputInterface input) => Input = input;
    }
}
