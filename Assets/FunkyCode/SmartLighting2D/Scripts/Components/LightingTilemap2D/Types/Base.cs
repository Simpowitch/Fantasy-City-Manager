using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LightingTilemapCollider {

    public class Base {
        protected GameObject gameObject;
        protected TilemapProperties properties = new TilemapProperties();

        public TilemapProperties Properties {
            get => properties;
        }

        public void SetGameObject(GameObject gameObject) {
            this.gameObject = gameObject;
        }

        public bool UpdateProperties() {
			properties.tilemap = gameObject.GetComponent<Tilemap>();

			if (properties.tilemap == null) {
				return(false);
			}

			properties.grid = properties.tilemap.layoutGrid;

			if (properties.grid == null) {
				Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid", gameObject);
				return(false);
			} else {
				properties.cellSize = properties.grid.cellSize;
				properties.cellGap = properties.grid.cellGap;
			}

			properties.cellAnchor = properties.tilemap.tileAnchor;

			return(true);
		}
    }
}