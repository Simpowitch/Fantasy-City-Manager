using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayLightingTilemapCollider2D : MonoBehaviour {
    public enum MapType {UnityEngineTilemapRectangle};

    public LightingLayer dayLayer = LightingLayer.Layer1;

    public MapType tilemapType = MapType.UnityEngineTilemapRectangle;

    public bool onlyColliders = false;

    public static List<DayLightingTilemapCollider2D> list = new List<DayLightingTilemapCollider2D>();

    public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();
	}

	public void OnDisable() {
		list.Remove(this);
	}


}
