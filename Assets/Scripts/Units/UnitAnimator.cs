using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [Header("Renderers and sprites")]
    [SerializeField] SpriteRenderer body = null;
    [SerializeField] SpriteRenderer head = null;
    [SerializeField] SpriteRenderer lHand = null;
    [SerializeField] SpriteRenderer lFoot = null;
    [SerializeField] SpriteRenderer rHand = null;
    [SerializeField] SpriteRenderer rFoot = null;

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


    [SerializeField] GameObject carryObjectHolder = null;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    [SerializeField] string idle = "idle";
    [SerializeField] string walkUp = "walkUp";
    [SerializeField] string walkRight = "walkRight";
    [SerializeField] string walkDown = "walkDown";
    [SerializeField] string walkLeft = "walkLeft";

    [SerializeField] string harvest = "harvest";
    [SerializeField] string chop = "chop";
    [SerializeField] string mine = "mine";
    [SerializeField] string plant = "plant";
    [SerializeField] string build = "build";

    string lastTag;

    private void Awake()
    {
        lastTag = idle;
    }

    public void PlayIdleAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == idle)
            return;
        lastTag = idle;
        animator.SetTrigger(lastTag);
    }

    public void PlayWalkAnimation(Vector3 dir)
    {
        Direction2D direction = ChangeSpriteDirection(dir);
        string newTag = GetWalkTag(direction);
        if (lastTag == newTag)
            return;
        lastTag = newTag;
        animator.SetTrigger(lastTag);
    }


    public void PlayHarvestFoodAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == harvest)
            return;
        lastTag = harvest;
        animator.SetTrigger(lastTag);
    }

    public void PlayMiningAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == mine)
            return;
        lastTag = mine;
        animator.SetTrigger(lastTag);
    }

    public void PlayWoodChopAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == chop)
            return;
        lastTag = chop;
        animator.SetTrigger(lastTag);
    }

    public void PlayPlantSeedAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == plant)
            return;
        lastTag = plant;
        animator.SetTrigger(lastTag);
    }

    public void PlayBuildAnimation(Vector3 dir)
    {
        ChangeSpriteDirection(Direction2D.S);
        if (lastTag == build)
            return;
        lastTag = build;
        animator.SetTrigger(lastTag);
    }

    public void PlayCarryObjectAnimation(CityResource.Type type)
    {
        carryObjectHolder.SetActive(true);
    }

    public void PlayCarryNoObjectAnimation()
    {
        carryObjectHolder.SetActive(false);
    }

    //Converts a vector to a direction and then calls for sprite changes
    private Direction2D ChangeSpriteDirection(Vector3 dir)
    {
        Direction2D direction;
        if (dir == Vector3.zero)
            direction = Direction2D.S;
        else
        {
            direction = Utility.GetDirection(this.transform.position, this.transform.position + dir);
        }
        ChangeSpriteDirection(direction);
        return direction;
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
    }

    private string GetWalkTag(Direction2D direction)
    {
        switch (direction)
        {
            case Direction2D.N:
                return walkUp;
            case Direction2D.NE:
            case Direction2D.E:
            case Direction2D.SE:
                return walkRight;
            case Direction2D.S:
                return walkDown;
            case Direction2D.SW:
            case Direction2D.W:
            case Direction2D.NW:
                return walkLeft;
            default:
                return "";
        }
    }
}
