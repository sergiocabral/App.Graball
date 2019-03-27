using System;
using System.Collections.Generic;
using System.Text;

namespace Graball.Module.Domains.Data
{
    /// <summary>
    /// Interface para campos de tabela: Domain
    /// </summary>
    public interface EntityDomainInterface
    {
        /// <summary>
        /// Nome completo do domínio.
        /// </summary>
        string Fullname { get; }

        /// <summary>
        /// Nome antes do primeiro ponto.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sufixo apos o primeiro ponto.
        /// </summary>
        string Suffix { get; }

        /// <summary>
        /// Sufixo depois do último ponto.
        /// </summary>
        string TLD { get; }

        /// <summary>
        /// Comprimento do nome.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Status de disponibilidade.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Última atualização na tabela.
        /// </summary>
        DateTime Updated { get; }
    }
}