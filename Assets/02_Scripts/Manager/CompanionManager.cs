using System.Collections.Generic;

public class CompanionManager
{
    public IReadOnlyList<string> GetOwnedCompanionIds()
    {
        List<string> ids = new List<string>();
        foreach(OwnedCompanionData owned in GameManager.User.Companions)
        {
            ids.Add(owned.CompanionId);
        }

        return ids;
    }

    public OwnedCompanionData GetOwnedCompanion(string companionId)
    {
        foreach(OwnedCompanionData owned in GameManager.User.Companions)
        {
            if (owned.CompanionId == companionId)
            {
                return owned;
            }
        }
       
        return null;
    }

    public bool AddCompanion(string companionId)
    {
        if(GetOwnedCompanion(companionId) != null)
        {
            //TODO 희준 : 중복획득시 꿈의 조각 지급
            return false;
        }

        OwnedCompanionData ownedData = new OwnedCompanionData();
        ownedData.CompanionId = companionId;
        ownedData.Level = 1;
        GameManager.User.Companions.Add(ownedData);
        return true;
    }


}
