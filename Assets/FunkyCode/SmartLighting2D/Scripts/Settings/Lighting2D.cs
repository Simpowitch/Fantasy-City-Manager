using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

public class Lighting2D {
	public const int VERSION = 116;
	public const string VERSION_STRING = "1.1.6";

	static public Lighting2DMaterials materials = new Lighting2DMaterials();

	// Buffer Settings
	static public BufferPreset[] bufferPresets {
		get => Profile.bufferPresets.list;
	}

	// Common Settings
	static public LightingSettings.QualitySettings commonSettings {
		get => Profile.qualitySettings;
	}
	// Day Settings
	static public DayLightingSettings dayLightingSettings {
		get => Profile.dayLightingSettings;
	}

	// Fog of War
	static public FogOfWar fogOfWar {
		get => Profile.fogOfWar;
	}

	// Disable
	static public bool disable = false;

	// Light Source Buffers
	static public LightingSourceSettings lightingBufferSettings {
		get => ProjectSettings.lightingBufferSettings;
	}
	
	static public RenderingMode renderingMode {
		get {
			if (projectSettings == null) {
				return(RenderingMode.OnPostRender); // ?
			}
			return(ProjectSettings.renderingMode);
		}
	}

	// Atlas
	static public AtlasSettings atlasSettings {
		get => ProjectSettings.atlasSettings;
	}
	
	// Utilities
	static public PolygonTriangulator2D.Triangulation triangulation {
		get {
			return(ProjectSettings.triangulation);
		}
	}

	static public CoreAxis coreAxis {
		get {
			return(ProjectSettings.coreAxis);
		}
	}

	// Set & Get API
	static public Color darknessColor {
		get { return bufferPresets[0].darknessColor; }
		set { bufferPresets[0].darknessColor = value; }
	}

	static public float lightingResulution {
		get { return bufferPresets[0].lightingResolution; }
		set { bufferPresets[0].lightingResolution = value; }
	}

	// Methods
	static public void UpdateByProfile(Profile setProfile) {
		if (setProfile == null) {
			Debug.Log("Light 2D: Update Profile is Missing");
			return;
		}

		// Disable
		disable = setProfile.disable;

		// Set profile also
		profile = setProfile;
	}

	static public void RemoveProfile() {
		profile = null;
	}

	// Profile
	static private Profile profile = null;
	static public Profile Profile
	{
		get
		{
			if (profile != null) {
				return(profile);
			}

			if (ProjectSettings != null) {
				profile = ProjectSettings.Profile;
			}

			if (profile == null) {
				profile = Resources.Load("Profiles/Default Profile") as Profile;

				if (profile == null) {
					Debug.LogError("Light 2D: Default Profile not found");
				}
			}

			return(profile);
		}
	}

	static private ProjectSettings projectSettings;
	static public ProjectSettings ProjectSettings
	{
		get
		{
			if (projectSettings != null) {
				return(projectSettings);
			}

			projectSettings = Resources.Load("Settings/Project Settings") as ProjectSettings;

			if (projectSettings == null) {
				Debug.LogError("Light 2D: Project Settings not found");
				return(null);
			}
		
			return(projectSettings);
		}
	}
}