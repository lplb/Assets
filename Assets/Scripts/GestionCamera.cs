using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionCamera : MonoBehaviour {
    public Camera cameraGlobale;
    public Camera cameraPersonnage;
	// Use this for initialization
	void Start () {
        cameraGlobale.enabled = true;
        cameraPersonnage.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("tab")) {
            if (cameraGlobale.enabled) {
                cameraGlobale.enabled = false;
                cameraPersonnage.enabled = true;
            } else {
                cameraGlobale.enabled = true;
                cameraPersonnage.enabled = false;
            }
        }
	}
}
