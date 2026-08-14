using UnityEngine;

public static class UnityUtility
{
    public static bool ValidateReference(Object reference, string referenceName)
    {
        if (reference != null)
        {
            return true;
        }

        Debug.LogError($"'{referenceName}'가 할당되지 않았습니다.");
        return false;
    }
}