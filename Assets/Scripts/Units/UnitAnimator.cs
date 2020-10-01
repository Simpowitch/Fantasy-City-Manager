using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    //[Header("Renderers and sprites")]
    //[SerializeField] SpriteRenderer body = null;
    //[SerializeField] SpriteRenderer head = null;
    //[SerializeField] SpriteRenderer lHand = null;
    //[SerializeField] SpriteRenderer lFoot = null;
    //[SerializeField] SpriteRenderer rHand = null;
    //[SerializeField] SpriteRenderer rFoot = null;

    //[SerializeField] Sprite[] bodies = null;
    //[SerializeField] Sprite[] heads = null;
    //[SerializeField] Sprite[] lHands = null;
    //[SerializeField] Sprite[] lFeet = null;
    //[SerializeField] Sprite[] rHands = null;
    //[SerializeField] Sprite[] rFeets = null;

    [SerializeField] GameObject carryObjectHolder = null;

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    [SerializeField] string idle = "idle";
    [SerializeField] string walk = "walking";
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
        ChangeSpriteDirections(dir);
        if (lastTag == idle)
            return;
        lastTag = idle;
        animator.SetTrigger(lastTag);
    }

    public void PlayWalkAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == walk)
            return;
        lastTag = walk;
        animator.SetTrigger(lastTag);
    }


    public void PlayHarvestFoodAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == harvest)
            return;
        lastTag = harvest;
        animator.SetTrigger(lastTag);
    }

    public void PlayMiningAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == mine)
            return;
        lastTag = mine;
        animator.SetTrigger(lastTag);
    }

    public void PlayWoodChopAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == chop)
            return;
        lastTag = chop;
        animator.SetTrigger(lastTag);
    }

    public void PlayPlantSeedAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == plant)
            return;
        lastTag = plant;
        animator.SetTrigger(lastTag);
    }

    public void PlayBuildAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
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

    private void ChangeSpriteDirections(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;
        //Exchange sprites dependent on the movement
    }
}
