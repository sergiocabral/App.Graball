using System;
using System.Diagnostics;
using System.Threading;

namespace Graball.General.IO
{
    public static class ConsoleReadLine
    {
        /// <summary>
        /// Última resposta de Console.ReadLine()
        /// </summary>
        private static string inputLast;

        /// <summary>
        /// Thread que processa as respostas de Console.ReadLine() quando houver oportunidade.
        /// </summary>
        private static Thread inputThread = new Thread(inputThreadAction) { IsBackground = true };

        /// <summary>
        /// Notifica oportunidade para obter resposta de Console.ReadLine().
        /// </summary>
        private static AutoResetEvent inputGet = new AutoResetEvent(false);

        /// <summary>
        /// Notifica que foi obtido resposta de Console.ReadLine().
        /// </summary>
        private static AutoResetEvent inputGot = new AutoResetEvent(false);

        /// <summary>
        /// Construtor.
        /// </summary>
        static ConsoleReadLine()
        {
            inputThread.Start();
        }

        /// <summary>
        /// Processamento da thread que processa as respostas de Console.ReadLine() quando houver oportunidade.
        /// </summary>
        private static void inputThreadAction()
        {
            while (true)
            {
                inputGet.WaitOne();
                inputLast = Console.ReadLine();
                inputGot.Set();
            }
        }

        /// <summary>
        /// Obtem resposta de Console.ReadLine() com timeout.
        /// Se estourar o tempo gera Exception.
        /// </summary>
        /// <param name="timeout">Opcional. Tempo limite de espera.</param>
        /// <returns>Resposta.</returns>
        public static string ReadLine(int timeout = Timeout.Infinite)
        {
            if (timeout == Timeout.Infinite)
            {
                return Console.ReadLine();
            }
            else
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.ElapsedMilliseconds < timeout && !Console.KeyAvailable) ;

                if (Console.KeyAvailable)
                {
                    inputGet.Set();
                    inputGot.WaitOne();
                    return inputLast;
                }
                else
                {
                    throw new TimeoutException("User did not provide input within the timelimit.");
                }
            }
        }

        /// <summary>
        /// Obtem resposta de Console.ReadLine() com timeout.
        /// </summary>
        /// <param name="timeout">Tempo limite de espera.</param>
        /// <param name="defaults">Resposta padrão se estourar o timeout.</param>
        /// <returns>Resposta.</returns>
        public static string ReadLine(int timeout, string defaults)
        {
            try
            {
                return ReadLine(timeout);
            }
            catch
            {
                return defaults;
            }
        }
    }
}
