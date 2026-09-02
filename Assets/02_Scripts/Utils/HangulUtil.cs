using System.Collections.Generic;

public static class HangulUtil
{
    private const string CHO_SUNG = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";

    public static List<string> BuildTypingFrames(string word)
    {
        List<string> frames = new List<string>();
        string completed = "";

        foreach (char letter in word)
        {
            int code = letter - '가';
            int cho = code / 588;
            int jung = (code % 588) / 28;
            int jong = code % 28;

            frames.Add(completed + CHO_SUNG[cho]);
            frames.Add(completed + (char)('가' + cho * 588 + jung * 28));

            if (jong > 0)
            {
                frames.Add(completed + letter);
            }

            completed += letter;
        }

        return frames;
    }
}
