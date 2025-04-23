using Microsoft.CognitiveServices.Speech;

namespace AzureSpeech.Entities
{
    public class Voice
    {
        public string Name { get; set; }
        public string Locale { get; set; }
        public SynthesisVoiceGender Gender { get; set; }
    }
}
