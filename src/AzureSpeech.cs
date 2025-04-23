using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using AzureSpeech.Entities;

namespace AzureSpeech
{
    public class AzureSpeech
    {
        private SpeechConfig _speechConfig;
        private readonly string _subscriptionKey;
        private readonly string _region;

        public AzureSpeech(string subscriptionKey, string region)
        {
            _subscriptionKey = subscriptionKey ?? throw new ArgumentNullException(nameof(subscriptionKey));
            _region = region ?? throw new ArgumentNullException(nameof(region));

            InitializeSpeechConfig();
        }

        private void InitializeSpeechConfig()
        {
            _speechConfig = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            _speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);
        }

        public void SetVoice(Voice voice)
        {
            if(voice == null || string.IsNullOrWhiteSpace(voice.Name))
                throw new ArgumentException("Voice cant' be empty");

            _speechConfig.SpeechSynthesisVoiceName = voice.Name;
        }

        public void SetOutputFormat(SpeechSynthesisOutputFormat outputFormat)
        {
            _speechConfig.SetSpeechSynthesisOutputFormat(outputFormat);
        }

        public async Task SpeakToFile(string text, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be empty", nameof(text));

            if (string.IsNullOrWhiteSpace(outputFilePath))
                throw new ArgumentException("Output file path cannot be empty", nameof(outputFilePath));

            using var audioConfig = AudioConfig.FromWavFileOutput(outputFilePath);
            using var synthesizer = new SpeechSynthesizer(_speechConfig, audioConfig);

            var result = await synthesizer.SpeakTextAsync(text);

            if(result.Reason == ResultReason.Canceled)
                throw new Exception("Speech synthesis canceled");
        }

        public async Task<List<Voice>> GetVoices()
        {
            using var synthesizer = new SpeechSynthesizer(_speechConfig, null);
            using var result = await synthesizer.GetVoicesAsync();

            if (result.Reason == ResultReason.VoicesListRetrieved)
            {
                var list = new List<Voice>();

                foreach (var voice in result.Voices)
                {
                    list.Add(new Voice()
                    {
                        Gender = voice.Gender,
                        Locale = voice.Locale,
                        Name = voice.ShortName
                    });
                }

                return list;
            }
            else
            {
                throw new Exception($"Failed to retrieve voices: {result.Reason}");
            }
        }
    }
}