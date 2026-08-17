using System.Collections.Generic;


public static class ReadOnlyListRxtensions
{
    public static bool ContainsItem<T>(this IReadOnlyList<T> list, T item)
    {
        // 원본이 List<T> 라면 빠른 Contains 메서드를 사용
        if (list is List<T> originList)
        {
            return originList.Contains(item);
        }

        // 원본이 List가 아니라면 LINQ 확장 메서드로 탐색
        return System.Linq.Enumerable.Contains(list, item);
    }
}
