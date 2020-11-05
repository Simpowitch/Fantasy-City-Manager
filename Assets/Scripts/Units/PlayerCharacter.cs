using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IViewable
{
    [SerializeField] UnitAnimator unitAnimator = null; public UnitAnimator UnitAnimator => unitAnimator;
    IMoveVelocity movement = null;
    string playerName = "Player Playsson";
    string actionDescription;
    Need moving;
    [SerializeField] City city = null; public City City => city;

    const float DISTANCETOARRIVAL = 0.1f;
    

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

    public void SetColliderState(bool state) => GetComponent<Collider2D>().enabled = state;

    public void MoveTo(Vector3 position, UnitAnimator.ActionAnimation animationOnArrival, bool ignoreCollisions)
    {
        StopCoroutine(AutoMoveTo(Vector3.zero, UnitAnimator.ActionAnimation.Idle, false));
        StartCoroutine(AutoMoveTo(position, animationOnArrival, ignoreCollisions));
    }

    //Checks wheter or not the other tile is a neighbor of the tile this transform is on
    public bool IsWithinInteractionRange(ObjectTile otherTile)
    {
        ObjectTile onTile = otherTile.grid.GetGridObject(this.transform.position);
        return (onTile.GetNeighbors().Contains(otherTile) || onTile == otherTile);
    }

    private void PlayerCharacterInfoChanged() => InfoChangeHandler?.Invoke(this);

    IEnumerator AutoMoveTo(Vector3 movementTarget, UnitAnimator.ActionAnimation animationOnArrival, bool ignoreCollisions)
    {
        SetColliderState(!ignoreCollisions);
        movement.MoveTowards(movementTarget);
        unitAnimator.PlayWalkAnimation(movementTarget - transform.position);
        bool hasArrived = Vector2.Distance(movementTarget, this.transform.position) < DISTANCETOARRIVAL;
        while (!hasArrived)
        {
            yield return new WaitForEndOfFrame();
            hasArrived = Vector2.Distance(movementTarget, this.transform.position) < DISTANCETOARRIVAL;
        }
        movement.SetVelocity(Vector3.zero);
        unitAnimator.PlayActionAnimation(animationOnArrival);
    }

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
