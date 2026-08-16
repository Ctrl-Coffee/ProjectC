using UnityEngine;

public class PerkInfoUI : UIBase
{
    [SerializeField] private PerkTreeLineDrawer _lineDrawer;

    public PerkTreeLineDrawer LineDrawer
    {
        get
        {
            return _lineDrawer;
        }
    }
}
