using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoader : MonoBehaviour
{
    private GameObject loader;

    public void startLoading()
    {
        loader.SetActive(true);
        gameObject.SetActive(false);
    }

    public void stopLoading()
    {
        loader.SetActive(false);
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        loader = gameObject.transform.parent.gameObject.transform.Find("ButtonLoader").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
