﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering.Night.WithoutAtlas {
        
    public class ParticleRenderer  {
        
        static public void Draw(Camera camera, Vector2 offset, float z, int nightLayer) {
			ParticleSystem.Particle particle;
			Vector2 size, pos;

            List<LightingParticleRenderer2D> particleRendererList = LightingParticleRenderer2D.GetList();

			for(int i = 0; i < particleRendererList.Count; i++) {
				LightingParticleRenderer2D id = particleRendererList[i];

				if ((int)id.nightLayer != nightLayer) {
					continue;
				}

				ParticleSystem particleSystem = id.GetParticleSystem();

				if (particleSystem == null) {
					continue;
				}

				ParticleSystemRenderer particleSystemRenderer = id.GetParticleSystemRenderer();

				if (particleSystemRenderer == null) {
					continue;
				}

				ParticleSystemSimulationSpace simulationSpace = particleSystem.main.simulationSpace;

				if (id.particleArray == null || id.particleArray.Length < particleSystem.main.maxParticles) {
					id.particleArray = new ParticleSystem.Particle[particleSystem.main.maxParticles];
				}
					
				int particlesAlive = particleSystem.GetParticles (id.particleArray);

				Texture texture = particleSystemRenderer.sharedMaterial.mainTexture;
				if (id.customParticle) {
					texture = id.customParticle;
				}

				Vector2 pOffset = offset;
				float rotation = id.transform.eulerAngles.z * Mathf.Deg2Rad;
				Color color = id.color;

				switch(simulationSpace) {
					case ParticleSystemSimulationSpace.Local:
						pOffset.x += id.transform.position.x;
						pOffset.y += id.transform.position.y;
					break;
				}

				Material material = Lighting2D.materials.GetAdditive();
				material.SetColor ("_TintColor", color);
				material.mainTexture = texture;
				
				material.SetPass (0); 

				GL.Begin (GL.QUADS);

				for (int p = 0; p < particlesAlive; p++) {
					particle = id.particleArray [p];

					if (particle.remainingLifetime < 0.1f ) {
						continue;
					}

					size.x = (particle.GetCurrentSize(particleSystem) * id.scale) / 2;
					size.y = size.x;

					switch(simulationSpace) {
						case ParticleSystemSimulationSpace.Local:
							pos = particle.position;

							float angle = Mathf.Atan2(pos.y, pos.x) + rotation;
							float distance = pos.magnitude;

							pos.x = Mathf.Cos(angle) * distance;
							pos.y = Mathf.Sin(angle) * distance;

							pos.x *= id.transform.localScale.x;
							pos.y *= id.transform.localScale.y;

							break;

						case ParticleSystemSimulationSpace.World:
							pos = particle.position;
							break;

						default:
							pos = Vector2.zero;

							break;

					}

					pos.x += pOffset.x;
					pos.y += pOffset.y;

					//if (InCamera(camera, pos, size.x) == false) {
					//continue;
					//}

					Rendering.Night.WithoutAtlas.Particle.DrawPass(material, pos, size, particle.rotation, z);
				}

				GL.End (); 
			}
        }
    }
}