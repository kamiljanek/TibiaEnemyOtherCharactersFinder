namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class CharactersControllerTestTemplate
{
    public static IEnumerable<object[]> GetInvalidLengthRouteParameters()
    {
        string[] invalidLengthRouteParameters =
        {
            new ('X', 1),
            new ('X', 51)
        };

        foreach (var parameter in invalidLengthRouteParameters)
        {
            yield return new object[] { parameter };
        }
    }

    public static IEnumerable<object[]> GetUnacceptableRouteParameters()
    {
        string[] unacceptableRouteParameters =
        {
            "kkają",
            "kk=aj",
            "kka|j",
            "kka6j",
            "kka*j",
        };

        foreach (var parameter in unacceptableRouteParameters)
        {
            yield return new object[] { parameter };
        }
    }

    public static IEnumerable<object[]> GetUnacceptablePages()
    {
        int[] unacceptablePages =
        {
            -50,
            -1,
            0
        };

        foreach (var page in unacceptablePages)
        {
            yield return new object[] { page };
        }
    }

    public static IEnumerable<object[]> GetUnacceptablePageSizes()
    {
        int[] unacceptablePages =
        {
            -50,
            -1,
            0,
            101,
            200
        };

        foreach (var page in unacceptablePages)
        {
            yield return new object[] { page };
        }
    }
}