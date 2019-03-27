using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Graball.General.Cryptography
{
    /// <summary>
    /// <para>Disponibiliza funcionalidades relacionadas a criptografia de dados 
    /// com algorítimos de criptografia simétrica.</para>
    /// </summary>
    /// <typeparam name="TSymmetricAlgorithmProvider">Tipo do algoritimo de criptografia simétrica.</typeparam>
    public class CryptographySymmetric<TSymmetricAlgorithmProvider> where TSymmetricAlgorithmProvider : SymmetricAlgorithm
    {        
        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Armazena o total de bytes usado na palavra chave (Key).</para>
        /// </summary>
        public int BytesKey { get; }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Armazena o total de bytes usado no vetor de inicialização (IV).</para>
        /// </summary>
        public int BytesIV { get; }

        /// <summary>
        /// <para>(Leitura/Escrita></para>
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// <para>(Leitura/Escrita></para>
        /// <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </summary>
        public byte[] BytesSalt { get; set; }

        /// <summary>
        /// <para>Construtor padrão.</para>
        /// </summary>
        public CryptographySymmetric() : this(string.Empty, new byte[] { }) { }


        /// <summary>
        /// <para>Construtor padrão.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        /// <para>Bytes usados na derivação da chave de criptografia que foi informada pelo usuário.</para>
        /// </param>
        public CryptographySymmetric(string key, byte[] bytesSalt)
            : this(
            key,
            bytesSalt,
            (new AlgorithmInfoCollection())[typeof(TSymmetricAlgorithmProvider)].BytesKey,
            (new AlgorithmInfoCollection())[typeof(TSymmetricAlgorithmProvider)].BytesIV)
        { }

        /// <summary>
        /// <para>Construtor padrão.</para>
        /// <para>Informa o comprimento de bytes usado na criptografia.</para>
        /// </summary>
        /// <param name="bytesKey">
        /// <para>Total de bytes usados na palavra chave (Key) do algorítmo de criptografia.</para>
        /// </param>
        /// <param name="bytesIV">
        /// <para>Total de bytes usados no vetor de inicialização (IV) do algorítmo de criptografia.</para>
        /// </param>
        public CryptographySymmetric(int bytesKey, int bytesIV)
            : this(string.Empty, new byte[] { }, bytesKey, bytesIV) { }

        /// <summary>
        /// <para>Construtor padrão.</para>
        /// <para>Informa o comprimento de bytes usado na criptografia.</para>
        /// </summary>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        /// <para>Bytes usados na derivação da chave de criptografia que foi informada pelo usuário.</para>
        /// </param>
        /// <param name="bytesKey">
        /// <para>Total de bytes usados na palavra chave (Key) do algorítmo de criptografia.</para>
        /// </param>
        /// <param name="bytesIV">
        /// <para>Total de bytes usados no vetor de inicialização (IV) do algorítmo de criptografia.</para>
        /// </param>
        public CryptographySymmetric(string key, byte[] bytesSalt, int bytesKey, int bytesIV)
        {
            Key = key;
            BytesSalt = bytesSalt;
            BytesKey = bytesKey;
            BytesIV = bytesIV;
        }

        /// <summary>
        /// <para>Criptografa ou descriptografa uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <param name="text">
        /// <para>Texto de entrada.</para>
        /// </param>
        /// <returns>
        /// <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text)
        {
            return Apply(toCryptography, text, Key, BytesSalt);
        }

        /// <summary>
        /// <para>Criptografa ou descriptografa uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <param name="text">
        /// <para>Texto de entrada.</para>
        /// </param>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <returns>
        /// <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text, string key)
        {
            return Apply(toCryptography, text, key, BytesSalt);
        }

        /// <summary>
        /// <para>Criptografa ou descriptografa uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <param name="text">
        /// <para>Texto de entrada.</para>
        /// </param>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        /// <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </param>
        /// <returns>
        /// <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text, string key, byte[] bytesSalt)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ICryptoTransform cryptoTransform = GetCryptoTransform(toCryptography, key, bytesSalt);
                CryptoStream cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);

                if (toCryptography)
                {
                    cryptoStream.Write(Encoding.Default.GetBytes(text), 0, text.Length);
                }
                else
                {
                    byte[] arrayTexto = Convert.FromBase64String(text);
                    cryptoStream.Write(arrayTexto, 0, arrayTexto.Length);
                }

                cryptoStream.FlushFinalBlock();

                if (toCryptography)
                {
                    return Convert.ToBase64String(ms.ToArray());
                }
                else
                {
                    return Encoding.Default.GetString(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// <para>Obtem uma instância de uma classe devidamente configurada
        /// para realizar a des/criptografia.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <returns>
        /// <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform"/>.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography)
        {
            return GetCryptoTransform(toCryptography, Key, BytesSalt);
        }

        /// <summary>
        /// <para>Obtem uma instância de uma classe devidamente configurada
        /// para realizar a des/criptografia.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <returns>
        /// <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform"/>.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography, string key)
        {
            return GetCryptoTransform(toCryptography, key, BytesSalt);
        }

        /// <summary>
        /// <para>Obtem uma instância de uma classe devidamente configurada
        /// para realizar a des/criptografia.</para>
        /// </summary>
        /// <param name="toCryptography">
        /// <para>Quando igual a <c>true</c>, define o processo como Criptografia.
        /// Mas se for igual a <c>false</c>, define como Descriptografia.</para>
        /// </param>
        /// <param name="key">
        /// <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        /// <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </param>
        /// <returns>
        /// <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform"/>.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography, string key, byte[] bytesSalt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(key, bytesSalt);

            System.Reflection.MethodInfo method = typeof(TSymmetricAlgorithmProvider).GetMethod("Create", new Type[] { });
            TSymmetricAlgorithmProvider algoritmo = (TSymmetricAlgorithmProvider)method.Invoke(typeof(TSymmetricAlgorithmProvider), new object[] { });
            algoritmo.Key = pdb.GetBytes(BytesKey);
            algoritmo.IV = pdb.GetBytes(BytesIV);
            if (toCryptography)
            {
                return algoritmo.CreateEncryptor();
            }
            else
            {
                return algoritmo.CreateDecryptor();
            }
        }
    }
}
