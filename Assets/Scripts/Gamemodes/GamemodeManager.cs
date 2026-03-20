using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This class is created with the intention of being the storage place for the individual scriptable object "gamemodes"

//Will likely merge this class with the bomberman game manager, as limiting powerup spawn types is a req feature for game modes
public class GamemodeManager : MonoBehaviour
{
    public Gamemode_TemplateScriptableObject[] alternateGamemodes;
    public Gamemode_TemplateScriptableObject defaultGamemode;
    public Gamemode_TemplateScriptableObject currentGameMode;
    private int lastIndex = -1;
    public static GamemodeManager instance;
    private EscMenu playerManagerClass;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            currentGameMode = defaultGamemode;
        }
        else
        {
            Debug.Log("More than one Gamemode manager in the scene. Cleanup triggered");
        }
    }

    private void Start()
    {
        playerManagerClass = GameObject.FindGameObjectWithTag("PlayerInputManager").GetComponent<EscMenu>();
        
    }
    public Gamemode_TemplateScriptableObject GetDefaultGamemode()
    {
        currentGameMode = defaultGamemode;
        return currentGameMode; 
    }   
    
    public Gamemode_TemplateScriptableObject GetRandomAltGamemode()
    {
        if(lastIndex == -1)
        {
            lastIndex = Random.Range(0, alternateGamemodes.Length);
            currentGameMode = alternateGamemodes[lastIndex];
        }
        else
        {
            //alternate game modes can never repeat
            int temp = Random.Range(0, alternateGamemodes.Length);
            while (temp == lastIndex)
            {
                temp = Random.Range(0, alternateGamemodes.Length);
            }
            lastIndex = temp;
            currentGameMode = alternateGamemodes[lastIndex];
        }
        return currentGameMode;
    }
}
