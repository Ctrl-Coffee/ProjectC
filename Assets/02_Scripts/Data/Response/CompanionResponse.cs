using System;

[Serializable]
public class SaveCompanionResponse : CommonResponse
{
    
}

[Serializable]
public class LoadCompanionResponse : CommonResponse
{
    public CompanionWrapperDto data;
}
