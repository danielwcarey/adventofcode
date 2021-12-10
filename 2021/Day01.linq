<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day01
//
// Links
//   https://adventofcode.com/2021/day/1
//
// Answer
//   Part1 := 1754
//   Part2 := 1789
//
async Task Main() {

    // sample data
    //var data = sample.Split("\n").Select(str => int.Parse(str)).ToArray();

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day01-input.txt");
    var data = (await File.ReadAllLinesAsync(path)).Select(str => int.Parse(str)).ToArray();

    Part1(data);
    Part2(data);
}

// ( Depth, Count )
void Part1(int[] data) {
    int countDepthIncrease = 0;
    for (int index = 1; index < data.Length; index++) {
        countDepthIncrease += data[index - 1] <= data[index] ? 1 : 0;
    }

    countDepthIncrease.Dump("Part1");
}

// ( Depth, Count, Index )
void Part2(int[] data) {
    int countDepthIncrease = 0;
    for (int index = 0; index < data.Length - 3; index++) {
        var x = data[index..(index + 3)].Sum();
        var y = data[(index + 1)..(index + 4)].Sum();
        var z = x < y ? 1 : 0;        
        countDepthIncrease += z;
        //$"{x}, {y} -> {z}".Dump();
    }
    countDepthIncrease.Dump("Part2");
}


#region Sample
string sample =
@"199
200
208
210
200
207
240
269
260
263";
#endregion