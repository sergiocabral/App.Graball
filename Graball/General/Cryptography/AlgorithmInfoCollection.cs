using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Graball.General.Cryptography
{
    /// <summary>
    /// <para>Contem informações sobre algorítmos de criptografia simétrica.</para>
    /// </summary>
    public class AlgorithmInfoCollection
    {
        private Dictionary<string, AlgorithmInfo> algorithms = new Dictionary<string, AlgorithmInfo>();
        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Lista de algorítmos de criptografia simétrica.</para>
        /// </summary>
        public Dictionary<string, AlgorithmInfo> Algorithms
        {
            get { return new Dictionary<string, AlgorithmInfo>(algorithms); }
        }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Informações sobre o algorítmo de criptografia simétrica especificado.</para>
        /// </summary>
        /// <param name="cryptography">
        /// <para>Tipo do algorítmo de criptografia simétrica.</para>
        /// </param>
        /// <returns>
        /// <para>Retorna a estrutura <see cref="AlgorithmInfo"/>
        /// preenchida com as informações do algorítmo de criptografia simétrica.</para>
        /// </returns>
        public AlgorithmInfo this[Type cryptography]
        {
            get
            {
                Type baseType;
                do
                {
                    baseType = cryptography.BaseType;
                } while (baseType != typeof(SymmetricAlgorithm) && baseType != typeof(object));
                if (baseType == typeof(object))
                {
                    throw new FormatException("Is not a SymmetricAlgorithm.");
                }
                return Algorithms[cryptography.Name];
            }
        }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Informações sobre o algorítmo de criptografia simétrica especificado.</para>
        /// </summary>
        /// <param name="cryptography">
        /// <para>Tipo do algorítmo de criptografia simétrica.</para>
        /// </param>
        /// <returns>
        /// <para>Retorna a estrutura <see cref="AlgorithmInfo"/>
        /// preenchida com as informações do algorítmo de criptografia simétrica.</para>
        /// </returns>
        public AlgorithmInfo this[string cryptography]
        {
            get { return Algorithms[cryptography]; }
        }

        /// <summary>
        /// <para>Construtor.</para>
        /// </summary>
        public AlgorithmInfoCollection()
        {
            AddAlgorithm(
                typeof(DES),
                8,  // Chave de 64 bits
                8); // Vetor de Inicialização (IV) de 64 bits para bloco de 64 bits
            AddAlgorithm(
                typeof(TripleDES),
                24, // Chave de 192 bits
                8); // Vetor de Inicialização (IV) de 64 bits para bloco de 64 bits
            AddAlgorithm(
                typeof(RC2),
                16, // Chave de 128 bits
                8); // Vetor de Inicialização (IV) de 64 bits para bloco de 64 bits
            AddAlgorithm(
                typeof(Rijndael),
                32,  // Chave de 256 bits
                16); // Vetor de Inicialização (IV) de 64 bits para bloco de 128 bits
        }

        /// <summary>
        /// <para>Adiciona na propriedade <see cref="Algorithms"/> um algorítmo de criptografia simétrica.</para>
        /// </summary>
        /// <param name="type">
        /// <para>Tipo da classe que representa o algorítmo de criptografia.</para>
        /// </param>
        /// <param name="bytesKey">
        /// <para>Quantidade de bytes usados na chave de criptografia.</para>
        /// </param>
        /// <param name="bytesIV">
        /// <para>Quantidade de bytes usados no Vetor de Inicialização (IV).</para>
        /// </param>
        private void AddAlgorithm(Type type, int bytesKey, int bytesIV)
        {
            AlgorithmInfo algorithm = new AlgorithmInfo(type, type.Name, bytesKey, bytesIV);
            algorithms.Add(algorithm.Name, algorithm);
        }

    }
}
