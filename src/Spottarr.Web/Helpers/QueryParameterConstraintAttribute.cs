using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Spottarr.Web.Helpers;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class QueryParameterConstraintAttribute : Attribute, IActionConstraint
{
    // Ensure this constraint runs early
    public int Order => 0;
    public string Parameter { get; }
    public string Value { get; }

    public QueryParameterConstraintAttribute(string parameter, string value)
    {
        Parameter = parameter;
        Value = value;
    }

    public bool Accept(ActionConstraintContext context)
    {
        var query = context.RouteContext.HttpContext.Request.Query;
        return query.TryGetValue(Parameter, out var actualValues)
               && Value.Equals(actualValues, StringComparison.OrdinalIgnoreCase);
    }
}