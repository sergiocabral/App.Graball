using System;
using System.Collections.Generic;
using System.Text;

namespace Graball.Business.IO
{
    /// <summary>
    /// Interface para gerenciadores de Input ou Output.
    /// </summary>
    public interface ManagerInterface<T>
    {
        /// <summary>
        /// Adiciona um item gerenciado.
        /// </summary>
        /// <param name="item">Item gerenciado.</param>
        void Add(T item);
    }
}
