

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Xml.XPath;

/*
foreach (var group in result1.GroupBy(self => self.StartUp))
{
    Console.WriteLine($"{group.Key.Name} needs {group.Key.MoneyNeeded:C}");
    foreach (var item in group)
    {
        Console.WriteLine($"   Founded by {item.Funder.Name} with {item.Money:C} from the original {item.Funder.MoneyAvailable:C}");
    }
    Console.WriteLine();
}
 * */

var summary = BenchmarkRunner.Run<Performance>();

public class Performance
{
    private readonly FundingSystem _fundingSystem;

    public Performance()
    {
        List<StartUp> _startUps = [
        new StartUp("Orange", 10_000),
        new StartUp("Apple", 17_000),
        new StartUp("Watermelon", 80_000),
        new StartUp("Banana", 45_000),
        new StartUp("Pineapple", 12_000),
    ];

        List<Funder> _funders = [
            new Funder("blue", 25_000),
        new Funder("yellow", 53_000),
        new Funder("purple", 39_000),
        new Funder("red", 50_000),
    ];

        _fundingSystem = new FundingSystem(_startUps, _funders);
    }


    [Benchmark]
    public void CalculateFundersForEach() => _fundingSystem.CalculateFundersForEach();

    [Benchmark]
    public void CalculateFundersRecursion() => _fundingSystem.CalculateFundersRecursion();
}




