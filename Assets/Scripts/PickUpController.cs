﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PickUpController : MonoBehaviour {

    GameObject player;
    string playerState;
	// Use this for initialization
	void Start () {
	    player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
    {
        playerState = player.GetComponent<PlayerController>().stateMachine.getState();
	    switch(player.GetComponent<PlayerController>().stateMachine.getState())
        {
            case "Muskalo":
                //renderer.material.color = Color.green;
                break;
            case "Fox":
                //renderer.material.color = Color.yellow;
                break;
            case "Wolf":
                //renderer.material.color = Color.black;
                break;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.SendMessage("PickUp", SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
