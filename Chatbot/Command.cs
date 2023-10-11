using System.Diagnostics;

namespace Chatbot
{
    interface ICommand
    {
        ICommand Work(string command = null);
    }

    internal class CommandPower : ICommand
    {
        public ICommand Work(string command = null)
        {
            if (command == null)
            {
                App.i.Speak("절전모드로 들어갈까요?");
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

    internal class CommandNotepad : ICommand
    {
        public ICommand Work(string command = null)
        {
            Process.Start("notepad.exe");
            App.i.Speak("메모장을 실행했습니다");
            return null;
        }
    }

    internal class CommandExplorer : ICommand
    {
        public ICommand Work(string command = null)
        {
            Process.Start("explorer.exe");
            App.i.Speak("탐색기를 실행했습니다");
            return null;
        }
    }
}
