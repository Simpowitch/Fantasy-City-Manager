using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingParticleRenderer2D : MonoBehaviour {
    public enum Type {Particle};

	public LightingLayer nightLayer = LightingLayer.Layer1;

    public Color color = Color.white;

    public float scale = 1;

    public Texture customParticle;

    private ParticleSystem particleSystem2D;
    private ParticleSystemRenderer particleSystemRenderer2D;
    public ParticleSystem.Particle[] particleArray;

    public static List<LightingParticleRenderer2D> list = new List<LightingParticleRenderer2D>();

    static public List<LightingParticleRenderer2D> GetList() {
		return(list);
	}

    public void OnEnable() {
		list.Add(this);

        LightingManager2D.Get();
	}

	public void OnDisable() {
		list.Remove(this);
	}

    public ParticleSystem GetParticleSystem() {
        if (particleSystem2D == null) {
            particleSystem2D = GetComponent<ParticleSystem>();
        }

        return(particleSystem2D);
    }

    public ParticleSystemRenderer GetParticleSystemRenderer() {
        if (particleSystemRenderer2D == null) {
            particleSystemRenderer2D = GetComponent<ParticleSystemRenderer>();
        }

        return(particleSystemRenderer2D);
    }
}
