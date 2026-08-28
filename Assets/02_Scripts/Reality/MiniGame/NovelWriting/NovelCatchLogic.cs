using UnityEngine;
public class NovelCatchLogic
{
    private const float ZONE_WIDTH = 0.3f;
    private const float KEY_SPEED = 0.6f;

    // TODO 희준 : 기획 단어 표 확정 후 데이터 테이블로 이동
    private static readonly string[] KEY_WORDS =
    {
    "달빛", "모험", "새벽", "그림자", "이야기", "메아리", "용사", "마법", "운명", "여행", "별자리", "높은음자리", "지나치다", "아름다운", "생각할수록", "쌍둥이자리",
    };

    public string CreateKeyText()
    {
        string txt = KEY_WORDS[Random.Range(0, KEY_WORDS.Length)];

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
