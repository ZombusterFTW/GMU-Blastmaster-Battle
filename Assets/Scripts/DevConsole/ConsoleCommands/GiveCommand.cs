using GMUBMB.Utilities.DevConsole.Commands;


using UnityEngine;


namespace GMUBMB.Utilities.DevConsole
{
    [CreateAssetMenu(fileName = "New Give Command", menuName = "DevConsole/ConsoleCommands/Give")]
    public class GiveCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            //This command will allow a players' direct ability stats to be changed via the console.

            if (args.Length != 3)
            {
                DeveloperConsoleBehavior.instance.AddMessageToConsole("/give requires 3 arguments. Etc /give 1(playernum int) firelevel(variable string) 20(amount int)");
                return true;
            }
            else
            {
                if(!int.TryParse(args[0], out int playerNumber))
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Player number must be entered as an int. Etc Etc /give 1(playernum int) firelevel(variable string) 20(amount int)");
                    return true;
                }
                PlayerController player = CheckPlayerNumber(playerNumber);
                if (player == null)
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Player " + playerNumber + " does not exist.");
                    return true;
                }
                //DeveloperConsoleBehavior.instance.AddMessageToConsole("Player found successfully.");
                if (!CheckVariable(args[1]))
                {
                    DeveloperConsoleBehavior.instance.AddMessageToConsole(args[1] + " is not a valid variable.");
                    return true;
                }
                string variable = args[1];

                if(variable == "bombcap" || variable == "movespeed" || variable == "firelevel")
                {
                    if (!float.TryParse(args[2], out var amount))
                    {
                        DeveloperConsoleBehavior.instance.AddMessageToConsole("Variable must be entered as a int or float. If the value is entered as a float it will be rounded to an integer(unless you are changing movement speed).");
                        return true;
                    }
                    if(variable != "movespeed") amount = Mathf.RoundToInt(amount);
                    if(variable == "movespeed")
                    {
                        player.moveSpeed_Game = amount;
                    }
                    else if(variable == "bombcap")
                    {
                        player.bombCap_Game = (int)amount;
                    }
                    else if(variable == "firelevel")
                    {
                        player.bombFireLevel_Game = (int)amount;
                    }
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Set Player " + player.GetPlayerID() + "'s " + variable + " to " + amount);
                    return true;
                }
                else
                {
                    string variable2 = args[2];
                    variable2.ToLower();
                    if (variable == "playerpowerup")
                    {
                        if (variable2 == "wings") player.playerPowerup = PlayerPowerup.playerFly;
                        else if (variable2 == "gloves") player.playerPowerup = PlayerPowerup.playerPushBombs;
                        else if (variable2 == "none") player.playerPowerup = PlayerPowerup.None;
                        else
                        {
                            DeveloperConsoleBehavior.instance.AddMessageToConsole(variable2 + " is not a valid player powerup. Valid types are none, wings, and gloves. Etc /give 4 playerpowerup wings");
                            return true;
                        }
                    }
                    else if(variable == "shield")
                    {
                        if (variable2 == "true")
                        {
                            player.hasShield = true;
                            player.ActivateShieldVFX(true);
                        }
                        else if (variable2 == "false")
                        {
                            player.hasShield = false;
                            player.ActivateShieldVFX(false);
                        }
                        else
                        {
                            DeveloperConsoleBehavior.instance.AddMessageToConsole("The shield command must be entered with true or false. Etc /give 4 shield true");
                            return true;
                        }
                    }
                    else if (variable == "bombtype")
                    {
                        if (variable2 == "default") player.playerBombType = BombTypes.DefaultBomb;
                        else if (variable2 == "spike") player.playerBombType = BombTypes.PierceBomb;
                        else if (variable2 == "slide") player.playerBombType = BombTypes.SlideBomb;
                        else if (variable2 == "lightning") player.playerBombType = BombTypes.LightningBomb;
                        else if (variable2 == "remote") player.playerBombType = BombTypes.RemoteBomb;
                        else
                        {
                            DeveloperConsoleBehavior.instance.AddMessageToConsole(variable2 + " is not a valid bombtype. Valid bombtypes are default, spike, slide, lightning, and remote. Etc /give 4 bombtype slide");
                            return true;
                        }
                    }
                    DeveloperConsoleBehavior.instance.AddMessageToConsole("Set Player " + player.GetPlayerID() + "'s " + variable + " to " + variable2);
                    return true;
                }
            }
        }




        private PlayerController CheckPlayerNumber(int playerNum)
        {
            if(playerNum > 4) return null;
            foreach(PlayerController controller in GameObject.FindObjectsOfType<PlayerController>()) 
            {
                if(controller.GetPlayerID() == playerNum) return controller;
            }
            return null;
        }


        private bool CheckVariable(string variableName)
        {
            switch(variableName.ToLower()) 
            {
                default:
                    {
                        return false;
                    }
                case "bombcap":
                    {
                        return true;
                    }
                case "movespeed":
                    {
                        return true;
                    }
                case "bombtype":
                    {
                        return true;
                    }
                case "shield":
                    {
                        return true;
                    }
                case "firelevel":
                    {
                        return true;
                    }
                case "playerpowerup":
                    {
                        return true;
                    }
            }
        }
    }
}

