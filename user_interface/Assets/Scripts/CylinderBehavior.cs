using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class CylinderBehavior : MonoBehaviour
{
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(gameObject.name);
        originalScale = new Vector3(0.11f, 0.002f, 0.11f); //gameObject.transform.localScale; // TODO: solve bug of scale going to 0 on runtime
        gameObject.transform.localScale = originalScale;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSliderUpdate(SliderEventData eventData)
    {
        var sliderValue = eventData.NewValue;
        // Debug.Log(sliderValue.ToString("0.000"));
        gameObject.transform.localScale = new Vector3(originalScale[0] * 2 * sliderValue, originalScale[1], originalScale[2] * 2 * sliderValue);
    }
}
