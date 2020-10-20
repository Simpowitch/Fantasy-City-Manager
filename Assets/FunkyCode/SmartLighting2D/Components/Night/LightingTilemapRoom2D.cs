using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

#if UNITY_2017_4_OR_NEWER
 //ITilemap.GetSprite(Vector3)
    using UnityEngine.Tilemaps;

    [ExecuteInEditMode]
    public class LightingTilemapRoom2D : MonoBehaviour {
        public LightingLayer nightLayer = LightingLayer.Layer1;
        
        public enum MapType {UnityEngineTilemapRectangle, SuperTilemapEditor};
        public enum MaskType {Sprite}
        public enum ShaderType {ColorMask, MultiplyTexture};

        public MapType mapType = MapType.UnityEngineTilemapRectangle;
        public ShaderType shaderType = ShaderType.ColorMask;

        public MaskType maskType = MaskType.Sprite;

        public Color color = Color.black;

        public SuperTilemapEditorSupport.TilemapRoom2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapRoom2D();
        public LightingTilemapCollider.Rectangle rectangle = new LightingTilemapCollider.Rectangle();

        public static List<LightingTilemapRoom2D> list = new List<LightingTilemapRoom2D>();

        static public List<LightingTilemapRoom2D> GetList() {
            return(list);
        }

        public void OnEnable() {
            list.Add(this);

            LightingManager2D.Get();

			rectangle.SetGameObject(gameObject);

            Initialize();
        }

        public void OnDisable() {
            list.Remove(this);
        }

        public void Initialize() {
            switch(mapType) {
                case MapType.UnityEngineTilemapRectangle:
                    rectangle.Initialize();
                break;	

                case MapType.SuperTilemapEditor:
                    superTilemapEditor.lightingTilemapRoom2D = this;
                    superTilemapEditor.Initialize();

                break;
            }
        }

        public TilemapProperties GetTilemapProperties() {
			switch(mapType) {
				case MapType.UnityEngineTilemapRectangle:
					return(rectangle.Properties);
			}

			return(null);
		}
    }

#endif

/*

   public List<Polygon2D> edgeColliders = new List<Polygon2D>();
        public List<Polygon2D> polygonColliders = new List<Polygon2D>();

        public Mesh[] polygonCollidersMeshes = null;
        public Mesh[] edgeCollidersMeshes = null;

        //???????????????????




        public Mesh GetPolygonMesh(Polygon2D poly) {
            if (poly.pointsList.Count < 3) {
                return(null);
            }
            int id = polygonColliders.IndexOf(poly);

            if (polygonCollidersMeshes[id] == null) {
            Mesh mesh = poly.CreateMesh(Vector2.zero, Vector2.zero);

            polygonCollidersMeshes[id] = mesh;
            }

            return(polygonCollidersMeshes[id]);
        }
        
        public Mesh GetEdgeMesh(Polygon2D poly) {
            if (poly.pointsList.Count < 3) {
                return(null);
            }
            int id = edgeColliders.IndexOf(poly);

            if (edgeCollidersMeshes[id] == null) {
            Mesh mesh = poly.CreateMesh(Vector2.zero, Vector2.zero);

            edgeCollidersMeshes[id] = mesh;
            }

            return(edgeCollidersMeshes[id]);
        }


        */