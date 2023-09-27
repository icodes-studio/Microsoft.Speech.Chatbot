using System.Diagnostics;

namespace Chatbot
{
    interface ICommander
    {
        ICommander Command(string command);
    }

    internal class CommanderPower : ICommander
    {
        public ICommander Command(string command)
        {
            if (command == "poweroff")
            {
                App.i.Speak("정말?");
                return this;
            }
            else if (command == "yes")
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "/h /f";
                psi.FileName = "c:\\windows\\system32\\shutdown.exe";
                Process.Start(psi);
                return null;
            }
            return null;
        }
    }

    internal class CommanderNotepad : ICommander
    {
        public ICommander Command(string command)
        {
            if (command == "notepad")
                Process.Start("notepad.exe");

            return null;
        }
    }

    internal class CommanderExplorer : ICommander
    {
        public ICommander Command(string command)
        {
            if (command == "explorer")
                Process.Start("explorer.exe");

            return null;
        }
    }
}
