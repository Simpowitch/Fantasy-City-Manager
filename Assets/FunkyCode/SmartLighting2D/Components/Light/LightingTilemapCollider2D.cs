using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;
using LightingSettings;
using UnityEngine.Tilemaps;
using System;

#if UNITY_2017_4_OR_NEWER

	[ExecuteInEditMode]
	public class LightingTilemapCollider2D : MonoBehaviour {
		public enum MapType {UnityEngineTilemapRectangle, UnityEngineTilemapIsometric, UnityEngineTilemapHexagon, SuperTilemapEditor};
		public enum ShadowTileType {AllTiles, ColliderOnly};
		public MapType mapType = MapType.UnityEngineTilemapRectangle;

		public LightingLayer lightingCollisionLayer = LightingLayer.Layer1;
		public LightingLayer lightingMaskLayer = LightingLayer.Layer1;

		public ShadowTileType colliderTileType = ShadowTileType.AllTiles;

		public NormalMapMode bumpMapMode = new NormalMapMode();

		public LightingTilemapCollider.Rectangle rectangle = new LightingTilemapCollider.Rectangle();
		public LightingTilemapCollider.Isometric isometric = new LightingTilemapCollider.Isometric();
		public LightingTilemapCollider.Hexagon hexagon = new LightingTilemapCollider.Hexagon();

		public LightingTilemapTransform lightingTransform = new LightingTilemapTransform();

		public SuperTilemapEditorSupport.TilemapCollider2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapCollider2D();

		public static List<LightingTilemapCollider2D> list = new List<LightingTilemapCollider2D>();

		public void OnEnable() {
			list.Add(this);

			LightingManager2D.Get();

			rectangle.SetGameObject(gameObject);
			isometric.SetGameObject(gameObject);
			hexagon.SetGameObject(gameObject);

			Initialize();

			LightingSource2D.ForceUpdateAll();
		}

		public void OnDisable() {
			list.Remove(this);

			LightingSource2D.ForceUpdateAll();
		}

		static public List<LightingTilemapCollider2D> GetList() {
			return(list);
		}

		public void Update() {
			lightingTransform.Update(this);

			if (lightingTransform.UpdateNeeded) {
				rectangle.UpdateProperties();
				isometric.UpdateProperties();
				hexagon.UpdateProperties();

				LightingSource2D.ForceUpdateAll();
			}
		}

		public void Initialize() {
			TilemapEvents.Initialize();

			switch(mapType) {
				case MapType.UnityEngineTilemapRectangle:
					rectangle.Initialize();

				break;

				case MapType.UnityEngineTilemapIsometric:
					isometric.Initialize();

				break;

				case MapType.UnityEngineTilemapHexagon:
					hexagon.Initialize();

				break;

				case MapType.SuperTilemapEditor:
					superTilemapEditor.Initialize(this);

				break;
			}
		}

		public bool IsCustomPhysicsShape() {
			switch(mapType) {
				case MapType.UnityEngineTilemapRectangle:
					if (rectangle.maskType == LightingTilemapCollider.Rectangle.MaskType.SpriteCustomPhysicsShape) {
						return(true);
					}
					if (rectangle.colliderType == LightingTilemapCollider.Rectangle.ColliderType.SpriteCustomPhysicsShape) {
						return(true);
					}
					break;
				case MapType.UnityEngineTilemapIsometric:
					if (isometric.maskType == LightingTilemapCollider.Isometric.MaskType.SpriteCustomPhysicsShape) {
						return(true);
					}
					if (isometric.colliderType == LightingTilemapCollider.Isometric.ColliderType.SpriteCustomPhysicsShape) {
						return(true);
					}
					break;
			}
			return(false);
		}

		public TilemapProperties GetTilemapProperties() {
			switch(mapType) {
				case MapType.UnityEngineTilemapRectangle:
					return(rectangle.Properties);
				
				case MapType.UnityEngineTilemapIsometric:
					return(isometric.Properties);

				case MapType.UnityEngineTilemapHexagon:
					return(hexagon.Properties);

				case MapType.SuperTilemapEditor:
					return(superTilemapEditor.properties);
			}

			return(null);
		}
	}

#endif