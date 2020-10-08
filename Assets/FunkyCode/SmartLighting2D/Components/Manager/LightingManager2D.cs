using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightingSettings;

#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode] 
public class LightingManager2D : LightingMonoBehaviour {
	private static LightingManager2D instance;

	public CameraSettings[] cameraSettings = new CameraSettings[1];

	public bool debug = false;

	public int version;

	// Sets Lighting Main Profile Settings for Lighting2D at the start of the scene
	private static bool initialized = false; 

	public Camera GetCamera(int id) {
		if (cameraSettings.Length <= id) {
			return(null);
		}

		return(cameraSettings[id].GetCamera());
	}

	public int GetCameraBufferID(int id) {
		if (cameraSettings.Length <= id) {
			return(0);
		}

		return(cameraSettings[id].bufferID);
	}

	public static void ForceUpdate() {
		LightingManager2D manager = Get();

		if (manager != null) {
			// manager.gameObject.SetActive(false);
			// manager.gameObject.SetActive(true);
		}
	}
	
	static public LightingManager2D Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(LightingManager2D manager in Object.FindObjectsOfType(typeof(LightingManager2D))) {
			instance = manager;
			return(instance);
		}

		// Create New Light Manager
		GameObject gameObject = new GameObject();
		gameObject.name = "Lighting Manager 2D";

		instance = gameObject.AddComponent<LightingManager2D>();
		instance.Initialize();

		return(instance);
	}

	public void Initialize () {
		instance = this;

		transform.position = Vector3.zero;

		version = Lighting2D.VERSION;

		if (cameraSettings == null) {
			cameraSettings = new CameraSettings[1];
			cameraSettings[0] = new CameraSettings();
		}

		// Reset Materials
		// Lighting2D.materials = new Lighting2DMaterials();
	}

	public void Awake() {
		LightingManager2D.initialized = false;
		SetupProfile();

		if (instance != null && instance != this) {
			instance = this;
			Debug.LogWarning("Smart Lighting2D: Lighting Manager duplicate was found, old instance destroyed.", gameObject);

			foreach(LightingManager2D manager in Object.FindObjectsOfType(typeof(LightingManager2D))) {
				if (manager != instance) {
					Destroy(manager.gameObject); // Fix Destroy Self
				}
			}
		}
	}

	private void Update() {
		ForceUpdate(); // For Late Update Method?

		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L)) {
			debug = !debug;
		}
	}

	void LateUpdate() {
	
		LightingCamera camera = LightingCamera.Get();
		
		if (Lighting2D.Profile.qualitySettings.updateMethod == LightingSettings.QualitySettings.UpdateMethod.LateUpadte) {
			UpdateLoop();
			
			camera.lightingCamera.enabled = false;
		} else {
			camera.lightingCamera.enabled = true;
		}
	}

	public void SetupProfile() {
		if (LightingManager2D.initialized == false) {
			LightingSettings.Profile profile = Lighting2D.Profile;
			Lighting2D.UpdateByProfile(profile);
			LightingManager2D.initialized = true;

			AtlasSystem.Manager.Initialize();
			Lighting2D.materials.Reset();
		}
	}

	public void UpdateLoop() {
		SetupProfile();

		UpdateMaterials();

		UpdateBuffers();

		UpdateMainBuffers();

		AtlasSystem.Manager.Update();
		// Colliders should be updated before the light sources

		foreach(DayLightingCollider2D collider in DayLightingCollider2D.GetList()) {
			collider.UpdateLoop();
		}
		
		foreach(LightingCollider2D collider in LightingCollider2D.GetList()) {
			collider.UpdateLoop();
		}

		foreach(LightingSource2D source in LightingSource2D.GetList()) {
			source.UpdateLoop();
		}

		//LightingCamera.instance.bufferCamera.Render();

		foreach(LightingBuffer2D buffer in LightingBuffer2D.GetList()) {
			buffer.Render();
		}

		foreach(FogOfWarBuffer2D buffer in FogOfWarBuffer2D.list) {
			buffer.Render();
		}

		foreach(LightingMainBuffer2D buffer in LightingMainBuffer2D.list) {
			buffer.Render();
		}
	}

	void UpdateMainBuffers() {
		foreach(LightingMainBuffer2D buffer in LightingMainBuffer2D.list) {

			if (Lighting2D.disable) {
				buffer.updateNeeded = false;	
				return;
			}

			CameraSettings cameraSettings = buffer.cameraSettings;
			bool render = cameraSettings.renderMode != CameraSettings.RenderMode.Disabled;

			if (render && cameraSettings.GetCamera() != null) {
				buffer.updateNeeded = true;
			
			} else {
				buffer.updateNeeded = false;
			}
		}
	}

	public void UpdateBuffers() {
		for(int i = 0; i < cameraSettings.Length; i++) {
			LightingMainBuffer2D buffer = LightingMainBuffer2D.Get(cameraSettings[i]);
			if (buffer != null) {
				buffer.cameraSettings.renderMode = cameraSettings[i].renderMode;
			}

			if (Lighting2D.fogOfWar.enabled && cameraSettings[i].bufferID == Lighting2D.fogOfWar.bufferID) {
				if (cameraSettings[i].cameraType != CameraSettings.CameraType.SceneView) {
					FogOfWarBuffer2D.Get(cameraSettings[i]);
				}
			}
		}
	}
	
	public void UpdateMaterials() {
		if (Lighting2D.materials.Initialize(Lighting2D.commonSettings.HDR)) {

			foreach(LightingMainBuffer2D buffer in new List<LightingMainBuffer2D>(LightingMainBuffer2D.list)) {
				buffer.DestroySelf();
			}

			foreach(LightingBuffer2D buffer in new List<LightingBuffer2D>(LightingBuffer2D.list)) {
				buffer.DestroySelf();
			}

			foreach(LightingSource2D buffer in LightingSource2D.GetList()) {
				buffer.ForceUpdate();
			}
		}
	}

	public bool IsSceneView()
	{
		for (int i = 0; i < cameraSettings.Length; i++)
		{
			CameraSettings cameraSetting = cameraSettings[i];

			if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView)
			{
				if (cameraSetting.renderMode == CameraSettings.RenderMode.Draw)
				{
					return (true);
				}
			}
		}

		return (false);
	}

	void OnGUI() {
		if (debug) {
			LightingDebug.OnGUI();
		}
	}

	#if UNITY_EDITOR
		private void OnEnable() {
			#if UNITY_2019_1_OR_NEWER
				SceneView.beforeSceneGui += OnSceneView;
			#else
				SceneView.onSceneGUIDelegate += OnSceneView;
			#endif	
		}

		private void OnDisable() {
			#if UNITY_2019_1_OR_NEWER
				SceneView.beforeSceneGui -= OnSceneView;
			#else
				SceneView.onSceneGUIDelegate -= OnSceneView;
			#endif	
		}

	static public void OnSceneView(SceneView sceneView)
	{
		LightingManager2D manager = LightingManager2D.Get();

		if (manager.IsSceneView())
		{
			ForceUpdate();

			LightingManager2D.Get().UpdateLoop();

			LightingCamera lightingCamera = LightingCamera.Get();

			if (lightingCamera != null)
			{
				//lightingCamera.gameObject.SetActive(false);
				//lightingCamera.gameObject.SetActive(true);
			}
		}

	}


	//static public void OnSceneView(SceneView sceneView) {
	//		ForceUpdate();

	//		LightingManager2D.Get().UpdateLoop();

	//		LightingCamera lightingCamera = LightingCamera.Get();

	//		if (lightingCamera != null) {
	//			//lightingCamera.gameObject.SetActive(false);
	//			//lightingCamera.gameObject.SetActive(true);
	//		}
	//	}
	#endif
}