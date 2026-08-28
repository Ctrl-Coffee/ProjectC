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

    public static bool ValidateArrayReference(Object[] references, string referenceName)
    {
        if (references == null)
        {
            Debug.LogError($"'{referenceName}'가 할당되지 않았습니다.");
            return false;
        }

        for (int i = 0; i < references.Length; i++)
        {
            if (!ValidateReference(references[i], $"{referenceName}[{i}]"))
            {
                return false;
            }
        }

        return true;
    }
}