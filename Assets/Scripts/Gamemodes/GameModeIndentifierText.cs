using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeIndentifierText : MonoBehaviour
{
    //Stores the text mesh UGUI and the strings that they will be set to show on awake.
    //The prefab has an animator on it automatically.

    public TextMeshProUGUI gameModeTitle;
    public TextMeshProUGUI gameModeDescription;

    public string gameModeDescriptionText;
    public string gameModeTitleText;

    private void Awake()
    {
        gameModeTitle.text = gameModeTitleText;
        gameModeDescription.text = gameModeDescriptionText;
        //Destroy self after 10 seconds.
        Destroy(gameObject, 10);
    }

    public void UpdateText()
    {
        gameModeTitle.text = gameModeTitleText;
        gameModeDescription.text = gameModeDescriptionText;
    }
}
