using System;

[Serializable]
public class BaseData
{
    public string Id;
}

[Serializable]
public class PoolData : BaseData
{
    public int InitSize;
}

[Serializable]
public class PreLoadAssetData : BaseData
{
    public string Address;
    public string AssetType;
}