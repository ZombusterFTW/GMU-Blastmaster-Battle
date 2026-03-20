using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeButtonImage : MonoBehaviour
{
    // Start is called before the first frame update
    private Button buttonParent;
    private Image charLogoImage;
    void Start()
    {
        charLogoImage = GetComponent<Image>();  
        buttonParent = GetComponentInParent<Button>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonParent != null) charLogoImage.color = buttonParent.targetGraphic.canvasRenderer.GetColor();
    }
}
