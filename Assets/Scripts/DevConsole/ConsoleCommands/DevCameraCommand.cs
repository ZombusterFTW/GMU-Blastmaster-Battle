using GMUBMB.Utilities.DevConsole.Commands;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New DevCamera Command", menuName = "DevConsole/ConsoleCommands/DevCamera")]
    public class DevCameraCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {


            if (SceneManager.GetActiveScene().name != "L01_Arena" && SceneManager.GetActiveScene().name != "L02_Garden" && SceneManager.GetActiveScene().name != "L03_Woods")
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/devcam requires a playable level to be active.");
                return true;
            }



            if (args.Length == 1)
            {
                if (args[0] == "0")
                {
                    foreach (Billboardable billboarder in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<Billboardable>())
                    {
                        billboarder.ReassignCameraTarget(Camera.main);
                    }
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Dev cam disabled. Player joining is enabled.");
                    foreach(FreeCameraBMB devcam in GameObject.FindObjectsOfType<FreeCameraBMB>())
                    {
                        Destroy(devcam.gameObject);
                    }
                    DeveloperConsoleBehavior.instance.devCamInputManager.DisableJoining();
                    DeveloperConsoleBehavior.instance.gameInputManager.EnableJoining();
                }
                else if (args[0] == "1") 
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Dev cam enabled. Player joining is disabled. Press an input on a newly connected Xbox controller to spawn camera");
                    DeveloperConsoleBehavior.instance.devCamInputManager.EnableJoining();
                    DeveloperConsoleBehavior.instance.gameInputManager.DisableJoining();
                }
                else DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /devcam with 0 or 1 as an argument. Ex. /devcam 0");
                return true;
            }
            else
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /devcam with 0 or 1 as an argument. Ex. /devcam 0");
                return true;
            }
        }
    }
}

