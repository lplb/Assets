﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monde : MonoBehaviour {
    public int nbMoutons = 10;
    public int nbLoups = 5;
	public float distanceAlignement = 10;
	public float distanceSep = 10;
    public int dimensionMap;
    public GameObject interieurEnclos;

    List<GameObject> moutons = new List<GameObject>();
    List<GameObject> loups = new List<GameObject>();
    GameObject berger;
    Vector3 mousePos;

    // Use this for initialization
    void Start () {
        Application.targetFrameRate = 16;
        QualitySettings.vSyncCount = 0;
        for (int i = 0; i < nbMoutons; i++) {
            moutons.Add((GameObject)Instantiate(Resources.Load("Mouton")));
            moutons[i].transform.Translate(new Vector3(Random.Range(-25,25),0, Random.Range(-25, 25)));
            moutons[i].GetComponent<Mover>().maxSpeed = 0.25f;
            moutons[i].GetComponent<Mover>().maxForce = 0.5f;
            moutons[i].GetComponent<Mover>().targetRadius = 5;
        }
        for (int i = 0; i < nbLoups; i++) {
            loups.Add((GameObject)Instantiate(Resources.Load("Loup")));
            loups[i].transform.Translate(new Vector3(Random.Range(0,50),0, Random.Range(0, 50)));
            loups[i].GetComponent<Mover>().maxSpeed = 0.5f;
            loups[i].GetComponent<Mover>().maxForce = 0.75f;
            loups[i].GetComponent<Mover>().targetRadius = 2;
        }
        berger = (GameObject)Instantiate(Resources.Load("Berger"));
        berger.GetComponent<Mover>().maxSpeed = 0.5f;
        berger.GetComponent<Mover>().maxForce = 0.75f;
        berger.GetComponent<Mover>().targetRadius = 2;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if(Input.GetMouseButton(0)) {
            click();
        }
        if(nbMoutons > 0)
            updateMoutons();
        updateLoups();
        updateBerger();
    }

    void updateMoutons(){
        flocking(moutons, 0.75f, 0.05f);
        for (int i = 0; i < nbMoutons; i++) {
            Mover mover = moutons[i].GetComponent<Mover>();
            if (mover.vel.sqrMagnitude>0.001) {
                moutons[i].transform.rotation = Quaternion.LookRotation(mover.vel);
            }
            foreach(GameObject loup in loups) {
                if((loup.transform.position - moutons[i].transform.position).sqrMagnitude < 100)
                    mover.applyForce(0.5f * mover.flee(loup.transform.position));
            }
            if ((berger.transform.position - moutons[i].transform.position).sqrMagnitude < 25)
                mover.applyForce(mover.arrive(berger.transform.position));
            mover.calculPhys();
            if (moutons[i].transform.position.x > dimensionMap || moutons[i].transform.position.z > dimensionMap) {
                GameObject moutonParti = moutons[i];
                moutons.Remove(moutons[i]);
                Destroy(moutonParti);
                nbMoutons--;
            }
            //if(moutons[i].)
        }
    }

    void updateLoups(){
        separation(loups, 0.5f);
        foreach (GameObject loup in loups) {
            if (nbMoutons > 0) {
                GameObject cible = moutons[0];
                float diffCible = (cible.transform.position - loup.transform.position).sqrMagnitude;
                float diffTemp;
                foreach (GameObject mouton in moutons) {
                    diffTemp = (mouton.transform.position - loup.transform.position).sqrMagnitude;
                    if (diffTemp < diffCible) {
                        cible = mouton;
                        diffCible = diffTemp;
                    }
                }
                if (diffCible < 1) {
                    moutons.Remove(cible);
                    Destroy(cible);
                    nbMoutons--;
                }
                Mover mover = loup.GetComponent<Mover>();
                mover.applyForce(mover.arrive(cible.transform.position));
                if ((berger.transform.position - loup.transform.position).sqrMagnitude < 100)
                    mover.applyForce(mover.flee(berger.transform.position));
                mover.calculPhys();
                if (mover.vel.sqrMagnitude > 0.001) {
                    loup.transform.rotation = Quaternion.LookRotation(mover.vel);
                } 
            }
        }
    }

    void updateBerger() {
        if (!mousePos.Equals(new Vector3(0, 0, 0)))
            berger.GetComponent<Mover>().applyForce(berger.GetComponent<Mover>().arrive(mousePos));
        berger.GetComponent<Mover>().calculPhys();
    }

    void click() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit) && hit.point != new Vector3(0,0,0)) {
            mousePos = hit.point;
            mousePos.y = 0;
        }
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
       // cube.transform.position = mousePos;
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
            steer = limit(steer * facteur, individu.GetComponent<Mover>().maxForce);
            individu.GetComponent<Mover>().applyForce(steer);
        }
    }

	void separation(List<GameObject> groupe, float facteur) {
		Vector3 somme = new Vector3(0,0,0);

		foreach (GameObject individu in groupe) {
			int count = 0;
			foreach (GameObject autreIndividu in groupe) {
				if(autreIndividu != individu) {
					Vector3 dist = individu.transform.position - autreIndividu.transform.position;
					if(dist.sqrMagnitude < this.distanceSep * this.distanceSep) {
						count++;
						somme += dist.normalized / distanceSep;
					}
				}

			}
			if (count > 0) {
				somme.Normalize();
				somme *= individu.GetComponent<Mover> ().maxSpeed;

				Vector3 steer = somme - individu.GetComponent<Mover> ().vel;
				steer = limit (steer*facteur, individu.GetComponent<Mover> ().maxForce);
				individu.GetComponent<Mover> ().applyForce (steer);
			}
		}
	
	}

    void flocking(List<GameObject> groupe, float separationFactor, float alignementFactor) {
        separation(groupe, separationFactor);
        alignement(groupe, alignementFactor);
    }

    Vector3 limit(Vector3 vec, float limit){
		if (vec.magnitude > limit) {
			return vec.normalized * limit;
		}
		else {
			return vec;
		}
			
	}
}