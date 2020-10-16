using UnityEngine;

[System.Serializable]
public class Body
{
    public enum Bodypart { Head, Body, LeftHand, RightHand, LeftFoot, RightFoot }
    [SerializeField] BodypartSpriteGroup head = null, body = null, leftHand = null, rightHand = null, leftFoot = null, rightFoot = null;
    public BodypartSpriteGroup GetBodypartSpriteGroup(Bodypart bodypart)
    {
        switch (bodypart)
        {
            case Bodypart.Head:
                return head;
            case Bodypart.Body:
                return body;
            case Bodypart.LeftHand:
                return leftHand;
            case Bodypart.RightHand:
                return rightHand;
            case Bodypart.LeftFoot:
                return leftFoot;
            case Bodypart.RightFoot:
                return rightFoot;
            default:
                return null;
        }
    }

    public void SetBodypartSpriteGroup(Bodypart bodypart, BodypartSpriteGroup bodypartSpriteGroup)
    {
        switch (bodypart)
        {
            case Bodypart.Head:
                head = bodypartSpriteGroup;
                break;
            case Bodypart.Body:
                body = bodypartSpriteGroup;
                break;
            case Bodypart.LeftHand:
                leftHand = bodypartSpriteGroup;
                break; 
            case Bodypart.RightHand:
                rightHand = bodypartSpriteGroup;
                break; 
            case Bodypart.LeftFoot:
                leftFoot = bodypartSpriteGroup;
                break; 
            case Bodypart.RightFoot:
                rightFoot = bodypartSpriteGroup;
                break; 
        }
    }
}
[System.Serializable]
public class BodypartSpriteGroup
{
    public enum SpriteDirection { Up, Right, Down }
    [SerializeField] Sprite up = null, right = null, down = null;
    public Sprite GetSprite(SpriteDirection dir)
    {
        switch (dir)
        {
            case SpriteDirection.Up:
                return up;
            case SpriteDirection.Right:
                return right;
            case SpriteDirection.Down:
                return down;
            default:
                return null;
        }
    }
}
