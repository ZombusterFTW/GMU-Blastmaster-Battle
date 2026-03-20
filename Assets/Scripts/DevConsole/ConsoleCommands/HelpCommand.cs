using GMUBMB.Utilities.DevConsole.Commands;

using UnityEngine;


namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Help Command", menuName = "DevConsole/ConsoleCommands/Help")]
    public class HelpCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {

            if (args.Length == 0)
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Valid commands are: \n/timescale timescale(float)\n/clear\n/help\n/log message(string)\n/spawn powerupname(string) blocknumber(int)\n/blocks(returns the amount of blocks in a given level)\n/devcam int(1 == on 0 == off)\n/give playerNumber(int) variable(string) variable2(string, int, or float)\n/hud(to toggle hud visibility)\n/loadscene sceneIndex(int)");
                return true;
            }
            else
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /help with no arguments.");
                return false;
            }
        }
    }
}

