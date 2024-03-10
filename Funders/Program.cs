

using System.Xml.XPath;

List<StartUp> startUps = [
    new StartUp("Orange", 10_000),
    new StartUp("Apple", 17_000),
    new StartUp("Watermelon", 80_000),
    new StartUp("Banana", 45_000),
    new StartUp("Pineapple", 12_000),
    ];

List<Funder> funders = [
    new Funder("blue", 25_000),
    new Funder("yellow", 53_000),
    new Funder("purple", 39_000),
    new Funder("red", 50_000),
];

var result = CalculateFundersForEach(startUps, funders);

foreach (var group in result.GroupBy(self => self.StartUp))
{
    Console.WriteLine($"{group.Key.Name} needs {group.Key.MoneyNeeded:C}");
    foreach (var item in group)
    {
        Console.WriteLine($"   Founded by {item.Funder.Name} with {item.Money:C} from the original {item.Funder.MoneyAvailable:C}");
    }
    Console.WriteLine();
}

IReadOnlyCollection<Funding> CalculateFundersForEach(IEnumerable<StartUp> startUps, IEnumerable<Funder> funders)
{
    var result = new List<Funding>();

    using var fundersEnumerable = funders.GetEnumerator();
    var funderCurrent = default(Funder)!;
    var moneyStillAvailable = decimal.Zero;

    foreach (var startUp in startUps)
    {
        var moneyStillNeeded = startUp.MoneyNeeded;
        while (moneyStillNeeded > 0)
        {
            if (moneyStillAvailable == 0)
            {
                if (!fundersEnumerable.MoveNext())
                {
                    break;
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


public record StartUp(string Name, decimal MoneyNeeded);
public record Funder(string Name, decimal MoneyAvailable);
public record Funding(StartUp StartUp, Funder Funder, decimal Money);
