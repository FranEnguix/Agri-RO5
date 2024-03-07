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
        
    }

    void Update(){
        RaycastHit hit;
        Ray pulsation;
        
        //Shows tha agent info panel
        if (Input.GetKeyDown(KeyCode.I)){
            infoWindow.enabled = !infoWindow.enabled;
        }    
        
        //Selects the agent to show info
        if (Input.GetMouseButton(0)){
            pulsation = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(pulsation, out hit)){
                posAgent = hit.collider.transform.parent;
                
            }
        }
        if (posAgent!=null){
            var pos= posAgent.name.IndexOf("@");
            String aux = posAgent.name.Substring(0,pos);
            text2.text = "Name:  " + aux;
            text3.text = "PosX:  " + posAgent.position.x; 
            text4.text = "PosZ:  " + posAgent.position.z;
            text5.text = "Elevation:   " + posAgent.position.y;
            
        }
    
    }
   

}