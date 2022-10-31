using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class RoomSizeTextBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderUpdate(SliderEventData eventData)
    {
        TextMeshPro mText = GetComponent<TextMeshPro>();
        var sliderValue = eventData.NewValue;
        var realSize = sliderValue * 40;
        var text = realSize.ToString("0.0")+ "m²";
        mText.SetText(text);
        //gameObject.transform.localScale = new Vector3(originalScale[0] * 2 * sliderValue, originalScale[1], originalScale[2] * 2 * sliderValue);
    }
}
