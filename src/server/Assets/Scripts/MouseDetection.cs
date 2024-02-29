using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;


public class MouseDetection : MonoBehaviour
{ 
    private Boolean infoState = false;
	private Canvas infoWindow;
    private GameObject capturedAgent = null;

    private TMP_Text text2, text3, text4, text5;
    private Transform posAgent;
    void Start(){
        infoWindow = GameObject.Find("Data Agent UI").GetComponent<Canvas>();
		infoWindow.enabled = false;

        text2 = GameObject.Find("Text2").GetComponent<TMP_Text>();
        text3 = GameObject.Find("Text3").GetComponent<TMP_Text>();
        text4 = GameObject.Find("Text4").GetComponent<TMP_Text>();
        text5 = GameObject.Find("Text5").GetComponent<TMP_Text>();
        //text2 = GameObject.Find("Text2").GetComponent<TMP_Text>();
    }

    void Update(){
        RaycastHit hit;
        Ray pulsation;

        if (Input.GetKeyDown(KeyCode.I)){
            infoWindow.enabled = !infoWindow.enabled;
            
        }
        if (Input.GetMouseButton(0)){
            pulsation = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(pulsation, out hit))
                Debug.Log("Raycast " + hit.collider.name);
                Debug.Log("Tipo: "+ hit.collider.transform.parent.gameObject.name);
        }
        if (capturedAgent!=null){
            String aux;
            posAgent = capturedAgent.transform;
        //text2 = GameObject.Find("Text2").GetComponent<TMP_Text>();
           // Debug.Log("Dentro update "+ capturedAgent.name);
              
            var pos= capturedAgent.name.IndexOf("@");
            aux = capturedAgent.name.Substring(0,pos);
            text2.text = "Name:  " + aux;
            text3.text = "PosX:  " + posAgent.position.x; 
            text4.text = "PosZ:  " + posAgent.position.z;
            text5.text = "Elevation:   " + posAgent.position.y;
        }
    }

    
    void OnMouseDown()
    {
        String aux;
        
        
        Debug.Log("Pillado 2 " + gameObject.name);
        capturedAgent = gameObject;
        /*
        posAgent = gameObject.transform;
        //text2 = GameObject.Find("Text2").GetComponent<TMP_Text>();
        
              
        var pos= capturedAgent.name.IndexOf("@");
        aux = capturedAgent.name.Substring(0,pos);
        text2.text = "Name:  " + aux;
        text3.text = "PosX:  " + posAgent.position.x; 
        text4.text = "PosZ:  " + posAgent.position.z;
        text5.text = "Elevation:   " + posAgent.position.y;
        */
        //Debug.Log("Pillado6 ");

    }
}