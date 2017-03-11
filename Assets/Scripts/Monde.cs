using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monde : MonoBehaviour {
    public int nbMoutons = 10;
    public int nbLoups = 1;
    public float desiredSeparation = 1;
    public float distanceAlignement = 10;

    List<GameObject> moutons = new List<GameObject>();
    List<GameObject> loups = new List<GameObject>();
    GameObject berger;
    Vector3 mousePos;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < nbMoutons; i++) {
            moutons.Add((GameObject)Instantiate(Resources.Load("Mouton")));
            moutons[i].transform.Translate(new Vector3(3*i,0,5*i));
            moutons[i].GetComponent<Mover>().maxSpeed = 1;
            moutons[i].GetComponent<Mover>().maxForce = 10;
            moutons[i].GetComponent<Mover>().targetRadius = 5;
        }
        for (int i = 0; i < nbLoups; i++) {
            loups.Add((GameObject)Instantiate(Resources.Load("Loup")));
        }
        berger = (GameObject)Instantiate(Resources.Load("Berger"));
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetMouseButton(0)) {
            click();
        }
        updateMoutons();
        updateLoups();
        updateBerger();
    }

    void updateMoutons(){
        alignement(moutons, 0.05f);
        foreach (GameObject mouton in moutons) {
            mouton.GetComponent<Mover>().calculPhys();
            mouton.GetComponent<Mover>().applyForce(mouton.GetComponent<Mover>().arrive(mousePos));
        }
    }

    void updateLoups(){
        foreach (GameObject loup in loups) {
            loup.GetComponent<Mover>().calculPhys();
        }
    }

    void updateBerger() {
        berger.GetComponent<Mover>().arrive(mousePos);
        berger.GetComponent<Mover>().calculPhys();
    }

    void click() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.point != new Vector3(0,0,0)) {
            mousePos = hit.point;
        }
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = mousePos;
    }

    void separation(List<GameObject> groupe) {
        
    }

    void alignement(List<GameObject> groupe, float facteur) {
        foreach (GameObject individu in groupe) {
            Vector3 somme = new Vector3(0,0,0);
            int count = 0;
            foreach (GameObject autreIndividu in groupe) {
                if(autreIndividu != individu) {
                    Vector3 dist = individu.transform.position - autreIndividu.transform.position;
                    if(dist.sqrMagnitude < this.distanceAlignement * this.distanceAlignement) {
                        count++;
                        somme += autreIndividu.GetComponent<Mover>().vel;
                    }
                }
            }
            Vector3 desired = somme / count;
            Vector3 steer = desired.normalized * individu.GetComponent<Mover>().maxSpeed - individu.GetComponent<Mover>().vel;
            if (steer.magnitude > individu.GetComponent<Mover>().maxForce) {
                steer.Normalize();
                steer *= individu.GetComponent<Mover>().maxForce;
            }
            individu.GetComponent<Mover>().applyForce(steer * facteur);
        }
    }

    void flocking(List<GameObject> groupe) {

    }
}
