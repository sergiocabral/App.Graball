using System;

namespace Graball.General.Cryptography
{
    /// <summary>
    /// <para>Agrupa informações sobre um algorítmo de criptografia.</para>
    /// </summary>
    public struct AlgorithmInfo
    {
        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Tipo da classe que representa o algorítmo de criptografia.</para>
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Nome que representa o algorítmo de criptografia.</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Quantidade de bytes usado na chave de criptografia.</para>
        /// </summary>
        public int BytesKey { get; }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Quantidade de bytes usado no Vetor de Inicialização (IV).</para>
        /// </summary>
        public int BytesIV { get; }

        /// <summary>
        /// <para>Construtor.</para>
        /// </summary>
        /// <param name="type"><para>Tipo da classe que representa o algorítmo
        /// de criptografia.</para></param>
        /// <param name="name"><para>Nome que representa o algorítmo
        /// de criptografia.</para></param>
        /// <param name="bytesKey"><para>Quantidade de bytes usado na
        /// chave de criptografia.</para></param>
        /// <param name="bytesIV"><para>Quantidade de bytes usado no
        /// Vetor de Inicialização (IV).</para></param>
        public AlgorithmInfo(Type type, string name, int bytesKey, int bytesIV)
        {
            Type = type;
            Name = name;
            BytesKey = bytesKey;
            BytesIV = bytesIV;
        }
    }
}
