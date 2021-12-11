<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day07
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 
//   Part2 := 
//
async Task Main() {
	// sample data
	var data = await Data.ParseAsync(new StringReader(sample));// sample.Split("\n").Select(str => int.Parse(str)).ToArray();

	// data
	//string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day07-input.txt");
	//var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

	Part1(data);
	//Part2(data);
}

void Part1(Data data) {
	data.Dump("Part1");
}

void Part2(Data data) {
	data.Dump("Part2");
}

partial class Data {
	
}

partial class Data {
	public static async Task<Data> ParseAsync(TextReader reader){
		var result = new Data();
		return result;
	}
}

#region Sample
string sample =
@"16,1,2,0,4,2,7,1,2,14";
#endregion