using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

class Program
{
    static string YourSubscriptionKey = "YourSubscriptionKey";
    static string YourServiceRegion = "YourServiceRegion";
    static SpeechRecognizer speechRecognizer;

    async static Task OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                if (speechRecognitionResult.Text.ToLower().Contains("lista de compras"))
                    await buildShoppingList();
                else if (speechRecognitionResult.Text.ToLower().Contains("piada"))
                    await tellJoke();
                break;
            case ResultReason.NoMatch:
                Console.WriteLine("NÃO FOI POSSÍVEL RECONHECER A FALA");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                Console.WriteLine($"CANCELADO: Motivo={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELADO: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELADO: ErrorDetails={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELADO: Did you set the speech resource key and region values?");
                }
                break;
        }
    }

    async static Task buildShoppingList()
    {
        bool isComplete = false;

        while (!isComplete)
        {
            Console.WriteLine("INFORME OS ITENS DA LISTA DE COMPRAS");
            var listRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

            switch (listRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
                    speechConfig.SpeechSynthesisVoiceName = "pt-BR-AntonioNeural";

                    using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
                    {
                        string text = $"SUA LISTA É: {listRecognitionResult.Text}";
                        await speechSynthesizer.SpeakTextAsync(text);
                    }

                    isComplete = true;
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine("NÃO FOI POSSÍVEL RECONHECER A FALA, TENTE NOVAMENTE");
                    break;
            }
        }
    }

    async static Task tellJoke()
    {
        var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
        speechConfig.SpeechSynthesisVoiceName = "pt-BR-AntonioNeural";

        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
        {
            string text = $"Porque o jacaré levou uma bronca?...Porque ele RÉPTIL de ano";
            await speechSynthesizer.SpeakTextAsync(text);
        }
    }

    async static Task Main(string[] args)
    {
        var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
        speechConfig.SpeechRecognitionLanguage = "pt-BR";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        Console.WriteLine("INFORME O COMANDO");
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        await OutputSpeechRecognitionResult(speechRecognitionResult);
    }
}