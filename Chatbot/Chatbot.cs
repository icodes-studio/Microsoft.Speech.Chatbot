using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;


namespace Chatbot
{
    public sealed class Chatbot : Singleton<Chatbot>
    {
        public SpeechRecognitionEngine Recognizer
        {
            get;
            private set;
        }

        public SpeechSynthesizer Synthesizer
        {
            get;
            private set;
        }

        public Dictionary<string, string> Commands
        {
            get;
            private set;
        } = new Dictionary<string, string>();

        public Dictionary<string, string> Answers
        {
            get;
            private set;
        } = new Dictionary<string, string>();

        public bool EnableRecognition
        {
            get; set;
        } = true;

        public bool Speaking
        {
            get;
            private set;
        } = false;

        public bool Start()
        {
            try
            {
                Synthesizer = new SpeechSynthesizer();

                foreach (var voice in Synthesizer.GetInstalledVoices())
                {
                    var info = voice.VoiceInfo;

                    Console.WriteLine();
                    Console.WriteLine("Initialize Microsoft Speech");
                    Console.WriteLine("  Name:          " + info.Name);
                    Console.WriteLine("  Culture:       " + info.Culture);
                    Console.WriteLine("  Age:           " + info.Age);
                    Console.WriteLine("  Gender:        " + info.Gender);
                    Console.WriteLine("  Description:   " + info.Description);
                    Console.WriteLine("  ID:            " + info.Id);
                    Console.WriteLine("  Enabled:       " + voice.Enabled);

                    if (info.SupportedAudioFormats.Count != 0)
                    {
                        var aformats = string.Empty;
                        for (int i = 0; i < info.SupportedAudioFormats.Count; ++i)
                        {
                            aformats += String.Format("{0}", info.SupportedAudioFormats[i].EncodingFormat.ToString());
                            aformats += (i == info.SupportedAudioFormats.Count - 1) ? string.Empty : ", ";
                        }

                        Console.WriteLine("  Audio formats: " + aformats);

                    }
                    else
                    {
                        Console.WriteLine("  No supported audio formats found");
                    }

                    var additionals = string.Empty;
                    foreach (var key in info.AdditionalInfo.Keys)
                    {
                        if (string.IsNullOrEmpty(key) == false)
                            additionals += String.Format("\n  {0}: {1}", key, info.AdditionalInfo[key]);
                    }

                    Console.WriteLine();
                    Console.WriteLine("Additional Info" + additionals);
                    Console.WriteLine();
                }

                SetupCommands();

                Console.WriteLine("Installed Recognizers");
                foreach (var ri in SpeechRecognitionEngine.InstalledRecognizers())
                {
                    Console.WriteLine("  {0}: {1}", ri.Id, ri.Culture);
                }

                Recognizer = new SpeechRecognitionEngine("SR_MS_ko-KR_TELE_11.0");
                Recognizer.LoadGrammar(new Grammar(Tools.FullPath("Chatbot.speech")));
                Recognizer.SetInputToDefaultAudioDevice();
                Recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(OnSpeechRecognitionRejected);
                Recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
                Recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Recognizer = null;
            }

            return true;
            //return (Recognizer != null);
        }

        private void OnSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //try
            //{
            //    if (EnableRecognition == false)
            //        return;

            //    if (Speaking == true)
            //        return;

            //    string answer;
            //    if (Answers.TryGetValue("4999", out answer) == false)
            //        return;

            //    Speak(answer);
            //}
            //catch (Exception exception)
            //{
            //    Debug.Log(exception.ToString());
            //}
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                Log.Debug(e.Result.Text);

                if (EnableRecognition == false)
                {
                    Log.Debug("SKIP: turned off");
                    return;
                }

                if (Speaking == true)
                {
                    Log.Debug("SKIP: speaking...");
                    return;
                }

                string command;
                if (Commands.TryGetValue(e.Result.Text.Trim(), out command) == false)
                {
                    Log.Debug("SKIP: no command");
                    return;
                }

                //string answer;
                //Answers.TryGetValue(command, out answer);
                //var notify = Tools.ToJson(new NotifyModuleSpeech()
                //{
                //    command = command,
                //    data = answer
                //});

                //Session.i.Broadcast(notify, null);
            }
            catch (Exception exception)
            {
                Log.Debug(exception.ToString());
            }
        }

        public void Speak(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                if (Synthesizer != null)
                    Synthesizer.Dispose();

                Speaking = true;
                Synthesizer = new SpeechSynthesizer();
                Synthesizer.Rate = int.Parse(ConfigurationManager.AppSettings["SpeechRate"]);
                Synthesizer.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
                Synthesizer.SetOutputToDefaultAudioDevice();
                Synthesizer.SpeakAsync(message.Replace("\n", " "));
                Synthesizer.SpeakCompleted += (sender, args) =>
                {
                    Task.Delay(1000).ContinueWith(t =>
                    {
                        Speaking = false;
                    });
                };

                // NOTE: 아래 방식으로 하면 소리가 가끔 출력이 안되는 경우가 있다.
                // Synthesizer.SpeakAsyncCancelAll();
                // Synthesizer.SpeakAsync(message);
            }
            catch (Exception exception)
            {
                Speaking = false;
                Log.Debug(exception.ToString());
            }
        }

        public void Stop()
        {
            if (Synthesizer != null)
            {
                Synthesizer.Dispose();
                Synthesizer = null;
            }

            if (Recognizer != null)
            {
                Recognizer.Dispose();
                Recognizer = null;
            }
        }

        private void SetupCommands()
        {
            var doc = new XmlDocument();
            doc.Load(Tools.FullPath("Chatbot.speech"));
            var root = doc["grammar"].GetAttribute("root");
            var rule = Tools.FindXmlNodes(doc, "grammar/rule".Split('/')).Find(x => x.Attributes["id"].Value == root);
            if (rule != null)
            {
                var items = Tools.FindXmlNodes(rule, "one-of/item".Split('/'));
                foreach (var item in items)
                {
                    var command = string.Empty;
                    var buckets = new List<List<string>>();
                    foreach (XmlNode i in item)
                    {
                        if (i.Name == "tag")
                        {
                            command = i.InnerText;
                        }
                        else if (i.Name == "ruleref")
                        {
                            var rules = new List<string>();
                            var uri = i.Attributes["uri"].Value.TrimStart('#');
                            var ruleref = Tools.FindXmlNodes(Tools.FindXmlNodes(doc, "grammar/rule".Split('/')).Find(x => x.Attributes["id"].Value == uri), "one-of/item".Split('/'));

                            foreach (XmlNode r in ruleref)
                                rules.Add(r.InnerText);

                            buckets.Add(rules);
                        }
                        else if (i.Name == "#text")
                        {
                            buckets.Add(new List<string>() { i.Value });
                        }
                    }
                    SetupCommands(buckets, command);
                }
            }

            rule = Tools.FindXmlNodes(doc, "grammar/rule".Split('/')).Find(x => x.Attributes["id"].Value == "Answer");
            if (rule != null)
            {
                var items = Tools.FindXmlNodes(rule, "one-of/item".Split('/'));
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
                        Answers.Add(command, message);
                    }
                    catch (Exception e)
                    {
                        Log.Debug("[{0}] {1}", command, message);
                        Log.Debug(e.ToString());
                    }
                }
            }
        }

        private void SetupCommands(List<List<string>> buckets, string command, string message = "", int index = 0)
        {
            if (buckets != null && index < buckets.Count)
            {
                foreach (var item in buckets[index])
                {
                    SetupCommands
                    (
                        buckets, command,
                        message + ((string.IsNullOrEmpty(item) == true) ? string.Empty : (" " + item)),
                        index + 1
                    );
                }
            }
            else
            {
                try
                {
                    var messageT = message.Trim();
                    if (string.IsNullOrEmpty(messageT))
                        throw new Exception("empty message");

                    Commands.Add(messageT, command);

                    //Log.Debug("[{0}] {1}", command, messageT);
                }
                catch (Exception e)
                {
                    Log.Debug("[{0}] {1}", command, message.Trim());
                    Log.Debug(e.ToString());
                }
            }
        }
    }
}
