using System;
using System.Collections.Generic;
using System.Linq;

public class ECSMatcher
{
    public static int _matcherId = 0;

    public int _mid = _matcherId++;

    private List<ECSRule> _rules = new List<ECSRule>();

    public ECSMatcher AnyOf(params Type[] comps) {
        _rules.Add(new ECSRule(ECSRuleType.AnyOf, comps));
        return this;
    }

    public ECSMatcher AllOf(params Type[] comps)
    {
        _rules.Add(new ECSRule(ECSRuleType.AllOf, comps));
        return this;
    }

    public ECSMatcher ExcludeOf(params Type[] comps)
    {
        _rules.Add(new ECSRule(ECSRuleType.ExcludeOf, comps));
        return this;
    }

    public ECSMatcher OnlyOf(params Type[] comps)
    {
        _rules.Add(new ECSRule(ECSRuleType.AllOf, comps));
        List<Type> compList = new List<Type>();
        foreach (var comp in comps)
        {
            if (!comps.Contains(comp))
            {
                compList.Add(comp);
            }
        }
        Type[] compArray = compList.ToArray();
        _rules.Add(new ECSRule(ECSRuleType.ExcludeOf,compArray));
        return this;
    }

    public bool IsMatch(Entity entity)
    {
        foreach (var rule in _rules)
        {
            if (!rule.isMatch(entity))
            {
                return false;
            }
        }
        return true;
    }

    public List<int> GetCompIds()
    {
        List<int> result = null;

        foreach (var rule in _rules)
        {
            List<int> ids = rule.GetCompIds();
            if(result == null)
            {
                result = ids;
            }
            else
            {
                result.AddRange(ids);
            }
        }
        return result;
    }
}
