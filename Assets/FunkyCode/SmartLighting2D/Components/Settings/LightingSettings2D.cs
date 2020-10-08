using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class LightingSettings2D : MonoBehaviour {
    public LightingSettings.Profile setProfile;
    public LightingSettings.Profile profile;

    static LightingSettings2D instance;

    public bool initializeCopy = false;

    public static LightingSettings2D Get() {
        return(instance);
    }

    private void OnDisable() {
        if (profile != null) {
           if (Application.isPlaying && initializeCopy == true) {
                Lighting2D.RemoveProfile();
           }
        } 
    }

    void SetupProfile() {
        if (setProfile == null) {
            setProfile = Lighting2D.Profile;
        } 

        if (initializeCopy) {
            profile = Object.Instantiate(setProfile);
        } else {
            profile = setProfile;
        }
    }

    private void OnEnable() {
        instance = this;

        SetupProfile();

        Update();
    }

    void Update() {
        if (profile != null) {
            Lighting2D.UpdateByProfile(profile);
        }
    }
}

