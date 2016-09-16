﻿using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
    public float Radius = 50;
    public float Power = 2000;
    public float VerticalModifier = 5;
    public Light Glow;

    Rigidbody CarBody;
    ParticleEmitter Particles;

    void Start () {
        CarBody = GameManager.Player.GetComponent<Rigidbody>();
        Particles = GetComponent<ParticleEmitter>();
        Explode();
    }

    public void Explode() {
        // shake the camera
        GameManager.GameCamera.GetComponent<CameraShake>().Shake();

        // apply explosion force to surrounding objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && rb != CarBody)
                rb.AddExplosionForce(Power, transform.position, Radius, VerticalModifier);
            ExplodeOnImpact eoi = hit.GetComponent<ExplodeOnImpact>();
            if (eoi != null)
                eoi.Enabled = true;
        }
    }

    public void Update() {
        Glow.intensity = (Particles.particleCount - 1000)*0.004f;
    }
}
