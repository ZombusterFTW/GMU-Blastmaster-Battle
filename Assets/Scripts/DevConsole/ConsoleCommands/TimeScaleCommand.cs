using GMUBMB.Utilities.DevConsole.Commands;

using UnityEngine;


namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New TimeScale Command", menuName = "DevConsole/ConsoleCommands/TimeScale")]
    public class TimeScaleCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {

            if(float.TryParse(args[0], out float value))
            {
                DeveloperConsoleBehavior.instance.SetSavedTimeScale(value);
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Game timescale set to: " + value);
                Debug.Log("Game timescale set to: " + value);
                return true;
            }
            else
            {
                //DeveloperConsoleBehavior.instance.AddMessageToConsole("Failed to parse timescale float");
                Debug.Log("Failed to parse timescale float");
                return false;
            }
        }
    }
}

