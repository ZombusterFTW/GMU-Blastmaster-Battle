using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PointLightPulse : MonoBehaviour
{
    public Light lightName;
    public float startRange = 12.0f;
    public float endRange = 17.0f;
    public float pulseTime = 5.0f;
    private float currentTarget;
    private Coroutine pulseCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        lightName.range = startRange;
        currentTarget = endRange;
        pulseCoroutine = StartCoroutine(PulseLoop());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PulseLoop()
    {
        while (true)
        {
            DOTween.To(() => lightName.range, x => lightName.range = x, currentTarget, pulseTime);
            yield return new WaitForSeconds(pulseTime);
            if (currentTarget == endRange)
            {
                currentTarget = startRange;
            }
            else
            {
                currentTarget = endRange;
            }

        }


    }

    private void OnDestroy()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
    }
}
