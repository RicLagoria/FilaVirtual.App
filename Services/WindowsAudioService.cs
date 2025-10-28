using System.Media;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de audio para Windows
    /// Usa SystemSounds para sonidos del sistema
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
                    SystemSounds.Asterisk.Play();
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
                    SystemSounds.Hand.Play();
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
                    SystemSounds.Question.Play();
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
