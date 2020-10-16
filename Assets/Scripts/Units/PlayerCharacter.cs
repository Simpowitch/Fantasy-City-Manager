using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IViewable
{
    [SerializeField] UnitAnimator unitAnimator = null;
    IMoveVelocity movement = null;
    string playerName = "Player Playsson";
    string actionDescription;
    public bool CanMove { get; set; } = true;
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


    private void Update()
    {
        Vector3 aim = transform.position;

        if (CanMove)
        {
            if (Input.GetKey(KeyCode.W)) //Up
                aim += new Vector3(0, 1);
            if (Input.GetKey(KeyCode.D)) //Right
                aim += new Vector3(1, 0);
            if (Input.GetKey(KeyCode.S)) //Down
                aim += new Vector3(0, -1);
            if (Input.GetKey(KeyCode.A)) //Left
                aim += new Vector3(-1, 0);
        }

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
