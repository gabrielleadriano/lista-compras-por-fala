using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

class Program
{
    static string YourSubscriptionKey = "YourSubscriptionKey";
    static string YourServiceRegion = "YourServiceRegion";
    static SpeechRecognizer speechRecognizer;
    static SpeechConfig speechConfig;

    async static Task OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                if (speechRecognitionResult.Text.ToLower().Contains("lista de compras"))
                    await BuildShoppingList();
                else if (speechRecognitionResult.Text.ToLower().Contains("piada"))
                    await TellJoke();
                break;
            case ResultReason.NoMatch:
                Console.WriteLine("NÃO FOI POSSÍVEL RECONHECER A FALA");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                Console.WriteLine($"CANCELADO: Motivo={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELADO: Código={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELADO: Detalhes={cancellation.ErrorDetails}");
                    Console.WriteLine($"CANCELADO: A chave e região estão corretas?");
                }
                break;
        }
    }

    async static Task BuildShoppingList()
    {
        bool isComplete = false;

        while (!isComplete)
        {
            Console.WriteLine("INFORME OS ITENS DA LISTA DE COMPRAS");
            var listRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

            switch (listRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:

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

    async static Task TellJoke()
    {
        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
        {
            string text = $"O que a televisão foi fazer no dentista?...Tratamento de canal";
            await speechSynthesizer.SpeakTextAsync(text);
        }

    }

    async static Task Main(string[] args)
    {
        speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
        speechConfig.SpeechRecognitionLanguage = "pt-BR";
        speechConfig.SpeechSynthesisVoiceName = "pt-BR-AntonioNeural";

        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        Console.WriteLine("INFORME O COMANDO");

        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        await OutputSpeechRecognitionResult(speechRecognitionResult);
    }
}