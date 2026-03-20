using GMUBMB.Utilities.DevConsole.Commands;

using UnityEngine;



namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Log Command", menuName = "DevConsole/ConsoleCommands/LogCommand")]
    public class LogCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            string logText = string.Join(" ", args);
            DeveloperConsoleBehavior.instance.AddMessageToConsole(logText);
            Debug.Log(logText);
            return true;
        }


    }
}

