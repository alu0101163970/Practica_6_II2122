using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gps : MonoBehaviour
{
    private GameObject uiSpace;
    private Text GPSInfo;

    IEnumerator Start()
    {   
        uiSpace = GameObject.Find("GPSInfo");
        GPSInfo = uiSpace.GetComponent<Text>();
        GPSInfo.text = "";
        
        // Wait for 5 sec to load the enviroment
        yield return new WaitForSeconds(5);
    
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Is not enable to use the location");
            GPSInfo.text = "Is not enable to use the location";
            yield break;
        }
            
        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(maxWait);
            GPSInfo.text = $"Waiting for location: {maxWait} sec. left";
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            GPSInfo.text = "Unable to determine device location";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            GPSInfo.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            GPSInfo.text = "Localzación: \n"+ 
                            " +Latitud: " + Input.location.lastData.latitude + "\n " +
                            " +Longitud: "+ Input.location.lastData.longitude + "\n " + 
                            " +Altitud: " +Input.location.lastData.altitude + " \n ";
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }
}
