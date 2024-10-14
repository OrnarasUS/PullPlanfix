using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PullPlanfix;

public static class Program
{
    public static readonly Dictionary<int, string> Templates = [];

    static readonly dotEnv env = dotEnv.Pull();
    static readonly HttpClient client = new();

    static async Task Log(string message, bool isError = false) =>
        await File.AppendAllLinesAsync(".log", [$"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {(isError ? '~' : ' ')} {message}"]);

    public static async Task PullTemplates(int numTry = 0)
    {
        try
        {
            if (numTry == 3)
            {
                await Log("Все попытки исчерпаны!", true);
                return;
            }
            await Log($"Выгрузка шаблонов...{(numTry > 0 ? $" (Попытка {numTry + 1})" : "")}");
            var uri = $"https://{env.UserPlanfix}.planfix.ru/rest/task/templates?fields=id%2Cname";
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", env.TokenPlanfix);
            req.Headers.Add("accept", "application/json");
            var resp = await client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            var raw = JObject.Parse(json);
            if ((string?)raw["result"] != "success") throw new HttpRequestException();
            Templates.Clear();
            foreach (var i in (JArray)raw["templates"]!)
            {
                Templates.Add((int)i["id"]!, (string)i["name"]!);
            }
        }
        catch (Exception e)
        {
            await Task.Delay(1500);
            await Log($"{e.Message}\n{e.StackTrace}", true);
            await PullTemplates(numTry + 1);
        }
    }

    public static async Task<PlanfixTask?> Pull(int id, int numTry = 0)
    {
        try
        {
            if (numTry == 3)
            {
                await Log("Все попытки исчерпаны!", true);
                return null;
            }
            await Log($"Выгрузка №{id}...{(numTry > 0 ? $" (Попытка {numTry + 1})" : "")}");
            string[] fields = [
                #region Системные поля
                "id", "name", "status", "template",
                "counterparty", "assignees", "dateTime",
                "endDateTime",
                #endregion
                #region Пользовательские поля
                "114352", "114320", "114286",
                "114394", "114278", "114280",
                "114424", "114288"
                #endregion
                ];
            var uri = $"https://{env.UserPlanfix}.planfix.ru/rest/task/{id}?fields={string.Join("%2C", fields)}";
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", env.TokenPlanfix);
            req.Headers.Add("accept", "application/json");
            var resp = await client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<PlanfixResponse>(json);
            await Log($"Задача №{id} {(res.Task is null ? "не найдена" : "получена")}");
            return res.Task;
        }
        catch (Exception e)
        {
            await Task.Delay(1500);
            await Log($"{e.Message}\n{e.StackTrace}", true);
            return await Pull(id, numTry + 1);
        }
    }

    public static async Task Push(SqlTask record, int numTry = 0)
    {
        try
        {
            if (numTry == 3)
            {
                await Log("Все попытки исчерпаны!", true);
                return;
            }
            await Log($"Обновление задачи №{record.id}...{(numTry > 0 ? $" (Попытка {numTry + 1})" : "")}");
            var req = new HttpRequestMessage(HttpMethod.Post, env.AddressSql);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", env.TokenSql);
            req.Headers.Add("Prefer", "resolution=merge-duplicates");
            req.Content = new StringContent(JsonConvert.SerializeObject(record, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            }), new MediaTypeHeaderValue("application/json"));
            var resp = await client.SendAsync(req);
            if ((int)resp.StatusCode >= 300) throw new HttpRequestException($"StatusCode: {(int)resp.StatusCode}");
        }
        catch (Exception e)
        {
            await Task.Delay(1500);
            await Log($"{e.Message}\n{e.StackTrace}", true);
            await Push(record, numTry + 1);
        }
    }

    public static async Task Main()
    {
        try
        {
            await PullTemplates();
            for (int i = 1; i <= 200000; i++)
            {
                await Task.Delay(1000);
                if (await Pull(i) is PlanfixTask pf) await Push(new SqlTask(pf));
            }
        }
        catch (Exception e)
        {
            await Log($"{e.Message}\n{e.StackTrace}", true);
            Environment.Exit(0);
        }
    }
}