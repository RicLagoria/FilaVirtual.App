namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de reproducción de audio
    /// </summary>
    public interface IAudioService
    {
        /// <summary>
        /// Reproduce un sonido de confirmación
        /// </summary>
        Task PlayConfirmationSoundAsync();
        
        /// <summary>
        /// Reproduce un sonido de error
        /// </summary>
        Task PlayErrorSoundAsync();
        
        /// <summary>
        /// Reproduce un sonido de inicio de grabación
        /// </summary>
        Task PlayStartRecordingSoundAsync();
    }
}
