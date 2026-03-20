using GMUBMB.Utilities.DevConsole.Commands;


using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Blocks Command", menuName = "DevConsole/ConsoleCommands/Blocks")]
    public class BlocksCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {

            if (SceneManager.GetActiveScene().name != "L01_Arena" && SceneManager.GetActiveScene().name != "L02_Garden" && SceneManager.GetActiveScene().name != "L03_Woods")
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/blocks requires a playable level to be active.");
                return true;
            }


            BombermanGameManager gameManager = GameObject.FindObjectOfType<BombermanGameManager>();
            if (gameManager != null) 
            {
                //This command returns the count of valid blocks for powerup spawns
                if (args.Length == 0)
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole(SceneManager.GetActiveScene().name + " has " + gameManager.blockLocationsNoIndestructibles.Count + " valid block locations");
                    return true;
                }
                else
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /blocks with no arguments.");
                    return true;
                }
            }
            else
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Couldn't find a valid GameManager.");
                return true;
            }
        }
    }
}

