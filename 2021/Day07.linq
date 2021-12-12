<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day07
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 341558
//   Part2 := 93214037
//
using static System.Math;

async Task Main() {
    // sample data, 37, 168
    //var data = await Data.ParseAsync(new StringReader(sample));// sample.Split("\n").Select(str => int.Parse(str)).ToArray();

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day07-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    Part1(data);
    Part2(data);
}

int GetBestFuelCost(Data data, Func<int, int> computeFuelCost) {
    var bestPosition = int.MaxValue;
    var bestFuel = int.MaxValue;

    for (int position = data.CrabPositions.Min(); position < data.CrabPositions.Max(); position++) {
        var testFuelCost = computeFuelCost(position);
        if (testFuelCost < bestFuel) {
            bestFuel = testFuelCost;
            bestPosition = position;
        }
    }
    return bestFuel;
}

void Part1(Data data) => GetBestFuelCost(data, data.ComputeFuelCost).Dump("Part1");

void Part2(Data data) => GetBestFuelCost(data, data.ComputeIncrementalFuelCost).Dump("Part2");

partial class Data {
    public List<int> CrabPositions { get; set; } = new();
}

partial class Data {

    public int ComputeFuelCost(int position) => this.CrabPositions.Sum(x => Abs(x - position));

    public int f(int x) => x switch {
        _ when x > 1 => x + f(x - 1),
        _ => x
    };
    public int ComputeIncrementalFuelCost(int position) => this.CrabPositions.Sum(x => f(Abs(x - position)));

    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();
        var input = await reader.ReadLineAsync();
        if (input == null) return result;

        result.CrabPositions.AddRange(input.Split(",").Select(s => int.Parse(s)));
        return result;
    }
}

#region Sample
string sample =
@"16,1,2,0,4,2,7,1,2,14";
#endregion