using GMUBMB.Utilities.DevConsole.Commands;


using UnityEngine;
using UnityEngine.SceneManagement;

namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Spawn Command", menuName = "DevConsole/ConsoleCommands/Spawn")]
    public class SpawnCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if(SceneManager.GetActiveScene().name != "L01_Arena" && SceneManager.GetActiveScene().name != "L02_Garden" && SceneManager.GetActiveScene().name != "L03_Woods") 
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/spawn requires a playable level to be active.");
                return true;
            }

            if (args.Length == 0 || args.Length > 2)
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/spawn requires at least one argument and no more than 2. if blockNumber is not defined a random spot will be chosen. Ex: /spawn bombup blockNumber(int)");
                return true;
            }
            else
            {
                BombermanGameManager gameManager = GameObject.FindObjectOfType<BombermanGameManager>();
                if (gameManager != null)
                {
                    int blockNumber = 0;
                    if (args.Length > 1)
                    {
                        if (int.TryParse(args[1], out int result))
                        {
                            if (gameManager.blockLocationsNoIndestructibles.Count - 1 >= result - 1 && result >= 1)
                            {
                                blockNumber = result;
                            }
                            else
                            {
                                DeveloperConsoleBehavior.instance.AddMessageToConsole("Block " + result + " is not a valid block.");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        blockNumber = Random.Range(0, gameManager.blockLocationsNoIndestructibles.Count);
                    }
                    gameManager.blockLocationsNoIndestructibles[blockNumber - 1] = new Vector3(gameManager.blockLocationsNoIndestructibles[blockNumber - 1].x, 2.5f, gameManager.blockLocationsNoIndestructibles[blockNumber - 1].z);
                    switch (args[0].ToLower())
                    {
                        case "wings":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber-1], false, gameManager.roundManager.wings);
                                break;
                            }
                        case "fireup":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.fireUp);
                                break;
                            }
                        case "bombup":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.bombUp);
                                break;
                            }
                        case "skates":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.skates);
                                break;
                            }
                        case "shield":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.shield);
                                break;
                            }
                        case "gloves":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.gloves);
                                break;
                            }
                        case "lightningbomb":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.lightningBomb);
                                break;
                            }
                        case "piercebomb":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.pierceBomb);
                                break;
                            }
                        case "remotebomb":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.remoteBomb);
                                break;
                            }
                        case "slimebomb":
                            {
                                gameManager.SpawnUntrackedItem(gameManager.blockLocationsNoIndestructibles[blockNumber - 1], false, gameManager.roundManager.slimeBomb);
                                break;
                            }
                        default:
                            {
                                DeveloperConsoleBehavior.instance.AddMessageToConsole(args[0] + " is not a valid powerup type. Valid types are wings, fireup, bombup, skates, shield, gloves, lightningbomb, piercebomb, remotebomb, and slimebomb.");
                                return true;
                            }
                    }
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Spawned " + args[0] + " on block " + blockNumber + " successfully");
                    return true;
                }
                else
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Couldn't find a valid GameManager.");
                    return true;
                }
            }  
        }
    }
}

