using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PowerupJumper : MonoBehaviour
{
    private float hoverHeight = .3f;
    private Tween thisTween;
    bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject != null)
        {
            thisTween = gameObject.GetComponentInChildren<SpriteRenderer>().transform.DOPunchPosition(Vector3.up * hoverHeight, 2f, 0, 0).SetLoops(-1);
        }
    }

    public void HoverCleanUp()
    {
        stop = true;
        StopAllCoroutines();
        thisTween.Kill();
    }
}

