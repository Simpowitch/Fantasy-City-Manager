using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DistrictInteractionMenu : MonoBehaviour
{
    [SerializeField] TabGroup mainGroup = null;
    [SerializeField] GameObject changeInfoPanel = null;
    [SerializeField] TextMeshProUGUI districtNameText = null;
    [SerializeField] TextMeshProUGUI currentInfoText = null;
    [SerializeField] TextMeshProUGUI newInfoText = null;
    [SerializeField] City city = null;

    private HexCell markedCell;
    int categoryIndex = 0;
    int subtypeIndex = 0;

    [Header("Prefabs")]
    [SerializeField] Pool<Home>[] homes = new Pool<Home>[5];


    private void Awake()
    {
        HexCell.OnHexCellClicked += CellClicked;
        Hide();
    }

    public void CellClicked(HexCell cell)
    {
        if (this.gameObject.activeSelf)
            Hide();
        else
            Show(cell);
    }

    private void Hide()
    {
        this.gameObject.SetActive(false);
        if (markedCell)
            markedCell.ShowSelected(false);
        markedCell = null;
    }

    private void Show(HexCell cell)
    {
        this.gameObject.SetActive(true);
        this.transform.position = cell.Position;
        markedCell = cell;
        markedCell.ShowSelected(true);
        mainGroup.ResetSelection();
        SetChangeInfoPanelState(false);

        string information = cell.District != null ? cell.District.GetCurrentInformation() : "Empty";
        districtNameText.text = cell.District != null ? cell.District.Name : cell.IsLand ? "Land" : "Ocean";
        SetCurrentInfo(information);
    }

    public void SetCategory(int index)
    {
        categoryIndex = index;
    }

    public void SetSubType(int index)
    {
        subtypeIndex = index;
        District district = markedCell.District;
        if (district != null)
        {
            if (district.DistrictCategoryIndex != categoryIndex || district.DistrictSubType != subtypeIndex)
            {
                SetChangeInfoPanelState(true);
                SetNewInfo("Changing the district type will have these effects: ");
            }
        }
        else
            SetChangeInfoPanelState(true);
    }

    private void SetChangeInfoPanelState(bool state)
    {
        changeInfoPanel.SetActive(state);
    }

    public void ConfirmNewDistrict()
    {
        if (markedCell.District != null)
            city.cityStats.RemoveDistrict(markedCell.District);

        District newDistrict = null;
        District.DistrictCategory category = (District.DistrictCategory)categoryIndex;
        switch (category)
        {
            case District.DistrictCategory.Residential:
                newDistrict = new ResidentialDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                markedCell.SpawnStructures(newDistrict.HouseCapacity, homes[newDistrict.DistrictSubType].array);
                break;
            case District.DistrictCategory.Culture:
                newDistrict = new CultureDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                break;
            case District.DistrictCategory.Services:
                newDistrict = new ServicesDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                break;
            case District.DistrictCategory.Security:
                newDistrict = new SecurityDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                break;
            case District.DistrictCategory.Producer:
                newDistrict = new ProducerDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                break;
            case District.DistrictCategory.Trade:
                newDistrict = new TradeDistrict(subtypeIndex, markedCell);
                markedCell.District = newDistrict;
                break;
        }

        city.cityStats.AddDistrict(markedCell.District);

        Hide();
    }

    public void SetCurrentInfo(string text) => currentInfoText.text = text;
    public void SetNewInfo(string text) => newInfoText.text = text;
}
