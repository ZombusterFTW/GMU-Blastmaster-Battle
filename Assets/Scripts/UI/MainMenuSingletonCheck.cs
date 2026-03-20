using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSingletonCheck : MonoBehaviour
{

    public static MainMenuSingletonCheck instance;


    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null && instance != this)
        {
            Debug.Log("PlayerManager already exists. Deleting clone");
            DestroyImmediate(gameObject);
        }
    }
}
