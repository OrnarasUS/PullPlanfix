namespace PullPlanfix;

public struct SqlTask
{
    public string id;
    public string? name;
    public string? status;
    public string? template;
    public string? service;
    public string? partner;
    public string? executors;
    public string? mark;
    public string? created;
    public string? accepted;
    public string? started;
    public string? billsended;
    public string? planended;
    public string? ended;
    public string? retailended;
    public string? owner;
    public string? planendedonly;

    public SqlTask(PlanfixTask pf)
    {
        id = $"{pf.Id}";
        name = pf.Name;
        status = pf.Status?.Name;
        template = pf.Template is null ? null : Program.Templates[pf.Template.Value.Id];
        partner = pf.Counterparty?.Name;
        created = (pf.DateTime?.DateTime)?.ToString("yyyy-MM-dd HH:mm");
        planended = pf.EndDateTime;
        owner = pf.Assigner?.Name;

        if (pf.Assignees is PlanfixAssignees assigs)
        {
            PlanfixEntity[] execs = assigs.Groups.Union(assigs.Users).ToArray();
            executors = execs.Length > 0 ? string.Join(", ", execs.Select(i => i.Name)) : null;
        }

        mark = ((int?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114352, null)?.Value).ToString();
        accepted = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114320, null)?.Value;
        started = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114286, null)?.Value;
        billsended = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114394, null)?.Value;
        ended = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114280, null)?.Value;
        retailended = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114424, null)?.Value;
        service = (string?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114278, null)?.Value;
        planendedonly = (PlanfixDateTime?)pf.CustomFieldData?.FirstOrDefault(i => i?.Field.Id == 114288, null)?.Value;
    }

}
