using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

// Project Settings
namespace LightingSettings {
    [CreateAssetMenu(fileName = "Data", menuName = "Lighting2D/Project Settings", order = 2)]

    public class ProjectSettings : ScriptableObject {

		//public RenderingPipeline renderingPipeline = RenderingPipeline.Standard;
		
		public RenderingMode renderingMode = RenderingMode.OnRender;

		public Layers layers;

		public AtlasSettings atlasSettings;

		public EditorView sceneView;

		public LightingSourceSettings lightingBufferSettings;

		public CoreAxis coreAxis = CoreAxis.XY;

		public PolygonTriangulator2D.Triangulation triangulation;

		public bool disable;

		public Profile profile;
        public Profile Profile {
			get {
				if (profile != null) {
					return(profile);
				}
		
				profile = Resources.Load("Profiles/Default Profile") as Profile;

				if (profile == null) {
					Debug.LogError("Light 2D Project Settings: Default Profile not found");
				}
			
				return(profile);
			}

			set {
				profile = value;
			}
		}

		public ProjectSettings() {
			layers = new Layers();

			atlasSettings = new AtlasSettings();

			lightingBufferSettings = new LightingSourceSettings();

			coreAxis = CoreAxis.XY;

			triangulation = PolygonTriangulator2D.Triangulation.Advanced;

			disable = false;
		}
    }
}
