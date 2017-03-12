using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public Vector3 vel;
    public float maxForce;
    public float maxSpeed;
    public float targetRadius;

    Vector3 acc;

    // Use this for initialization
    void Start() {
        vel = new Vector3(0, 0, 0);
        acc = new Vector3(0, 0, 0);
    }

    public void applyForce(Vector3 force) {
  
        //if (Random.Range(0,100) < 1)
           // if (force.sqrMagnitude > 0.1)
                acc += force;
    }

    public Vector3 arrive(Vector3 target) {
        Vector3 dist = target - transform.position;
        dist.y = 0;
        Vector3 steer;
        if (dist.sqrMagnitude > targetRadius) {
            dist.Normalize();
            dist *= maxSpeed;

            steer = dist - vel;
            if (steer.magnitude > maxForce) {
                steer.Normalize();
                steer *= maxForce;
            }

        } else {
            float mult = Map(dist.magnitude, 0, targetRadius, 0, maxSpeed);
            dist.Normalize();
            //	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //	cube.transform.position = mousePos;
            steer = dist * mult - vel;

        }
        //Debug.Log(dist+"/"+steer+"/"+);
        return(steer);
    }

    public Vector3 seek(Vector3 target) {
        Vector3 dist = target - transform.position;
        dist.y = 0;
        Vector3 steer;
        dist.Normalize();
        dist *= maxSpeed;

        steer = dist - vel;
        if (steer.magnitude > maxForce) {
            steer.Normalize();
            steer *= maxForce;
        }

        return(steer);
    }

    public Vector3 flee(Vector3 target) {
        Vector3 dist = target - transform.position;
        dist.y = 0;
        Vector3 steer;
        dist.Normalize();
        dist *= -maxSpeed;

        steer = dist - vel;
        if (steer.magnitude > maxForce) {
            steer.Normalize();
            steer *= maxForce;
        }

       return(steer);
    }

    public void calculPhys() {
        transform.position += vel/2;
        vel += acc;
        if (vel.sqrMagnitude > maxSpeed * maxSpeed) {
            vel.Normalize();
            vel *= maxSpeed;
        }
        transform.position += vel / 2;
        acc *= 0;
    }

    float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}

