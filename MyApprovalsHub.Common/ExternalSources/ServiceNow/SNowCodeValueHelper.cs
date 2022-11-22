namespace MyApprovalsHub.Common.ExternalSources.ServiceNow;

public class SNowCodeValueHelper
{

    private Dictionary<string, string> _impact = new();

    private Dictionary<string, string> _priority = new();

    private Dictionary<string, string> _urgency = new();

    private Dictionary<string, string> _state = new();

    public Dictionary<string, string> Impact
    {
        get
        {
            return _impact;
        }        
    }

    public Dictionary<string, string> Priority
    {
        get
        {
            return _priority;
        }
    }

    public Dictionary<string, string> Urgency
    {
        get
        {
            return _urgency;
        }
    }

    public Dictionary<string, string> State
    {
        get
        {
            return _state;
        }
    }

    public SNowCodeValueHelper()
    {
        PopulateImpacts();

        PopulatePriorities();

        PopulateUrgency();

        PopulateState();
    }

    private void PopulateImpacts()
    {
       
        _impact.Add("1", "High");
        _impact.Add("2", "Medium");
        _impact.Add("3", "Low");

    }

    private void PopulatePriorities()
    {
        _priority.Add("1", "Critical");
        _priority.Add("2", "High");
        _priority.Add("3", "Moderate");
        _priority.Add("4", "Low");

    }

    private void PopulateUrgency()
    {
        _urgency.Add("1", "High");
        _urgency.Add("2", "Medium");
        _urgency.Add("3", "Low");
    }

    private void PopulateState()
    {
        _state.Add("1", "New");
        _state.Add("2", "In Progress");
        _state.Add("3", "On Hold");
        _state.Add("4", "On Resolved");
        _state.Add("5", "Closed");
        _state.Add("6", "Canceled");
    }
}
