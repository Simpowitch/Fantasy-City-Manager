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

    [Header("Animations")]
    [SerializeField] Animator animator = null;
    [SerializeField] string idle = "idle";
    [SerializeField] string walk = "walk";
    [SerializeField] string work = "work";

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
        animator.SetTrigger(idle);
    }

    public void PlayWalkAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == walk)
            return;
        lastTag = walk;
        animator.SetTrigger(walk);
    }

    public void PlayHarvestFoodAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == work)
            return;
        lastTag = work;
        animator.SetTrigger(work);
    }

    public void PlayMiningAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == work)
            return;
        lastTag = work;
        animator.SetTrigger(work);
    }

    public void PlayWoodChopAnimation(Vector3 dir)
    {
        ChangeSpriteDirections(dir);
        if (lastTag == work)
            return;
        lastTag = work;
        animator.SetTrigger(work);
    }
    public void PlayPlantSeedAnimation(Vector3 dir)
    {

    }
    public void PlayCarryObjectAnimation(CityResource.Type type)
    {

    }
    public void PlayCarryNoObjectAnimation()
    {

    }

    private void ChangeSpriteDirections(Vector3 dir)
    {
        if (dir == Vector3.zero)
            return;
        //Exchange sprites dependent on the movement
    }
}
