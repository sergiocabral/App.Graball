using System;

namespace Graball.General.Web
{
    /// <summary>
    /// Conjunto de dados retornados após um carregamento de página.
    /// </summary>
    public struct WebClientWithCookieLoadResult
    {
        /// <summary>
        /// Construtor de inicialização.
        /// </summary>
        /// <param name="url">Endereço.</param>
        /// <param name="success">Indica sucesso.</param>
        /// <param name="html">Código html.</param>
        /// <param name="cookies">Cookies.</param>
        public WebClientWithCookieLoadResult(string url, bool success, string html, string cookies) {
            Url = url;
            Exception = null;
            Success = success;
            Html = html;
            Cookies = cookies;
        }

        /// <summary>
        /// Construtor para indicar falha.
        /// </summary>
        /// <param name="exception"></param>
        public WebClientWithCookieLoadResult(string url, Exception exception)
        {
            Url = url;
            Exception = exception;
            Success = false;
            Html = null;
            Cookies = null;
        }

        /// <summary>
        /// Endereço.
        /// </summary>
        public object Url { get; }

        /// <summary>
        /// Informações sobre o erro.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Indica sucesso.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Código html.
        /// </summary>
        public string Html { get; }

        /// <summary>
        /// Cookies.
        /// </summary>
        public string Cookies { get; }
    }
}
