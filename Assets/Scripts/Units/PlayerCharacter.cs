using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IViewable
{
    [SerializeField] UnitAnimator unitAnimator = null; public UnitAnimator UnitAnimator => unitAnimator;
    IMoveVelocity movement = null;
    string playerName = "Player Playsson";
    string actionDescription;
    Need moving;

    private void Awake()
    {
        movement = GetComponent<IMoveVelocity>();
    }

    private void Start()
    {
        moving = new Need(Need.NeedType.Recreation, 0.1f, 1);
        moving.OnNeedValuesChanged += PlayerCharacterInfoChanged;
    }


    public void SetAim(Vector3 aim)
    {
        movement.MoveTowards(aim);
        if (aim != transform.position) //Target aim is different from current position
        {
            unitAnimator.PlayWalkAnimation(aim - transform.position);
            ActionDescription = "Moving";
            moving.Satisfy();
        }
        else if (ActionDescription != "Idle")
        {
            unitAnimator.PlayActionAnimation(UnitAnimator.ActionAnimation.Idle);
            ActionDescription = "Idle";
        }
    }


    private void PlayerCharacterInfoChanged() => InfoChangeHandler?.Invoke(this);


    #region Viewable interface
    public InfoChangeHandler InfoChangeHandler { get; set; }

    public string ActionDescription
    {
        get => actionDescription;
        set
        {
            if (value != actionDescription)
            {
                actionDescription = value;
                PlayerCharacterInfoChanged();
            }
        }
    }
    public string Name
    {
        get => playerName;
        set
        {
            playerName = value;
            PlayerCharacterInfoChanged();
        }
    }
    public float GetPrimaryStatValue() => moving.CurrentValue;
    public string GetPrimaryStatName() => "Happiness";
    public Need[] GetNeeds()
    {
        Need[] needs = new Need[1];
        needs[0] = moving;
        return needs;
    }
    public string GetSpeciality() => "Burgomaster";
    #endregion
}
