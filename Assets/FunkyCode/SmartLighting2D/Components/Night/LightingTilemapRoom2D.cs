using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

#if UNITY_2017_4_OR_NEWER

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

        public TilemapProperties properties = new TilemapProperties();

        public SuperTilemapEditorSupport.TilemapRoom2D superTilemapEditor = new SuperTilemapEditorSupport.TilemapRoom2D();

        public List<Polygon2D> edgeColliders = new List<Polygon2D>();
        public List<Polygon2D> polygonColliders = new List<Polygon2D>();

        public Mesh[] polygonCollidersMeshes = null;
        public Mesh[] edgeCollidersMeshes = null;

        public LightingTile[,] map;

        private Tilemap tilemap2D;

        public static List<LightingTilemapRoom2D> list = new List<LightingTilemapRoom2D>();

        static public List<LightingTilemapRoom2D> GetList() {
            return(list);
        }

        public void OnEnable() {
            list.Add(this);

            LightingManager2D.Get();

            Initialize();
        }

        public void OnDisable() {
            list.Remove(this);
        }

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

        public void Initialize() {
            switch(mapType) {
                case MapType.UnityEngineTilemapRectangle:
                    InitializeUnityDefault();
                break;	

                case MapType.SuperTilemapEditor:
                    InitializeSuperTilemapEditor();

                break;
            }
        }

        void InitializeUnityDefault() {
            tilemap2D = GetComponent<Tilemap>();

            if (tilemap2D == null) {
                return;
            }

            Grid grid = tilemap2D.layoutGrid;

            if (grid == null) {
                Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid");
            } else {
                properties.cellSize = grid.cellSize;
            }

            properties.cellAnchor = tilemap2D.tileAnchor;

            int minPos = Mathf.Min(tilemap2D.cellBounds.xMin, tilemap2D.cellBounds.yMin);
            int maxPos = Mathf.Max(tilemap2D.cellBounds.size.x, tilemap2D.cellBounds.size.y);

            properties.area = new BoundsInt(minPos, minPos, 0, maxPos + Mathf.Abs(minPos), maxPos + Mathf.Abs(minPos), 1);

            TileBase[] tileArray = tilemap2D.GetTilesBlock(properties.area);

            map = new LightingTile[properties.area.size.x + 1, properties.area.size.y + 1];

            for (int index = 0; index < tileArray.Length; index++) {
                TileBase tile = tileArray[index];
                if (tile == null) {
                    continue;
                }

                TileData tileData = new TileData();

                ITilemap tilemap = (ITilemap) FormatterServices.GetUninitializedObject(typeof(ITilemap));
                typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(tilemap, tilemap2D);
                tile.GetTileData(new Vector3Int(0, 0, 0), tilemap, ref tileData);

                LightingTile lightingTile = new LightingTile();
                lightingTile.SetOriginalSprite(tileData.sprite);
                lightingTile.GetShapePolygons();
        
                
                map[(index % properties.area.size.x), (index / properties.area.size.x)] = lightingTile;
                //map[(index % area.size.x), (index / area.size.y)] = true;
            }
        }

        void InitializeSuperTilemapEditor() {
            superTilemapEditor.lightingTilemapRoom2D = this;
            superTilemapEditor.Initialize();
        }

        public class TilemapProperties {
            public Vector2 cellSize = new Vector2(1, 1);
            public Vector2 cellAnchor = new Vector2(0.5f, 0.5f);
            public Vector2 cellGap = new Vector2(1, 1);
            public Vector2 colliderOffset = new Vector2(0, 0);
            public BoundsInt area;

            public Vector2Int arraySize = new Vector2Int(); // Implement or Remove
        }
    }

#endif