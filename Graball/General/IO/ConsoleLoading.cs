using System;
using System.Timers;

namespace Graball.General.IO
{
    /// <summary>
    /// Animação de "carregando..." para console.
    /// </summary>
    public static class ConsoleLoading
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        static ConsoleLoading()
        {
            timer = new Timer(100);
            timer.Elapsed += Animation;
        }

        /// <summary>
        /// Caracteres de cada sprite de animação.
        /// </summary>
        private static string sprites = @"-\|/";

        /// <summary>
        /// Posição do sprite atual.
        /// </summary>
        private static int position = 0;

        /// <summary>
        /// Timer.
        /// </summary>
        private static Timer timer;

        /// <summary>
        /// Processa a animação.
        /// </summary>
        /// <param name="sender">Originador do evento.</param>
        /// <param name="e">Informações sobre o evento.</param>
        private static void Animation(object sender, ElapsedEventArgs e)
        {
            if (position >= sprites.Length) { position = 0; }
            var backupForegroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                Console.Write("{0}\b", sprites[position++]);
            }
            catch { }
            Console.ForegroundColor = backupForegroundColor;
        }

        /// <summary>
        /// Define (ou verifica) o console com animação de carregando.
        /// </summary>
        /// <param name="mode">Opcional. Ativa ou desativa. Se não informado não altera o estado atual.</param>
        /// <returns>Estado atual.</returns>
        public static bool Active(bool? mode = null)
        {
            if (mode.HasValue)
            {
                if (mode.Value && !timer.Enabled) {
                    timer.Start();
                }
                else if (!mode.Value && timer.Enabled)
                {
                    timer.Stop();

                    Console.Write("\b \b");
                    position = 0;
                }
            }
            return timer.Enabled;
        }
    }
}