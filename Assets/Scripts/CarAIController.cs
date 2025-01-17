﻿using UnityEngine;

public class CarAIController : MonoBehaviour {
    public Transform Target;               // 'target' the target object to aim for.
    public float Speed = 100;
    public float UpdateRate = 0.5f; // interval in seconds to update direction
    public float MaxGroundDistance = 1f; // only updates direction if wihtin this distance from ground
    public LayerMask GroundLayers; // layers that count as ground
    public float GroundedDrag = 7; // angular drag while grounded

    float AirDrag;
    bool Grounded;
    ExplodeOnImpact ImpactExplosion;
    float VelocityY = -1;
    Vector3 Direction;
    Vector3 Velocity;
    Rigidbody Body;

    void Awake() {
        ImpactExplosion = GetComponent<ExplodeOnImpact>();
        Body = GetComponent<Rigidbody>();
        Body.centerOfMass = Vector3.down;
        AirDrag = Body.angularDrag;
    }

    void Start() {
        GameManager.CopCount++;
        Target = GameManager.Player.transform;

        // call the more expensive functions periodically
        InvokeRepeating("UpdateDirection", Random.Range(0, UpdateRate), UpdateRate);
    }

    void UpdateDirection() {
        // update direction and drag, depending on grounded state
        Grounded = Physics.Raycast(transform.position, -transform.up, MaxGroundDistance);
        if (Grounded) Body.angularDrag = GroundedDrag;
        else {
            Body.angularDrag = AirDrag;
            return;
        }

        transform.LookAt(Target);
        Velocity = transform.forward * Speed;
        // adjust velocity to decrease Inidial D drifting
        Velocity -= transform.right * transform.InverseTransformDirection(Body.velocity).x;
        Velocity.y = VelocityY;
    }

    void FixedUpdate() {
        if (!Grounded) return;
        Body.AddForce(Velocity);
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f) {
            return 1f;
        } else {
            return -1f;
        }
    }

    void OnDestroy() {
        GameManager.CopCount--;
        GameManager.KilledCops++;
    }
}
