using UnityEngine;

public class LobbyController
{
    private GameObject _backgroundInstance;

    public void Enter(GameObject backgroundPrefab)
    {
        _backgroundInstance = Object.Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity);
    }

    public void Release()
    {
        if (_backgroundInstance == null)
            return;

        Object.Destroy(_backgroundInstance);
        _backgroundInstance = null;
    }

}
