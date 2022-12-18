using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition2D : MonoBehaviour
{
    private float speed = 2f;
    void Update ()
    {
        //move
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0f;
        Vector3 oldPosition = transform.position;
        transform.position = Vector3.Lerp(transform.position, mousePosition, speed * Time.deltaTime);
        //rotate 
        Vector3 deltaVector = transform.position - oldPosition;
        float angle = Mathf.Atan2(deltaVector.y, deltaVector.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Vector3 position = transform.position;
        //send position and angle to server
    }
}
