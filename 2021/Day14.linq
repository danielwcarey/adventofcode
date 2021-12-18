<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day14
//
// Links
//   https://adventofcode.com/2021/day/14
//
// Answer
//   Part1 := 2447
//   Part2 := 3018019237563
//
using System.Numerics;
using System.Collections.Concurrent;

async Task Main() {
    // sample data
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day14-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    Part1(data, 10, "Part1");
    Part2(data, 40, "Part2");

}

class PolymerCount : Dictionary<string, BigInteger> {
    public PolymerCount() { }
    public PolymerCount(PolymerCount polymerCount) : base(polymerCount) { }
    public PolymerCount(List<string> list) {
        foreach (var ch in list) {
            Add(ch.ToString(), 0);
        }
    }
}
class PolymerPairLookup : Dictionary<string, string> {
    public PolymerPairLookup() { }
}

class PolymerPairCountLookup : Dictionary<string, BigInteger> {
    public PolymerPairCountLookup(PolymerPairLookup polymerPairLookup) {
        foreach (var key in polymerPairLookup.Keys.Distinct()) {
            this.TryAdd(key, 0);
        }
    }

    public void Add(string list) {
        for (var x = 0; x < list.Count() - 1; x++) {
            var key = string.Join("", list[x], list[x + 1]);
            this[key] += 1;
        }
        this[list[^1..]] = 1;
    }
}

void Part1(Data data, int steps, string title) {
    var step = 0;

    var polymerCount = new PolymerCount(data.PolymerList);

    // Initialize polymerCount from template
    foreach (var ch in data.PolymerTemplate) {
        polymerCount[ch.ToString()]++;
    }

    var polymerPairCountLookup = new PolymerPairCountLookup(data.PolymerPairLookup);

    // Initialize polymerPairCount
    polymerPairCountLookup.Add(data.PolymerTemplate);
    //polymerPairCountLookup.Dump($"polymerPairCountLookup: {step}");

    while (step < steps) {
        // Make a list of changes being made to the Polymer Pairs [add, subtract]
        var addPolymerPairList = new Dictionary<string, BigInteger>();
        var subtractPolymerPairList = new Dictionary<string, BigInteger>();

        // For all the Polymer Pairs, update new Polymers' count and update pair changes[add, subtract]
        foreach (var polymerPairCountKey in polymerPairCountLookup.Keys) {
            if (polymerPairCountKey == null || polymerPairCountKey.Length != 2) continue;
            var count = polymerPairCountLookup[polymerPairCountKey];
            var nextPolymer = data.PolymerPairLookup[polymerPairCountKey];

            polymerCount[nextPolymer.ToString()] += count;

            var pair1 = string.Join("", polymerPairCountKey[0], nextPolymer);
            var pair2 = string.Join("", nextPolymer, polymerPairCountKey[1]);

            if (!subtractPolymerPairList.ContainsKey(polymerPairCountKey)) subtractPolymerPairList.Add(polymerPairCountKey, 0);
            if (!addPolymerPairList.ContainsKey(pair1)) addPolymerPairList.Add(pair1, 0);
            if (!addPolymerPairList.ContainsKey(pair2)) addPolymerPairList.Add(pair2, 0);

            subtractPolymerPairList[polymerPairCountKey] = count;
            addPolymerPairList[pair1] += count;
            addPolymerPairList[pair2] += count;
        }
        foreach (var key in addPolymerPairList.Keys) {
            polymerPairCountLookup[key] += addPolymerPairList[key];
        }
        foreach (var key in subtractPolymerPairList.Keys) {
            polymerPairCountLookup[key] -= subtractPolymerPairList[key];
        }
        step++;
    }
    var min = polymerCount.Min(p => p.Value);
    var max = polymerCount.Max(p => p.Value);
    var value = max - min;

    value.Dump(title);
    polymerCount.Dump($"polymerCount: {step}");

}

void Part2(Data data, int steps, string title) => Part1(data, steps, title);

partial class Data {
    public string PolymerTemplate { get; set; } = "";
    public List<string> PolymerList { get; set; } = new();
    public PolymerPairLookup PolymerPairLookup { get; set; } = new();
}

partial class Data {
    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();

        result.PolymerTemplate = await reader.ReadLineAsync() ?? "";
        await reader.ReadLineAsync();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            var match = line.Split(" -> ");
            result.PolymerPairLookup.Add(match[0], match[1]);
        } while (true);

        result.PolymerList.AddRange(
            result.PolymerPairLookup.Keys.SelectMany(key => new string[] { key[0].ToString(), key[1].ToString() }).Distinct()
        );
        return result;
    }
}

#region Sample
string sample =
@"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C
";
#endregion
