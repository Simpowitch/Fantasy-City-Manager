using UnityEngine;
using UnityEngine.Tilemaps;

public class CitySetup : MonoBehaviour
{
    public City city;
    public Player player;
    public Tilemap groundTilemap;
    public Tile basicGround;
    public Transform bottomLeftCorner, topRightCorner;

    [Header("Settings")]
    [SerializeField] int xSize = 50, ySize = 30;

    [SerializeField] int startCitizens = 5;

    [SerializeField] int startGold = 10;
    [SerializeField] int startWood = 10;
    [SerializeField] int startStone = 10;
    [SerializeField] int startIron = 10;
    [SerializeField] int startFood = 10;

    [SerializeField] int cameraRestraintEdge = 5;

    private void Start()
    {
        SetupMap();
        city.Setup(xSize, ySize, startGold, startWood, startStone, startIron, startFood, startCitizens, player.playerCharacter.transform);
        player.ActiveCity = city;
        SetupCameraRestraints();
    }

    public void SetupMap()
    {
        //Ground
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), basicGround);
            }
        }
    }

    private void SetupCameraRestraints()
    {
        bottomLeftCorner.position = new Vector3(-cameraRestraintEdge, -cameraRestraintEdge);
        topRightCorner.position = new Vector3(cameraRestraintEdge + xSize, cameraRestraintEdge + ySize);
    }
}
