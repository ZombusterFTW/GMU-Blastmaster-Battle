using GMUBMB.Utilities.DevConsole.Commands;


using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New LoadScene Command", menuName = "DevConsole/ConsoleCommands/LoadScene")]
    public class LoadSceneCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (args.Length > 1 || args.Length == 0)
            {
               DeveloperConsoleBehavior.instance.AddMessageToConsole("Enter /loadscene with one argument. Ex: /loadscene 1");
               return true;
            }
            else
            {
                SceneTransitionerManager transitionerManager = SceneTransitionerManager.instance;
                if(int.TryParse(args[0], out int result))
                {
                    if (SceneManager.sceneCountInBuildSettings - 1 >= result && result >= 0)
                    {
                        transitionerManager.LoadGameLevelByIndex(result, true);
                        DeveloperConsoleBehavior.instance.AddMessageToConsole("Scene " + result + " loaded successfully.");
                    }
                    else DeveloperConsoleBehavior.instance.AddMessageToConsole("Scene " + result + " does not exist.");
                }
                else
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole(args[0] + " is not a valid scene. Scenes must be entered as ints, Ex: /loadscene 0. Refer to build settings to see scenes by build index");
                }
                return true;
            }
        }
    }
}

