namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de reconocimiento de voz
    /// </summary>
    public interface ISpeechRecognitionService
    {
        /// <summary>
        /// Evento que se dispara cuando se reconoce voz
        /// </summary>
        event EventHandler<string>? SpeechRecognized;

        /// <summary>
        /// Evento que se dispara cuando ocurre un error
        /// </summary>
        event EventHandler<string>? ErrorOccurred;

        /// <summary>
        /// Indica si el servicio está escuchando actualmente
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Inicia el reconocimiento de voz
        /// </summary>
        Task StartListeningAsync();

        /// <summary>
        /// Detiene el reconocimiento de voz
        /// </summary>
        Task StopListeningAsync();
    }
}

