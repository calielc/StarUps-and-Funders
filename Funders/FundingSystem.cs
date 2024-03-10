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
        var result = new List<Funding>();

        using var fundersEnumerable = _funders.GetEnumerator();
        var funderCurrent = default(Funder)!;
        var moneyStillAvailable = decimal.Zero;

        foreach (var startUp in _startUps)
        {
            var moneyStillNeeded = startUp.MoneyNeeded;
            while (moneyStillNeeded > 0)
            {
                if (moneyStillAvailable == 0)
                {
                    if (!fundersEnumerable.MoveNext())
                    {
                        throw new InvalidOperationException("No more funders");
                    }

                    funderCurrent = fundersEnumerable.Current;
                    moneyStillAvailable = funderCurrent.MoneyAvailable;
                }

                if (moneyStillAvailable >= moneyStillNeeded)
                {
                    result.Add(new Funding(startUp, funderCurrent, moneyStillNeeded));
                    moneyStillAvailable -= moneyStillNeeded;
                    moneyStillNeeded = 0;
                }
                else
                {
                    result.Add(new Funding(startUp, funderCurrent, moneyStillAvailable));
                    moneyStillNeeded -= moneyStillAvailable;
                    moneyStillAvailable = 0;
                }
            }
        }

        return result;
    }

    public IReadOnlyCollection<Funding> CalculateFundersRecursion()
    {
        var result = new List<Funding>();

        using var fundersEnumerable = _funders.GetEnumerator();
        var moneyStillAvailable = decimal.Zero;

        foreach (var startUp in _startUps)
        {
            CalculateStarUp(startUp, startUp.MoneyNeeded);
        }

        return result;

        void CalculateStarUp(StartUp startUp, decimal moneyStillNeeded)
        {
            if (moneyStillAvailable == 0)
            {
                if (!fundersEnumerable.MoveNext())
                {
                    throw new InvalidOperationException("No more funders");
                }
                moneyStillAvailable = fundersEnumerable.Current.MoneyAvailable;
            }

            if (moneyStillAvailable >= moneyStillNeeded)
            {
                result.Add(new Funding(startUp, fundersEnumerable.Current, moneyStillNeeded));
                moneyStillAvailable -= moneyStillNeeded;
            }
            else
            {
                result.Add(new Funding(startUp, fundersEnumerable.Current, moneyStillAvailable));
                moneyStillNeeded -= moneyStillAvailable;
                moneyStillAvailable = 0;

                CalculateStarUp(startUp, moneyStillNeeded);
            }
        }
    }
}

public record StartUp(string Name, decimal MoneyNeeded);
public record Funder(string Name, decimal MoneyAvailable);
public record Funding(StartUp StartUp, Funder Funder, decimal Money);
