using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Xml;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Text;

namespace Chatbot
{
    internal sealed class App : Singleton<App>
    {
        private SpeechSynthesizer synthesizer;
        private SpeechRecognitionEngine recognizer;
        private Dictionary<string, string> commands = new Dictionary<string, string>();
        private Dictionary<string, string> answers = new Dictionary<string, string>();
        private bool speaking = false;
        private ICommander commander = null;

        protected override void Awake()
        {
            base.Awake();

            Debug.Listeners.Add(
                new TextWriterTraceListener(Console.Out));

            Log.Initialize(
                ConfigurationManager.AppSettings["LogPath"],
                nameof(Chatbot),
                (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["LogLevel"]),
                (level, message) => Debug.WriteLine(message));
        }

        public bool Start()
        {
            try
            {
                synthesizer = new SpeechSynthesizer();
                foreach (var voice in synthesizer.GetInstalledVoices())
                {
                    var info = voice.VoiceInfo;

                    Console.WriteLine($"Instaled Synthesizers");
                    Console.WriteLine($"  Name          {info.Name}");
                    Console.WriteLine($"  Culture       {info.Culture}");
                    Console.WriteLine($"  Age           {info.Age}");
                    Console.WriteLine($"  Gender        {info.Gender}");
                    Console.WriteLine($"  Description   {info.Description}");
                    Console.WriteLine($"  ID            {info.Id}");
                    Console.WriteLine($"  Enabled       {voice.Enabled}");

                    if (info.SupportedAudioFormats.Count != 0)
                    {
                        var aformats = new StringBuilder();
                        for (int i = 0; i < info.SupportedAudioFormats.Count; ++i)
                        {
                            aformats.Append(info.SupportedAudioFormats[i].EncodingFormat.ToString());
                            aformats.Append((i == info.SupportedAudioFormats.Count - 1) ? string.Empty : ", ");
                        }

                        Console.WriteLine($"  Audio formats {aformats}");

                    }
                    else
                    {
                        Console.WriteLine($"  No supported audio formats found");
                    }
                }

                Console.WriteLine("\nInstalled Recognizers");
                foreach (var installed in SpeechRecognitionEngine.InstalledRecognizers())
                    Console.WriteLine($"  {installed.Id}");

                recognizer = new SpeechRecognitionEngine("SR_MS_ko-KR_TELE_11.0");
                recognizer.LoadGrammar(new Grammar(Tools.FullPath("App.command")));
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.SpeechRecognitionRejected += OnSpeechRecognitionRejected;
                recognizer.SpeechRecognized += OnSpeechRecognized;
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                SetupCommands();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                recognizer = null;
            }

            return (recognizer != null);
        }

        private void OnSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            try
            {
                if (speaking == true)
                    return;

                if (answers.TryGetValue("unknown", out var answer) == false)
                    return;

                Speak(answer);
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString());
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                Log.Debug(e.Result.Text);

                if (speaking == true)
                {
                    Log.Debug("SKIP: speaking...");
                    return;
                }

                if (commands.TryGetValue(e.Result.Text.Trim(), out var command) == false)
                {
                    Log.Debug("SKIP: no command");
                    return;
                }

                if (commander != null)
                {
                    commander = commander.Command(command);
                }
                else
                {
                    if (answers.TryGetValue(command, out var answer))
                    {
                        Speak(answer);
                    }
                    else
                    {
                        commander = command switch
                        {
                            "power"     => new CommanderPower().Command(command),
                            "notepad"   => new CommanderNotepad().Command(command),
                            "explorer"  => new CommanderExplorer().Command(command),
                            _           => null
                        };
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.ToString());
            }
        }

        public void Speak(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                if (synthesizer != null)
                    synthesizer.Dispose();

                speaking = true;
                synthesizer = new SpeechSynthesizer();
                synthesizer.Rate = int.Parse(ConfigurationManager.AppSettings["SpeechRate"]);
                synthesizer.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
                synthesizer.SetOutputToDefaultAudioDevice();
                synthesizer.SpeakAsync(message.Replace("\n", " "));
                synthesizer.SpeakCompleted += (sender, args) =>
                {
                    // TODO: RecognizeAsyncStop 시키고 다시 Start 하는 방식으로 고민해보자.

                    Task.Delay(1000).ContinueWith(t =>
                    {
                        speaking = false;
                    });
                };

                // NOTE: 아래 방식으로 하면 소리가 가끔 출력이 안되는 경우가 있다.
                // synthesizer.SpeakAsyncCancelAll();
                // synthesizer.SpeakAsync(message);
            }
            catch (Exception exception)
            {
                speaking = false;
                Log.Error(exception.ToString());
            }
        }

        public void Stop()
        {
            if (synthesizer != null)
            {
                synthesizer.Dispose();
                synthesizer = null;
            }

            if (recognizer != null)
            {
                recognizer.Dispose();
                recognizer = null;
            }
        }

        private void SetupCommands()
        {
            var doc = new XmlDocument();
            doc.Load(Tools.FullPath("App.command"));
            var root = doc["grammar"].GetAttribute("root");
            var request = Tools.FindXmlNodes(doc, "grammar/rule").Find(x => string.Equals(x.Attributes["id"].Value, root, StringComparison.OrdinalIgnoreCase));
            if (request != null)
            {
                var items = Tools.FindXmlNodes(request, "one-of/item");
                foreach (var item in items)
                {
                    var command = string.Empty;
                    var combinations = new List<List<string>>();
                    foreach (XmlNode i in item)
                    {
                        if (string.Equals(i.Name, "tag", StringComparison.OrdinalIgnoreCase))
                        {
                            command = i.InnerText;
                        }
                        else if (string.Equals(i.Name, "ruleref", StringComparison.OrdinalIgnoreCase))
                        {
                            var combination = new List<string>();
                            var uri = i.Attributes["uri"].Value.TrimStart('#');
                            var found = Tools.FindXmlNodes(doc, "grammar/rule").Find(x => string.Equals(x.Attributes["id"].Value, uri, StringComparison.OrdinalIgnoreCase));
                            var rules = Tools.FindXmlNodes(found, "one-of/item");

                            foreach (XmlNode rule in rules)
                                combination.Add(rule.InnerText);

                            combinations.Add(combination);
                        }
                        else if (i.Name == "#text")
                        {
                            combinations.Add(new List<string> { i.Value });
                        }
                    }
                    SetupCommands(combinations, command, string.Empty, 0);
                }
            }

            var answer = Tools.FindXmlNodes(doc, "grammar/rule").Find(x => string.Equals(x.Attributes["id"].Value, "answer", StringComparison.OrdinalIgnoreCase));
            if (answer != null)
            {
                var items = Tools.FindXmlNodes(answer, "one-of/item");
                foreach (var item in items)
                {
                    var command = string.Empty;
                    var message = string.Empty;

                    foreach (XmlNode i in item)
                    {
                        if (i.Name == "tag")
                            command = i.InnerText;
                        else if (i.Name == "#text")
                            message = i.Value;
                    }

                    try
                    {
                        answers.Add(command, message);
                        Log.Debug("answer: [{0}] {1}", command, message);
                    }
                    catch (Exception e)
                    {
                        Log.Error("[{0}] {1}", command, message);
                        Log.Error(e.ToString());
                    }
                }
            }
        }

        private void SetupCommands(List<List<string>> combinations, string command, string message, int index)
        {
            if (combinations != null && index < combinations.Count)
            {
                foreach (var combination in combinations[index])
                {
                    SetupCommands
                    (
                        combinations, command,
                        message + (string.IsNullOrEmpty(combination) ? string.Empty : $" {combination}"),
                        index + 1
                    );
                }
            }
            else
            {
                try
                {
                    message = message.Trim();
                    if (string.IsNullOrEmpty(message))
                        throw new Exception("empty message");

                    commands.Add(message, command);
                    Log.Debug("request: [{0}] {1}", command, message);
                }
                catch (Exception e)
                {
                    Log.Error("[{0}] {1}", command, message.Trim());
                    Log.Error(e.ToString());
                }
            }
        }

        private static void Main(string[] args)
        {
            if (i.Start() == false)
                return;

            Console.WriteLine("\nType 'exit' to stop this program.\n");

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "exit")
                {
                    i.Stop();
                    break;
                }
                else
                {
                    i.Speak(command);
                }
            }
        }
    }
}
