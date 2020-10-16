using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using System.Linq;

public enum Direction2D
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW,
}

public static class Utility
{
    /// <summary>
    /// Returns the mouse position in the world with z set to 0
    /// </summary>
    /// <param name="worldCamera"></param>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition(Camera worldCamera, Vector3 mousePosition)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    /// <summary>
    /// Returns the mouse position in the world with z set to 0
    /// </summary>
    /// <param name="worldCamera"></param>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public static Vector3 GetMouseWorldPosition()
    {
        return GetMouseWorldPosition(Camera.main, Input.mousePosition);
    }

    /// <summary>
    /// Returns a random rotation in 2d
    /// </summary>
    /// <returns></returns>
    public static Quaternion ReturnRandomRotation()
    {
        Quaternion quaternion = new Quaternion();
        quaternion = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
        return quaternion;
    }

    public static List<T> ShuffleList<T>(List<T> list)
    {
        int count = list.Count;
        for (var i = 0; i < count - 1; ++i)
        {
            int r = UnityEngine.Random.Range(i, count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
        return list;
    }



    public static T ReturnRandom<T>(T[] array)
    {
        if (array != null && array.Length > 0)
        {
            return array[Random.Range(0, array.Length)];
        }
        Debug.LogWarning("Array empty");
        return default;
    }

    public static T ReturnRandom<T>(List<T> list)
    {
        if (list != null && list.Count > 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        Debug.LogWarning("List empty");
        return default;
    }


    public static bool RandomizeBool(int percentageForTrue) => percentageForTrue <= Random.Range(0, 100);

#if UNITYEDITOR
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string pathFolderName, string assetName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = "Assets";
        if (pathFolderName != null)
        {
            path += "/" + pathFolderName;
        }

        string assetPathAndName = "";

        if (assetName == "")
        {
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        }
        else
        {
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetName + ".asset");
        }

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
#endif

    public static float PercentageToFactor(int wholePercentage)
    {
        return (float)wholePercentage / 100;
    }

    public static int FactorToPercentage(float floatPercentage)
    {
        return Mathf.RoundToInt(floatPercentage * 100);
    }

    public static string FactorToPercentageText(float floatPercentage)
    {
        int percentage = FactorToPercentage(floatPercentage);
        return percentage.ToString() + "%";
    }


    public static T TestVariableAgainstConditions<T>(T variable, params Func<T, bool>[] conditions)
    {
        if (variable == null)
        {
            return default(T);
        }
        bool flag = true;
        foreach (var con in conditions)
        {
            if (!con.Invoke(variable))
            {
                flag = false;
                break;
            }
        }
        if (flag)
        {
            return variable;
        }
        return default(T);
    }


    #region ConditionalChecks
    public static IList<T> PopulateListWithMatchingConditions<T>(this IList<T> outList, IList<T> inList, params Func<T, bool>[] conditions)
    {
        foreach (var item in inList)
        {
            if (item == null)
            {
                continue;
            }
            bool flag = true;
            foreach (var con in conditions)
            {
                if (!con.Invoke(item))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                outList.Add(item);
            }
        }
        return outList;
    }

    public static T ReturnRandomElementWithCondition<T>(this IList<T> inList, params Func<T, bool>[] conditions)
    {
        IList<T> passedItems = new List<T>().PopulateListWithMatchingConditions(inList, conditions);
        if (passedItems.Count > 0)
            return passedItems[Random.Range(0, passedItems.Count)];
        else
            return default(T);
    }

    public static T ReturnElementWithCondition<T>(this IList<T> inList, Func<IList<T>, int> index, params Func<T, bool>[] conditions)
    {
        IList<T> passedItems = new List<T>().PopulateListWithMatchingConditions(inList, conditions);
        if (passedItems.Count > 0)
        {
            return passedItems[index.Invoke(passedItems)];
        }
        else
        {
            return default(T);
        }
    }
    #endregion

    public static T1 GetClosest<T1, T2>(List<T1> list, T2 origin) where T1 : MonoBehaviour where T2 : MonoBehaviour
    {
        float minDistance = float.MaxValue;
        T1 closest = default;
        foreach (var item in list)
        {
            float distance = Vector3.Distance(item.transform.position, origin.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }
        return closest;
    }

    public static T GetClosest<T>(List<T> list, Vector3 origin) where T : MonoBehaviour
    {
        float minDistance = float.MaxValue;
        T closest = default;
        foreach (var item in list)
        {
            float distance = Vector3.Distance(item.transform.position, origin);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }
        return closest;
    }

    public static Vector3 GetClosest(IEnumerable<Vector3> list, Vector3 origin)
    {
        float minDistance = float.MaxValue;
        Vector3 closest = default;
        foreach (var item in list)
        {
            float distance = Vector3.Distance(item, origin);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }
        return closest;
    }

    public static bool Between(float a, float b, float testValue)
    {
        if (testValue < b && testValue > a)
        {
            return true;
        }
        return false;
    }

    public static float GetCompassDegree(Vector2 A, Vector2 B)
    {
        //difference
        var Delta = B - A;
        //use atan2 to get the angle; Atan2 returns radians
        var angleRadians = Mathf.Atan2(Delta.x, Delta.y);

        //convert to degrees
        var angleDegrees = angleRadians * Mathf.Rad2Deg;


        if (angleDegrees < 0)
            angleDegrees += 360;

        return angleDegrees;
    }

    public static Direction2D GetDirection(Vector3 dir) => GetDirection(Vector3.zero, dir);

    public static Direction2D GetDirection(Transform transform) => GetDirection(transform.position, transform.position + transform.up);

    public static Direction2D GetDirection(Vector2 from, Vector2 to)
    {
        float degrees = GetCompassDegree(from, to);

        float degreesPerDirection = 360 / 8;

        float minDir = -degreesPerDirection / 2;
        float maxDir = minDir;

        for (Direction2D direction = Direction2D.N; direction <= Direction2D.NW; direction++)
        {
            maxDir += degreesPerDirection;

            if (Between(minDir, maxDir, degrees))
                return direction;
            minDir = maxDir;

        }
        return Direction2D.N;
    }


    public static bool IsDiagonal(Direction2D direction) => direction == Direction2D.NE || direction == Direction2D.SE || direction == Direction2D.SW || direction == Direction2D.NW;

    public static void MoveToLayer(Transform root, int layer, Transform[] layerChangeExceptions)
    {
        bool changeLayer = true;
        foreach (var exception in layerChangeExceptions)
        {
            if (exception == root)
                changeLayer = false;
        }
        if (changeLayer)
            root.gameObject.layer = layer;
        foreach (Transform child in root)
            MoveToLayer(child, layer, layerChangeExceptions);
    }

    public static string ReplaceWordInString(string originalString, string wordToReplace, string newWord)
    {
        string pattern = $@"\b{wordToReplace}\b";
        return Regex.Replace(originalString, pattern, newWord);
    }
}