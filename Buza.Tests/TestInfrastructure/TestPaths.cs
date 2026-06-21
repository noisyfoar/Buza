namespace Buza.Tests.TestInfrastructure;

public static class TestPaths
{
    public static string ProjectRoot { get; } = FindProjectRoot();

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "Buza.csproj");
            if (File.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Не удалось найти корень проекта Buza.csproj.");
    }
}
