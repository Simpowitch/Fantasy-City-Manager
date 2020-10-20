using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    public enum ActionAnimation
    {
        Idle,
        Harvest,
        ChopWood,
        Mine,
        PlantSeed,
        Build,
        Drink,
        ServingTavern
    }

    [Header("Renderers and sprites")]
    [SerializeField] SpriteRenderer headRenderer = null;
    [SerializeField] SpriteRenderer bodyRenderer = null;
    [SerializeField] SpriteRenderer lHandRenderer = null;
    [SerializeField] SpriteRenderer rHandRenderer = null;
    [SerializeField] SpriteRenderer lFootRenderer = null;
    [SerializeField] SpriteRenderer rFootRenderer = null;
    [SerializeField] SpriteRenderer itemRenderer = null;

    [SerializeField] Body body = null;
    public Body Body 
    { 
        get => body;
        set
        {
            body = value;

            //Change sprites
            ChangeSprite(Body.Bodypart.Head, headRenderer, BodypartSpriteGroup.SpriteDirection.Down);
            ChangeSprite(Body.Bodypart.Body, bodyRenderer, BodypartSpriteGroup.SpriteDirection.Down);
            ChangeSprite(Body.Bodypart.LeftHand, lHandRenderer, BodypartSpriteGroup.SpriteDirection.Down);
            ChangeSprite(Body.Bodypart.RightHand, rHandRenderer, BodypartSpriteGroup.SpriteDirection.Down);
            ChangeSprite(Body.Bodypart.LeftFoot, lFootRenderer, BodypartSpriteGroup.SpriteDirection.Down);
            ChangeSprite(Body.Bodypart.RightFoot, rFootRenderer, BodypartSpriteGroup.SpriteDirection.Down);
        }
    }

    [SerializeField] int[] headOrderInLayer = new int[4];
    [SerializeField] int[] lHandOrderInLayer = new int[4];
    [SerializeField] int[] rHandOrderInLayer = new int[4];
    [SerializeField] int[] lFootOrderInLayer = new int[4];
    [SerializeField] int[] rFootOrderInLayer = new int[4];

    [Header("Animations")]
    [SerializeField] Animator animator = null;

    [SerializeField] string xDir = "xDir";
    [SerializeField] string yDir = "yDir";

    [SerializeField] string isWalking = "walking";
    [SerializeField] string isCarrying = "carrying";

    string lastAnimationTag = ActionAnimation.Idle.ToString();
    Direction2D lastDirection = Direction2D.S;

    private void Start()
    {
        ChangeSpriteDirection(lastDirection);
        PlayActionAnimation(Direction2D.S, ActionAnimation.Idle);
    }

    public void SetIsCarrying(CityResource cityResource)
    {
        animator.SetBool(isCarrying, cityResource != null);
        itemRenderer.sprite = cityResource != null ? cityResource.Sprite : null;
    }

    public void PlayWalkAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(dir);

        Vector3 normalizedDir = dir.normalized;

        animator.SetBool(isWalking, true);
        animator.SetFloat(xDir, normalizedDir.x);
        animator.SetFloat(yDir, normalizedDir.y);
        itemRenderer.enabled = true;
        lastAnimationTag = "Walking";
    }

    public void PlayActionAnimation(ActionAnimation actionAnimation) => PlayActionAnimation(lastDirection, actionAnimation);
    public void PlayActionAnimation(Vector3 dir, ActionAnimation actionAnimation) => PlayActionAnimation(Utility.GetDirection(dir), actionAnimation);

    public void PlayActionAnimation(Direction2D dir, ActionAnimation actionAnimation)
    {
        Vector3 offset = Vector3.zero;
        switch (dir)
        {
            case Direction2D.N:
                offset = new Vector3(0, 1);
                break;
            case Direction2D.NE:
            case Direction2D.E:
            case Direction2D.SE:
                offset = new Vector3(1, 0);
                break;
            case Direction2D.S:
                offset = new Vector3(0, -1);
                break;
            case Direction2D.SW:
            case Direction2D.W:
            case Direction2D.NW:
                offset = new Vector3(-1, 0);
                break;
        }

            Vector3 normalizedDir = offset.normalized;
            animator.SetFloat(xDir, normalizedDir.x);
            animator.SetFloat(yDir, normalizedDir.y);

        switch (actionAnimation)
        {
            case ActionAnimation.Idle:
            case ActionAnimation.Drink:
            case ActionAnimation.ServingTavern:
                ChangeSpriteDirection(dir);
                break;
            case ActionAnimation.Harvest:
            case ActionAnimation.ChopWood:
            case ActionAnimation.Mine:
            case ActionAnimation.PlantSeed:
            case ActionAnimation.Build:
                ChangeSpriteDirection(Direction2D.S);
                break;
        }
        if (lastAnimationTag == actionAnimation.ToString())
            return;
        lastAnimationTag = actionAnimation.ToString();
        animator.SetTrigger(lastAnimationTag.ToString());
        animator.SetBool(isWalking, false);

        switch (actionAnimation)
        {
            case ActionAnimation.Idle:
                itemRenderer.enabled = true;
                break;
            case ActionAnimation.Drink:
            case ActionAnimation.Harvest:
            case ActionAnimation.ChopWood:
            case ActionAnimation.Mine:
            case ActionAnimation.PlantSeed:
            case ActionAnimation.Build:
            case ActionAnimation.ServingTavern:
                itemRenderer.enabled = false;
                break;
        }
    }

    //Converts a vector to a direction and then calls for sprite changes
    private void ChangeSpriteDirection(Vector3 dir)
    {
        Direction2D direction;
        if (dir == Vector3.zero)
            direction = lastDirection; //Keep the old direction
        else
            direction = Utility.GetDirection(this.transform.position, this.transform.position + dir); //Calculate new direction
        ChangeSpriteDirection(direction);
    }
    //Exchange sprites dependent on the direction of the action/movement
    public void ChangeSpriteDirection(Direction2D direction)
    {
        if (direction == lastDirection)
            return;

        BodypartSpriteGroup.SpriteDirection spriteDirection = BodypartSpriteGroup.SpriteDirection.Up;
        int directionIndex = 0;
        bool flip = false;
        switch (direction)
        {
            case Direction2D.N:
                spriteDirection = BodypartSpriteGroup.SpriteDirection.Up;
                directionIndex = 0;
                break;
            case Direction2D.NE:
            case Direction2D.E:
            case Direction2D.SE:
                spriteDirection = BodypartSpriteGroup.SpriteDirection.Right;
                directionIndex = 1;
                break;
            case Direction2D.S:
                spriteDirection = BodypartSpriteGroup.SpriteDirection.Down;
                directionIndex = 2;
                break;
            case Direction2D.SW:
            case Direction2D.W:
            case Direction2D.NW:
                spriteDirection = BodypartSpriteGroup.SpriteDirection.Right;
                directionIndex = 3;
                flip = true;
                break;
        }

        //Set orders in layer to correctly place object infront or behind other objects
        headRenderer.sortingOrder = headOrderInLayer[directionIndex];
        lHandRenderer.sortingOrder = lHandOrderInLayer[directionIndex];
        rHandRenderer.sortingOrder = rHandOrderInLayer[directionIndex];
        lFootRenderer.sortingOrder = lFootOrderInLayer[directionIndex];
        rFootRenderer.sortingOrder = rFootOrderInLayer[directionIndex];

        //Flip sprite renderer dependent on left/right
        headRenderer.flipX = flip;
        bodyRenderer.flipX = flip;
        lFootRenderer.flipX = flip;
        rFootRenderer.flipX = flip;
        lHandRenderer.flipX = flip;
        rHandRenderer.flipX = flip;

        //Change sprites
        ChangeSprite(Body.Bodypart.Head, headRenderer, spriteDirection);
        ChangeSprite(Body.Bodypart.Body, bodyRenderer, spriteDirection);
        ChangeSprite(Body.Bodypart.LeftHand, lHandRenderer, spriteDirection);
        ChangeSprite(Body.Bodypart.RightHand, rHandRenderer, spriteDirection);
        ChangeSprite(Body.Bodypart.LeftFoot, lFootRenderer, spriteDirection);
        ChangeSprite(Body.Bodypart.RightFoot, rFootRenderer, spriteDirection);

        lastDirection = direction;
    }

    private void ChangeSprite(Body.Bodypart bodypart, SpriteRenderer spriteRenderer, BodypartSpriteGroup.SpriteDirection spriteDirection)
    {
        Sprite foundSprite = Body.GetBodypartSpriteGroup(bodypart).GetSprite(spriteDirection);
        spriteRenderer.sprite = foundSprite ? foundSprite : spriteRenderer.sprite;
    }
}
