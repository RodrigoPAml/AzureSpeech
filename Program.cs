namespace AzureSpeech
{
    class Program
    {
        private static readonly string Key = "KEY_HERE";
        private static readonly string Region = "eastus";

        static async Task Main()
        {
            var speech = new AzureSpeech(Key, Region);

            var voices = await speech.GetVoices();
            var locales = voices
                .Select(x => x.Locale)
                .Where(x => x == "pt-BR" || x == "en-US")
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            Console.WriteLine("Available locales:");
            for (int i = 0; i < locales.Count; i++)
            {
                Console.WriteLine($"({i} - {locales[i]})");
            }

            Console.WriteLine("Select the number of the desired locale: ");
            if (!int.TryParse(Console.ReadLine(), out int localeIndex) || localeIndex < 0 || localeIndex >= locales.Count)
            {
                Console.WriteLine("Invalid index.");
                return;
            }

            string selectedLocale = locales[localeIndex];
            var filteredVoices = voices.Where(x => x.Locale == selectedLocale).ToList();

            Console.WriteLine($"Available voices ({selectedLocale}):");
            for (int i = 0; i < filteredVoices.Count; i++)
            {
                Console.WriteLine($"{i}: {filteredVoices[i].Name}");
            }

            Console.Write("Select the number of the desired voice: ");
            if (!int.TryParse(Console.ReadLine(), out int voiceIndex) || voiceIndex < 0 || voiceIndex >= filteredVoices.Count)
            {
                Console.WriteLine("Invalid index.");
                return;
            }

            speech.SetVoice(filteredVoices[voiceIndex]);

            Console.Write("Enter the text you want the voice to speak: ");
            string text = Console.ReadLine();

            await speech.SpeakToFile(text, "output.wav");

            Console.WriteLine("File 'output.wav' created successfully!");
        }
    }
}


