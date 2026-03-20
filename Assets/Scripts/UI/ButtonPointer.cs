using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPointer : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image selectedImage;
    public Image selectedImage2;
    private Button button;

    void Start()
    {
        selectedImage.color = new Color(1, 1, 1, 0);
        selectedImage2.color = new Color(1, 1, 1, 0);
        button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        selectedImage.color = new Color(1, 1, 1, 1);
        selectedImage2.color = new Color(1, 1, 1, 1);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectedImage.color = new Color(1, 1, 1, 0);
        selectedImage2.color = new Color(1, 1, 1, 0);
    }
}
