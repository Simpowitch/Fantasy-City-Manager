using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightingShape {

    public class SpriteCustomPhysicsShape : Base {
        
        private Sprite sprite;

        public CustomPhysicsShape customPhysicsShape = null;

        private SpriteRenderer spriteRenderer;

        override public void ResetLocal() {
            base.ResetLocal();
            
            customPhysicsShape = null;

			sprite = null;
        }

        public Sprite GetOriginalSprite() {
            if (sprite == null) {
                GetSpriteRenderer();

                if (spriteRenderer != null) {
                    sprite = spriteRenderer.sprite;
                }
            }
			return(sprite);
		}
        
		public SpriteRenderer GetSpriteRenderer() {
			if (transform == null) {
				return(spriteRenderer);
			}

			if (spriteRenderer == null) {
				spriteRenderer = transform.GetComponent<SpriteRenderer>();
			}

			return(spriteRenderer);
		}

        public CustomPhysicsShape GetPhysicsShape() {
			if (customPhysicsShape == null) {
                Sprite sprite = GetOriginalSprite();

                if (sprite != null) {
                    customPhysicsShape = CustomPhysicsShapeManager.RequesCustomShape(sprite);
                }
			}
			return(customPhysicsShape);
		}

        public float GetRadius() {
			if (meshesRadius < 0) {

				meshesRadius = 0;

				List<Polygon2D> polygons = GetPolygonsLocal();

				if (polygons.Count > 0) {
					foreach(Polygon2D poly in polygons) {
						foreach (Vector2D id in poly.pointsList) {
							meshesRadius = Mathf.Max(meshesRadius, Vector2.Distance(id.ToVector2(), Vector2.zero));
						}
					}
				}
			}
			return(meshesRadius);
		}

		public List<MeshObject> GetMeshes() {
			if (meshes == null) {
				List<Polygon2D> polygons = GetPolygonsLocal();

				if (polygons.Count > 0) {
					meshes = new List<MeshObject>();
					foreach(Polygon2D poly in polygons) {
						if (poly.pointsList.Count < 3) {
							continue;
						}

						Mesh mesh = PolygonTriangulator2D.Triangulate (poly, Vector2.zero, Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
						if (mesh) {
							meshes.Add( new MeshObject(mesh) );
						}
						
					}
				}
			}
			return(meshes);
		}

		public List<Polygon2D> GetPolygonsWorld() {
			if (polygons_world != null) {
				return(polygons_world);
			}

			Vector2 scale = new Vector2();

			List<Polygon2D> localPolygons = GetPolygonsLocal();

			if (polygons_world_cache != null) {				
				if (localPolygons.Count != polygons_world_cache.Count) {
					polygons_world_cache = null;


				} else {
					for(int i = 0; i < localPolygons.Count; i++) {
						if (localPolygons[i].pointsList.Count != polygons_world_cache[i].pointsList.Count) {
							polygons_world_cache = null;

							break;
						}
					}
				}
			}
		
			if (polygons_world_cache != null) {

				polygons_world = polygons_world_cache;

				Polygon2D poly;
				Vector2D point;

				SpriteRenderer spriteRenderer = GetSpriteRenderer();

				for(int i = 0; i < localPolygons.Count; i++) {
					poly = localPolygons[i];
					for(int p = 0; p < poly.pointsList.Count; p++) {
						point = poly.pointsList[p];
						
						polygons_world[i].pointsList[p].x = point.x;
						polygons_world[i].pointsList[p].y = point.y;
					}

					if (spriteRenderer != null) {
						scale.x = 1;
						scale.y = 1;

						if (spriteRenderer.flipX == true) {
							scale.x = -1;
						}

						if (spriteRenderer.flipY == true) {
							scale.y = -1;
						}
						
						if (spriteRenderer.flipX != false || spriteRenderer.flipY != false) {
							polygons_world[i].ToScaleItself(scale);
						}
					}

					polygons_world[i].ToWorldSpaceItself(transform);
				}
			} else {
			
				Polygon2D polygon;

				polygons_world = new List<Polygon2D>();

				SpriteRenderer spriteRenderer = GetSpriteRenderer();

				foreach(Polygon2D poly in localPolygons) {
					polygon = poly.Copy();

					if (spriteRenderer != null) {
						scale.x = 1;
						scale.y = 1;

						if (spriteRenderer.flipX == true) {
							scale.x = -1;
						}

						if (spriteRenderer.flipY == true) {
							scale.y = -1;
						}
						
						if (spriteRenderer.flipX != false || spriteRenderer.flipY != false) {
							polygon.ToScaleItself(scale);
						}
					}
					
					polygon.ToWorldSpaceItself(transform);

					polygons_world.Add(polygon);

					polygons_world_cache = polygons_world;
				}
			}

			return(polygons_world);
		}

		public List<Polygon2D> GetPolygonsLocal() {
			if (polygons_local != null) {
				return(polygons_local);
			}

			polygons_local = new List<Polygon2D>();

			#if UNITY_2017_4_OR_NEWER
			
				if (customPhysicsShape == null) {

					if (GetOriginalSprite() == null) {
						return(polygons_local);
					}

					customPhysicsShape = GetPhysicsShape();
				}

				polygons_local = customPhysicsShape.Get();

			#endif

			return(polygons_local);
		}
    }
}