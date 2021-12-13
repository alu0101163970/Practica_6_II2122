using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishLab : MonoBehaviour
{
    public GameObject player;
    public GameObject congratulationOnGUI;
    private Text info;
    bool isObjetiveCompleted= false;
    
    // Start is called before the first frame update
    void Start()
    {
        info = congratulationOnGUI.GetComponent<Text>();
        info.text = "";     
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(isObjetiveCompleted)
        {
            if(other.gameObject.name == "Player")
            { 
                PrintMessage();
            }
        }
        if(other.gameObject.name == "Objetivo")
        { 
            isObjetiveCompleted= true;
        }
        
    }

    void PrintMessage()
    {
        info.text = "       Felicitaciones      \n Completaste el Laberinto";
    }
}
