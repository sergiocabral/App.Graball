using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Module.Domains.Data
{
    /// <summary>
    /// Class para campos de tabela: Domain
    /// </summary>
    public class EntityDomain : EntityDomainInterface
    {
        /// <summary>
        /// Nome completo do domínio.
        /// </summary>
        public virtual string Fullname { get; set; }

        /// <summary>
        /// Nome antes do primeiro ponto.
        /// </summary>
        public virtual string Name { get => Regex.Match(Fullname, @"^[^\.]*", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Sufixo apos o primeiro ponto.
        /// </summary>
        public virtual string Suffix { get => Regex.Match(Fullname, @"\..*", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Sufixo depois do último ponto.
        /// </summary>
        public virtual string TLD { get => Regex.Match(Fullname, @"\.[^\.]*$", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Comprimento do nome.
        /// </summary>
        public virtual int Length { get => Name.Length; }

        /// <summary>
        /// Status de disponibilidade.
        /// </summary>
        public virtual string Status { get; set; }

        /// <summary>
        /// Última atualização na tabela.
        /// </summary>
        public virtual DateTime Updated { get; set; } = DateTime.Now;
    }
}
