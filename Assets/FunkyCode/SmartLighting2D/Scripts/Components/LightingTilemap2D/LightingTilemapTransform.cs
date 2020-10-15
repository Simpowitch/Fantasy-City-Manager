using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightingTilemapTransform {
    private bool update = true;
	public bool UpdateNeeded {
		get => update;
		set => update = value;
	}

    private Vector2 scale = Vector2.one;
    public Vector2 position = Vector2.one;
	public Vector3 tilemapAnchor = Vector3.zero;
	public Vector3 tilemapCellSize = Vector3.zero;
	public Vector3 tilemapGapSize = Vector3.zero;

	public void Update(LightingTilemapCollider2D tilemapCollider2D) {
		Transform transform = tilemapCollider2D.transform;

	    Vector2 position2D = LightingPosition.Get(transform);
		Vector2 scale2D = transform.lossyScale;

		update = false;

        if (scale != scale2D) {
			scale = scale2D;

			update = true;
		}

        if (position != position2D) {
			position = position2D;

			update = true;
		}

		Tilemap tilemap = GetTilemap(tilemapCollider2D.gameObject);

		if (tilemap) {
			if (tilemapAnchor != tilemap.tileAnchor) {
				tilemapAnchor = tilemap.tileAnchor;
				update = true;
			}
		}

		Grid grid = GetGrid(tilemapCollider2D.gameObject);

		if (grid) {
			if(tilemapCellSize != grid.cellSize) {
				tilemapCellSize = grid.cellSize;

				update = true;
			}

			if (tilemapGapSize != grid.cellGap) {
				tilemapGapSize = grid.cellGap;

				update = true;
			}
		}
	}

	
	Tilemap tilemap = null;
	public Tilemap GetTilemap(GameObject gameObject) {
		if (tilemap == null) {
			tilemap = gameObject.GetComponent<Tilemap>();
		}
		return(tilemap);
	}

	Grid grid = null;
	public Grid GetGrid(GameObject gameObject) {
		if (grid == null) {
			Tilemap tilemap = GetTilemap(gameObject);

			if (tilemap != null) {
				grid = tilemap.layoutGrid;
			}
			
		}
		return(grid);
	}
}
