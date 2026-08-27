using UnityEngine;
public class NovelCatchLogic
{
    private const float ZONE_WIDTH = 0.25f;
    private const float KEY_SPEED = 0.5f;

    public string CreateKeyText()
    {
        int letterLength = Random.Range(2, 6);
        string txt = "";

        for (int i = 0; i < letterLength; i++)
        {
            char letter = (char)('가' + Random.Range(0, 11172));
            txt += letter;
        }

        return txt;
    }

    public CatchZone CreateZone(float keyWidth)
    {
        if (keyWidth >= ZONE_WIDTH)
        {
            Logger.LogWarning($"키 폭({keyWidth})이 영역 폭({ZONE_WIDTH}) 이상이라 성공이 불가능한 라운드입니다");
        }

        float left = Random.Range(0f, 1f - ZONE_WIDTH);
        float right = left + ZONE_WIDTH;

        return new CatchZone
        {
            Left = left,
            Right = right,
        };
    }

    public float GetKeyCenter(float elapsedSeconds, float keyWidth)
    {
        return keyWidth / 2f + Mathf.PingPong(elapsedSeconds * KEY_SPEED, 1f - keyWidth);
    }

    public bool Judge(float keyLeft, float keyRight, CatchZone zone)
    {
        return zone.Left <= keyLeft && keyRight <= zone.Right;
    }
}
