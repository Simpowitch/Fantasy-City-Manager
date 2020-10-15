using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_2017_4_OR_NEWER

	[System.Serializable]
	public class LightingTile {
		public Vector3Int position;
		
		public Tile.ColliderType colliderType;

		public CustomPhysicsShape customPhysicsShape = null;

		private Sprite originalSprite;
		private Sprite atlasSprite;

		private List<Polygon2D> shapePolygons = null;
		private MeshObject shapeMesh = null;

		public List<Polygon2D> world_polygon = null;
		public List<List<Pair2D>> world_polygonPairs = null;

		public static MeshObject staticTileMesh = null;

		public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

		public void SetOriginalSprite(Sprite sprite) {
			originalSprite = sprite;
		}

		public Sprite GetOriginalSprite() {
			return(originalSprite);
		}

		public Sprite GetAtlasSprite() {
			return(atlasSprite);
		}

		public void SetAtlasSprite(Sprite sprite) {
			atlasSprite = sprite;
		}

		public bool InRange(Vector2 pos, float sourceSize) {
			return(Vector2.Distance(pos, Vector2.zero) > 2 + sourceSize);
		}

		public List<Polygon2D> GetPolygons(LightingTilemapCollider2D tilemap) {
			if (world_polygon == null) {

				if (tilemap.IsCustomPhysicsShape()) {
				
					if (GetShapePolygons().Count < 1) {
						return(null);
					}
					
					world_polygon = GetShapePolygons(); //poly.ToScaleItself(defaultSize); // scale?
					
				} else {
					world_polygon = new List<Polygon2D>();
					Polygon2D p;

					switch(tilemap.mapType) {
						case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
						case LightingTilemapCollider2D.MapType.SuperTilemapEditor:

							p = Polygon2D.CreateRect(Vector2.one * 0.5f);
							p.Normalize();

							world_polygon.Add(p);
						break;

						case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:

							p = Polygon2D.CreateIsometric(new Vector2(0.5f, 0.25f));
							p.Normalize();

							world_polygon.Add(p);
						break;

						case LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon:

							p = Polygon2D.CreateHexagon(new Vector2(0.5f, 0.25f));
							p.Normalize();

							world_polygon.Add(p);
						break;

					}
				}
			}
			return(world_polygon);
		}

		public List<Polygon2D> GetShapePolygons() {
			if (shapePolygons == null) {
				shapePolygons = new List<Polygon2D>();

				if (originalSprite == null) {
					return(shapePolygons);
				}

				#if UNITY_2017_4_OR_NEWER
					if (customPhysicsShape == null) {
						customPhysicsShape = CustomPhysicsShapeManager.RequesCustomShape(originalSprite);
					}

					if (customPhysicsShape != null) {
						shapePolygons = customPhysicsShape.Get();
					}
				#endif
			}
			return(shapePolygons);
		}

		public MeshObject GetTileDynamicMesh() {
			if (shapeMesh == null) {
				if (shapePolygons != null && shapePolygons.Count > 0) {
					shapeMesh = customPhysicsShape.GetMesh();
				}
			}
			return(shapeMesh);
		}

		public static MeshObject GetStaticTileMesh(LightingTilemapCollider2D tilemap) {
			if (staticTileMesh == null) {
				// Can be optimized?
				Mesh mesh = new Mesh();

				float x = 0.5f + 0.01f;
				float y = 0.5f + 0.01f;

				switch(tilemap.mapType) {
					case LightingTilemapCollider2D.MapType.UnityEngineTilemapRectangle:
						mesh.vertices = new Vector3[]{new Vector2(-x, -y), new Vector2(x, -y), new Vector2(x, y), new Vector2(-x, y)};
						mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
						mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
				
					break;
						
					case LightingTilemapCollider2D.MapType.SuperTilemapEditor:
						mesh.vertices = new Vector3[]{new Vector2(-x, -y), new Vector2(x, -y), new Vector2(x, y), new Vector2(-x, y)};
						mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
						mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
				
					break;

					case LightingTilemapCollider2D.MapType.UnityEngineTilemapIsometric:
						mesh.vertices = new Vector3[]{new Vector2(0, y), new Vector2(x, y / 2), new Vector2(0, 0), new Vector2(-x, y / 2)};
						mesh.triangles = new int[]{0, 1, 2, 2, 3, 0};
						mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
				
					break;

					case LightingTilemapCollider2D.MapType.UnityEngineTilemapHexagon:
						float yOffset = - 0.25f;
						mesh.vertices = new Vector3[]{new Vector2(0, y * 1.5f + yOffset), new Vector2(x, y + yOffset), new Vector2(0, -y * 0.5f + yOffset), new Vector2(-x, y + yOffset), new Vector2(-x, 0 + yOffset), new Vector2(x, 0 + yOffset)};
						mesh.triangles = new int[]{0, 1, 5, 4, 3, 0, 0, 5, 2, 0, 2, 4};
						mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(0, 1),  new Vector2(0, 1) };
				
					break;
				}
				
				staticTileMesh = new MeshObject(mesh);	
			}
			return(staticTileMesh);
		}
	}

#endif