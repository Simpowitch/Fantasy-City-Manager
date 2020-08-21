using UnityEngine;

public static class RNG
{
   public static bool PercentageIntTry(int percentage)
    {
        return percentage > Random.Range(0, 100);
    }
    public static bool FloatTry(float chance)
    {
        return chance > Random.Range(0f, 1f);
    }
}
