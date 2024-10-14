namespace PullPlanfix;

internal struct dotEnv
{
    public string TokenPlanfix;
    public string TokenSql;
    public string AddressSql;
    public string UserPlanfix;

    public static dotEnv Pull()
    {
        var args = new string?[4];
        if (!File.Exists(".env"))
        {
            Console.WriteLine("Не удалось получить переменные локальной среды");
            Environment.Exit(0);
        }
        foreach (var line in File.ReadAllLines(".env"))
        {
            if (line.StartsWith("planfix_token")) args[0] = line.Split('=')[1];
            else if (line.StartsWith("sql_uri")) args[1] = line.Split('=')[1];
            else if (line.StartsWith("sql_token")) args[2] = line.Split('=')[1];
            else if (line.StartsWith("planfix_user")) args[3] = line.Split('=')[1];
        }
        if (Array.Exists(args, i=>i is null))
        {
            Console.WriteLine("Не удалось получить переменные локальной среды");
            Environment.Exit(0);
        }
        return new dotEnv
        {
            TokenPlanfix = args[0]!,
            AddressSql = args[1]!,
            TokenSql = args[2]!,
            UserPlanfix = args[3]!,
        };
    }
}
