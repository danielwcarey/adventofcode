<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day06
//
// Links
//   https://adventofcode.com/2021/day/6
//
// Answer
//   Part1 := 366057
//   Part2 := 1653559299811
//
async Task Main() {
	// sample data
	//var data = await Data.ParseAsync(new StringReader(sample));// sample.Split("\n").Select(str => int.Parse(str)).ToArray();
	//data.CycleCount = 18; //26
	//data.CycleCount = 80; // 5934
	//data.CycleCount = 256; // 26984457539

	// data
	string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day06-input.txt");
	var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

	Part1(data, 80);
	Part2(data, 256);
}

long CycleCount(Data data, int cycleCount) {
	long[] cycle = new long[9];
	foreach (var item in data.LanternFishCycle) {
		cycle[item] += 1;
	}

	do {
		var nextCycle = new long[9];
		nextCycle[0] = cycle[1];
		nextCycle[1] = cycle[2];
		nextCycle[2] = cycle[3];
		nextCycle[3] = cycle[4];
		nextCycle[4] = cycle[5];
		nextCycle[5] = cycle[6];
		nextCycle[6] = cycle[7] + cycle[0];
		nextCycle[7] = cycle[8];
		nextCycle[8] = cycle[0];

		cycle = nextCycle;
		//string.Join(",", cycle).Dump();
	} while (--cycleCount > 0);
	return cycle.Sum(f => f);
}

void Part1(Data data, int cycleCount) => CycleCount(data, cycleCount).Dump("Part1");

void Part2(Data data, int cycleCount) => CycleCount(data, cycleCount).Dump("Part2");

partial class Data {
	public List<int> LanternFishCycle { get; set; } = new();
}

partial class Data {
	public static async Task<Data> ParseAsync(TextReader reader) {
		var result = new Data();
		var line = await reader.ReadLineAsync();
		if (line == null) return result;

		result.LanternFishCycle.AddRange(line.Split(",").Select(str => int.Parse(str)));
		return result;
	}
}

#region Sample
string sample =
@"3,4,3,1,2";
#endregion