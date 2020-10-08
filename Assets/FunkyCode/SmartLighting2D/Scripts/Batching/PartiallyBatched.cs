using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Non Batched Objects While Batching Is Enabled

public class PartiallyBatchedTilemap {
	public VirtualSpriteRenderer virtualSpriteRenderer;
	public Vector2 polyOffset; 
	public Vector2 tileSize;
	public LightingTile tile;

	#if UNITY_2017_4_OR_NEWER
		public LightingTilemapCollider2D tilemap;
	#endif
}

public class PartiallyBatchedCollider {
	public LightingCollider2D collider;
}

public class LightingAtlasBatches {
	public List<PartiallyBatchedCollider> colliderList = new List<PartiallyBatchedCollider>();
	public List<PartiallyBatchedTilemap> tilemapList = new List<PartiallyBatchedTilemap>();
}
