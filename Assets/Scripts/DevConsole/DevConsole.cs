using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMUBMB.Utilities.DevConsole;
using System.Linq;
using GMUBMB.Utilities.DevConsole.Commands;
using System;


//https://www.youtube.com/watch?v=usShGWFLvUk
//WIP integration of a dev console that may or may not be enabled on a release build

//This console will allow players to spawn powerups, change bomb type, increase cap, and other player options
//Will also allow for stage changes, disabling of in-game HUD, and ability to spawn a dev camera
//Add a help command as well

namespace GMUBMB.Utilities.DevConsole
{
    public class DevConsole
    {
        private readonly string prefix;
        private readonly IEnumerable<IConsoleCommand> commands;
        public DevConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public bool ProcessCommand(string inputValue)
        {
            if (!inputValue.StartsWith(prefix)) { return false; }
            inputValue = inputValue.Remove(0, prefix.Length);
            string[] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

           return ProcessCommand(commandInput, args);
        }

        public bool ProcessCommand(string commandInput, string[] args)
        {
            bool commandExecuted = false;
            foreach (var command in commands)
            {
                if(!commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if(command.Process(args))
                {
                    commandExecuted = true;
                    return true;
                }
            }

            if (!commandExecuted)
            {
                return false;
            }
            else return true;
        }
    }
}
    

