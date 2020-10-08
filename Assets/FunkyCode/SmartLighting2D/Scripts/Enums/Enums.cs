using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightingLayer {Layer1, Layer2, Layer3, Layer4, Layer5, Layer6};
public enum LightingLayerType {ShadowAndMask, ShadowOnly, MaskOnly}
public enum LightingLayerSorting {None, DistanceToLight, YAxisDown, YAxisUp};
public enum LightingLayerEffect {AlwaysLit, AboveLit};

public enum NormalMapTextureType {
	Texture,
	Sprite
}

public enum NormalMapType {
	PixelToLight,
	ObjectToLight
}

public enum LightingEventState {
	OnCollision, 
	OnCollisionEnter, 
	OnCollisionExit, 
	None
}
