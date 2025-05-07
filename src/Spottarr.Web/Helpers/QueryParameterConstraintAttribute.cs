using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Spottarr.Web.Helpers;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class QueryParameterConstraintAttribute : Attribute, IActionConstraint
{
    // Ensure this constraint runs early
    public int Order => 0;
    public string Parameter { get; }
    public string[] Values { get; }

    public QueryParameterConstraintAttribute(string parameter, params string[] values)
    {
        Parameter = parameter;
        Values = values;
    }

    public bool Accept(ActionConstraintContext context)
    {
        var query = context.RouteContext.HttpContext.Request.Query;
        return query.TryGetValue(Parameter, out var actualValues)
               && actualValues.Intersect(Values).Any();
    }
}