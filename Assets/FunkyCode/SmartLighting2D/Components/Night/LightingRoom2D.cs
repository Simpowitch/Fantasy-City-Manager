using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightingRoom2D : MonoBehaviour {
	public enum RoomType {Collider, Sprite};

	public LightingLayer nightLayer = LightingLayer.Layer1;
	public Color color = Color.black;

	public LightingRoomShape shape = new LightingRoomShape();

	public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

	public static List<LightingRoom2D> list = new List<LightingRoom2D>();

	static public List<LightingRoom2D> GetList() {
		return(list);
	}

	public void OnEnable() {
		list.Add(this);

		LightingManager2D.Get();

		shape.SetTransform(transform);
	}

	public void OnDisable() {
		list.Remove(this);
	}
	
	public void Awake() {
		Initialize();
	}

	public void Initialize() {
		shape.ResetLocal();
	}
}