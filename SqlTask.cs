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

    public SqlTask(PlanfixTask pf)
    {
        id = $"{pf.Id}";
        name = pf.Name;
        status = pf.Status?.Name;
        template = pf.Template is null ? null : Program.Templates[pf.Template.Value.Id];
        service = (string?)pf.CustomFieldData?.First(i => i?.Field.Id == 114278)?.Value;
        partner = pf.Counterparty?.Name;
        if (pf.Assignees is PlanfixAssignees assigs)
        {
            PlanfixEntity[] execs = assigs.Groups.Union(assigs.Users).ToArray();
            executors = execs.Length > 0 ? string.Join(", ", execs.Select(i=>i.Name)) : null;
        }
        mark = pf.CustomFieldData?.First(i => i?.Field.Id == 114352)?.Value?.ToString();
        created = (pf.DateTime?.DateTime)?.ToString("yyyy-MM-dd HH:mm");
        accepted = (PlanfixDateTime?)pf.CustomFieldData?.First(i => i?.Field.Id == 114320)?.Value;
        started = (PlanfixDateTime?)pf.CustomFieldData?.First(i => i?.Field.Id == 114286)?.Value;
        billsended = (PlanfixDateTime?)pf.CustomFieldData?.First(i => i?.Field.Id == 114394)?.Value;
        planended = pf.EndDateTime;
        ended = (PlanfixDateTime?)pf.CustomFieldData?.First(i => i?.Field.Id == 114280)?.Value;
        retailended = (PlanfixDateTime?)pf.CustomFieldData?.First(i => i?.Field.Id == 114424)?.Value;
        owner = pf.Assigner?.Name;
    }

}
