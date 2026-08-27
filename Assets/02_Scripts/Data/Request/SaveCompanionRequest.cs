using System;

[Serializable]
public class SaveCompanionRequest : AuthenticatedRequest
{
    public CompanionWrapperDto CompanionData;
}
