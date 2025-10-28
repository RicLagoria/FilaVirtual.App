namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de audio para Windows
    /// Usa Console.Beep para sonidos simples
    /// </summary>
    public class WindowsAudioService : IAudioService
    {
        /// <summary>
        /// Reproduce sonido de confirmación (éxito)
        /// </summary>
        public async Task PlayConfirmationSoundAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    // Sonido de confirmación: dos beeps cortos
                    Console.Beep(800, 200);
                    Thread.Sleep(50);
                    Console.Beep(1000, 200);
                    System.Diagnostics.Debug.WriteLine("[Audio] Sonido de confirmación reproducido");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio Error] Error reproduciendo confirmación: {ex.Message}");
            }
        }

        /// <summary>
        /// Reproduce sonido de error
        /// </summary>
        public async Task PlayErrorSoundAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    // Sonido de error: beep largo y grave
                    Console.Beep(400, 500);
                    System.Diagnostics.Debug.WriteLine("[Audio] Sonido de error reproducido");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio Error] Error reproduciendo error: {ex.Message}");
            }
        }

        /// <summary>
        /// Reproduce sonido de inicio de grabación
        /// </summary>
        public async Task PlayStartRecordingSoundAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    // Sonido de inicio: beep ascendente
                    Console.Beep(600, 150);
                    Thread.Sleep(50);
                    Console.Beep(800, 150);
                    System.Diagnostics.Debug.WriteLine("[Audio] Sonido de inicio de grabación reproducido");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Audio Error] Error reproduciendo inicio: {ex.Message}");
            }
        }
    }
}
