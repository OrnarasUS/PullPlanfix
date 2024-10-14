namespace PullPlanfix;

public record struct PlanfixResponse(PlanfixTask? Task);
public struct PlanfixTask
{
    public int Id;
    public string? Name;
    public PlanfixEntity? Status;
    public PlanfixId? Template;
    public PlanfixEntity? Counterparty;
    public PlanfixDateTime? DateTime;
    public PlanfixDateTime? EndDateTime;
    public PlanfixAssignees? Assignees;
    public PlanfixEntity? Assigner;
    public PlanfixField?[]? CustomFieldData;
}
public record struct PlanfixDateTime(DateTime DateTime)
{
    public static implicit operator string?(PlanfixDateTime? src) =>
        (src?.DateTime)?.ToString("yyyy-MM-dd HH:mm");
}
public record struct PlanfixId(int Id);
public record struct PlanfixField(PlanfixId Field, object Value);
public record struct PlanfixEntity(string Name);
public record struct PlanfixAssignees(PlanfixEntity[] Users, PlanfixEntity[] Groups);