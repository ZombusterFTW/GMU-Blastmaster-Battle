using GMUBMB.Utilities.DevConsole.Commands;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New HUD Command", menuName = "DevConsole/ConsoleCommands/HUD")]
    public class HUDCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {

            if (SceneManager.GetActiveScene().name != "L01_Arena" && SceneManager.GetActiveScene().name != "L02_Garden" && SceneManager.GetActiveScene().name != "L03_Woods")
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/hud requires a playable level to be active.");
                return true;
            }



            if (args.Length == 0)
            {
                RoundManager roundManager = GameObject.FindObjectOfType<RoundManager>();
                if (roundManager != null)
                {
                    roundManager.inGameUI.enabled = !roundManager.inGameUI.enabled;
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Toggled HUD visibility successfully.");
                    return true;
                }
                else
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Couldn't find a valid RoundManager.");
                    return true;
                }
            }
            else
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /hud with no argument to toggle HUD. Ex: /hud");
                return true;
            }
        }
    }
}

