namespace Shared.RabbitMq.Conventions;

public static class StringHelper
{
    public static string ToRabbitSnakeCase(this string value) =>
        string.Concat(value.Select((x, i) =>
                i > 0 && value[i - 1] != '.' && value[i - 1] != '/' && char.IsUpper(x) ? "_" + x : x.ToString()))
            .ToLowerInvariant();
}