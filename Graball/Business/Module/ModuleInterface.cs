using System;
using System.Collections.Generic;
using System.Text;

namespace Graball.Business.Module
{
    /// <summary>
    /// Interface para módulos do sistema.
    /// </summary>
    public interface ModuleInterface
    {
        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Traduções em formato JSON.
        /// </summary>
        string Translates { get; }
    }
}
