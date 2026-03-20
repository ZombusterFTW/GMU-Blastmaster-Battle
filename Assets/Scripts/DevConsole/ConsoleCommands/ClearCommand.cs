using GMUBMB.Utilities.DevConsole.Commands;

using UnityEngine;


namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Clear Command", menuName = "DevConsole/ConsoleCommands/Clear")]
    public class ClearCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {

            if (args.Length == 0)
            {
                DeveloperConsoleBehavior.instance.ClearDevConsole();
                return true;
            }
            else
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /clear with no arguments.");
                return false;
            }
        }
    }
}

