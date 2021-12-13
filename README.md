# Práctica 6 de Interfaces Inteligentes. GPS, Brújula y Acelerómetro.

## **Autor**: Francisco Jesús Mendes Gómez.

## **Índice:**  

1. [**Ejecución**](#id1)
2. [**Scripts e Implementación**](#id2)   
3. [**Anotaciones**](#id3)

<div name="id1" />

## 1. Ejecución

![Resultado](./img/resultado.gif)

Esta aplicación es un juego el cual consiste en recorrer un laberinto llevando con el personaje una esféra empujandola hasta el punto final. Para ello deberemos mover el personaje el balanceando el dispositivo.
  
Nota: El dispositivo debe contar con acelerómetro.

También observamos otros elementos como el botón Exit para cerrar la aplicación, una brújula real y las coordenadas de nuestra localización.

<div name="id2" />

## 2. Scripts e Implementación

1. `Acelerometro.cs`: 

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using StarterAssets;
#endif

public class Acelerometro : MonoBehaviour
{
    private GameObject player;
    private StarterAssetsInputs _input;

    void Start()
    {
        //Localizamos el personaje y su sistema de movimiento.
        player = GameObject.Find("Player");
        _input = player.GetComponent<StarterAssetsInputs>();
        _input.sprint = false;
    }
    void Update()
    {
        //Movemos al personaje mapeando los ejes del acelerometro al plano XZ del mundo.
        movePlayer(Input.acceleration.x, Input.acceleration.y); 
    }

    //Función que lleva a cabo el mapeo al sistema de coordenadas XZ del mundo
    void movePlayer(float x,float z)
    {
        if(((x < -1.0f) && (x > 1.0f)) || ((z < -1.0f) && (z > 1.0f)))
        {
            _input.sprint = false;
        }
        else 
        {
            _input.sprint = true;
        }
        _input.move = new Vector2(x, z);
    }
}

```

Este script se encuentra en objeto Acelerómetro y es el encargado de mover el personaje, que, en este caso, es un modelo de la starter asset que cuenta con su propio sistema de movimiento y animaciones.
  

2. `Brujula.cs`: 

```c#
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
        //Si el servicio de localización está corriendo activamos la brújula actualizamos la imagen
        //con la rotación que le corresponde.
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.compass.enabled = true;
            Quaternion target = Quaternion.Euler(0, 0, Input.compass.trueHeading);
            brujulaGUI.transform.rotation =  Quaternion.Slerp(brujulaGUI.transform.rotation, target,  Time.deltaTime * smooth);
        }
        else // Si no esta activado el servicio, entonces lo activamos
        {
            Input.location.Start();
        }      
    }
}

```

Gracias a este script podemos tener en la interfaz de la aplicación una brújula la cual utilizamos el sensor del dispositvo asignado para esta labor y rotamos la imagen de la interfaz con la orientación correspondiente (siempre el norte de la imagen apuntando al norte real). Este se encuentra en el objeto Brújula.
  

3. `Gps.cs`:

```c#
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

```

Con este script logramos mostrar por la pantalla nuestra localización actual al momento de ejecutar la aplicación. Este se encuentra en el objeto GPS.

4. `FinishLab.cs`: 

```c#
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

    // Detectamos las colisiones y comprobamos las condiciones de victoria.
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

    // Imprime el mensaje de felicitaciones
    void PrintMessage()
    {
        info.text = "       Felicitaciones      \n Completaste el Laberinto";
    }
}

```

Este se encuentra en el objeto Finish y es utilizado para mostrarnos por pantalla un mensaje de felicitaciones por terminar el juego.

5. `ExitGame.cs`:

```c#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{    public void CloseAplication()
    {
        Application.Quit();// Esto cierra la aplicación
    }
}

```

Contiene una función para hacer funcionar al botón de salida.

Nota: El script `FollowPlayer.cs` ha sido explicado en prácticas anteriores y ha sido reutilizado en esta práctica.


<div name="id3" />

## 3. Anotaciones

1. Para esta práctica se ha hecho uso de la herramienta `Unity Remote` para iOS, usada para controlar el desarrollo, la ejecución y depurar los fallos de la aplicación.

2. También utilizando el módulo SDK para Android se ha creado un .apk para ejecutarlo desde un movil.

