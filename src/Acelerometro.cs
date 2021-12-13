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
        player = GameObject.Find("Player");
        _input = player.GetComponent<StarterAssetsInputs>();
        _input.sprint = false;
    }
    void Update()
    {
        movePlayer(Input.acceleration.x, Input.acceleration.y); 
    }

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
