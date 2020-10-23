[System.Serializable]
public class MapData
{
    public int cellCountX, cellCountY;

    public HexCellData[] cells;

    public void Save(int cellCountX, int cellCountY, HexCell[] cells)
    {
        this.cellCountX = cellCountX;
        this.cellCountY = cellCountY;
        this.cells = new HexCellData[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            this.cells[i] = new HexCellData(cells[i]);
        }
    }
}