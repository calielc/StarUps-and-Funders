public class FundingSystem
{
    private IReadOnlyCollection<StartUp> _startUps;
    private IReadOnlyCollection<Funder> _funders;

    public FundingSystem(IReadOnlyCollection<StartUp> startUps, IReadOnlyCollection<Funder> funders)
    {
        _startUps = startUps;
        _funders = funders;
    }

    public IReadOnlyCollection<Funding> CalculateFundersForEach()
    {
        var result = new List<Funding>(_startUps.Count);

        using var fundersEnumerable = _funders.GetEnumerator();
        var funderCurrent = default(Funder)!;
        var moneyAvailable = decimal.Zero;

        foreach (var startUp in _startUps)
        {
            var moneyNeeded = startUp.MoneyNeeded;
            while (moneyNeeded > 0)
            {
                if (moneyAvailable == 0)
                {
                    if (!fundersEnumerable.MoveNext())
                    {
                        throw new InvalidOperationException("No more funders");
                    }

                    funderCurrent = fundersEnumerable.Current;
                    moneyAvailable = funderCurrent.MoneyAvailable;
                }

                if (moneyAvailable >= moneyNeeded)
                {
                    result.Add(new Funding(startUp, funderCurrent, moneyNeeded));
                    moneyAvailable -= moneyNeeded;
                    moneyNeeded = 0;
                }
                else
                {
                    result.Add(new Funding(startUp, funderCurrent, moneyAvailable));
                    moneyNeeded -= moneyAvailable;
                    moneyAvailable = 0;
                }
            }
        }

        return result;
    }

    public IReadOnlyCollection<Funding> CalculateFundersRecursion()
    {
        var result = new List<Funding>(_startUps.Count);

        using var fundersEnumerable = _funders.GetEnumerator();
        var moneyAvailable = decimal.Zero;

        foreach (var startUp in _startUps)
        {
            CalculateStarUp(startUp, startUp.MoneyNeeded);
        }

        return result;

        void CalculateStarUp(StartUp startUp, decimal moneyNeeded)
        {
            if (moneyAvailable == 0)
            {
                if (!fundersEnumerable.MoveNext())
                {
                    throw new InvalidOperationException("No more funders");
                }
                moneyAvailable = fundersEnumerable.Current.MoneyAvailable;
            }

            if (moneyAvailable >= moneyNeeded)
            {
                result.Add(new Funding(startUp, fundersEnumerable.Current, moneyNeeded));
                moneyAvailable -= moneyNeeded;
            }
            else
            {
                result.Add(new Funding(startUp, fundersEnumerable.Current, moneyAvailable));
                moneyNeeded -= moneyAvailable;
                moneyAvailable = 0;

                CalculateStarUp(startUp, moneyNeeded);
            }
        }
    }
}

public record StartUp(string Name, decimal MoneyNeeded);
public record Funder(string Name, decimal MoneyAvailable);
public record Funding(StartUp StartUp, Funder Funder, decimal Money);
