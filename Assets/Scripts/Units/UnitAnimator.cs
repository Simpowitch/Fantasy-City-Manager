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
    [SerializeField] SpriteRenderer body = null;
    [SerializeField] SpriteRenderer head = null;
    [SerializeField] SpriteRenderer lHand = null;
    [SerializeField] SpriteRenderer lFoot = null;
    [SerializeField] SpriteRenderer rHand = null;
    [SerializeField] SpriteRenderer rFoot = null;
    [SerializeField] SpriteRenderer itemRenderer = null;

    [SerializeField] Sprite[] bodies = new Sprite[3];
    [SerializeField] Sprite[] heads = new Sprite[3];
    [SerializeField] Sprite[] lHands = new Sprite[3];
    [SerializeField] Sprite[] lFeet = new Sprite[3];
    [SerializeField] Sprite[] rHands = new Sprite[3];
    [SerializeField] Sprite[] rFeet = new Sprite[3];


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
    Direction2D lastDirection;


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

    public void PlayActionAnimation(ActionAnimation actionAnimation) => PlayActionAnimation(Vector3.zero, actionAnimation);

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
        PlayActionAnimation(offset, actionAnimation);
    }

    public void PlayActionAnimation(Vector3 dir, ActionAnimation actionAnimation)
    {
        switch (actionAnimation)
        {
            case ActionAnimation.Idle:
            case ActionAnimation.Drink:
                ChangeSpriteDirection(dir);
                break;
            case ActionAnimation.Harvest:
            case ActionAnimation.ChopWood:
            case ActionAnimation.Mine:
            case ActionAnimation.PlantSeed:
            case ActionAnimation.Build:
            case ActionAnimation.ServingTavern:
                ChangeSpriteDirection(Direction2D.S);
                break;
        }
        if (dir != Vector3.zero)
        {
            Vector3 normalizedDir = dir.normalized;
            animator.SetFloat(xDir, normalizedDir.x);
            animator.SetFloat(yDir, normalizedDir.y);
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
        int spriteIndex = 0;
        int animatorDirectionIndex = 0;
        bool flip = false;
        switch (direction)
        {
            case Direction2D.N:
                spriteIndex = 2;
                animatorDirectionIndex = 0;
                break;
            case Direction2D.NE:
            case Direction2D.E:
            case Direction2D.SE:
                spriteIndex = 1;
                animatorDirectionIndex = 1;
                break;
            case Direction2D.S:
                spriteIndex = 0;
                animatorDirectionIndex = 2;
                break;
            case Direction2D.SW:
            case Direction2D.W:
            case Direction2D.NW:
                spriteIndex = 1;
                animatorDirectionIndex = 3;
                flip = true;
                break;
        }


        //Set orders in layer to correctly place object infront or behind other objects
        head.sortingOrder = headOrderInLayer[animatorDirectionIndex];
        lHand.sortingOrder = lHandOrderInLayer[animatorDirectionIndex];
        rHand.sortingOrder = rHandOrderInLayer[animatorDirectionIndex];
        lFoot.sortingOrder = lFootOrderInLayer[animatorDirectionIndex];
        rFoot.sortingOrder = rFootOrderInLayer[animatorDirectionIndex];

        //Flip sprite renderer dependent on left/right
        head.flipX = flip;
        body.flipX = flip;
        lFoot.flipX = flip;
        rFoot.flipX = flip;
        lHand.flipX = flip;
        rHand.flipX = flip;

        //Change sprites
        if (heads.Length > 0)
            head.sprite = heads[spriteIndex];
        if (bodies.Length > 0)
            body.sprite = bodies[spriteIndex];
        if (lFeet.Length > 0)
            lFoot.sprite = lFeet[spriteIndex];
        if (rFeet.Length > 0)
            rFoot.sprite = rFeet[spriteIndex];
        if (lHands.Length > 0)
            lHand.sprite = lHands[spriteIndex];
        if (rHands.Length > 0)
            rHand.sprite = rHands[spriteIndex];

        lastDirection = direction;
    }
}
