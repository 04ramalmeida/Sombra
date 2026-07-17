namespace Sombra.Extensions;

public static class TagExtensions
{
    public static string GetRndTag(this List<string> tags)
    {
        var index = Random.Shared.Next(0, tags.Count);
        return tags[index];
    }
}