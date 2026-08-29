using UnityEngine;

public class AwayReportTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        AwayReportFlow.SetRealLobbyActive(true);
    }

    private void OnDisable()
    {
        AwayReportFlow.SetRealLobbyActive(false);
    }
}
