using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brujula : MonoBehaviour
{
    public GameObject brujulaGUI;
    private Text info;
    float smooth = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.compass.enabled = true;
            Quaternion target = Quaternion.Euler(0, 0, Input.compass.trueHeading);
            brujulaGUI.transform.rotation =  Quaternion.Slerp(brujulaGUI.transform.rotation, target,  Time.deltaTime * smooth);
        }
        else
        {
            Input.location.Start();
        }      
    }
}
