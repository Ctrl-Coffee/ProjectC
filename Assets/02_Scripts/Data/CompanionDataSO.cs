using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionDataSO", menuName = "Scriptable Objects/CompanionDataSO")]
public class CompanionDataSO : ScriptableObject
{
    public List<CompanionData> CompanionDataList;
}
