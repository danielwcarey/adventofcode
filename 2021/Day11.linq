<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day11
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
	var data = await Data.ParseAsync(new StringReader(sample));

	// data
	//string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day11-input.txt");
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

partial class Grid {
    public Grid(int width, int height) => (Width, Height, Map) = (width, height, new int[height, width]);

    public int Width { get; private set; }
    public int Height { get; private set; }

    public int[,] Map { get; private set; }

    // reverse for linqpad dump
    public int this[int x, int y]
    {
        get => (y, x) switch {
            _ when x < 0 => int.MaxValue,
            _ when x >= Width => int.MaxValue,
            _ when y < 0 => int.MaxValue,
            _ when y >= Height => int.MaxValue,
            _ => Map[y, x],
        };
        set => Map[y, x] = value;
    }
}

partial class Data {
    public Grid? Grid { get; set; }
}

partial class Data {
    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();

        List<string> lines = new();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            lines.Add(line);

        } while (true);

        result.Grid = new Grid(lines[0].Length, lines.Count);
        
        var y = 0;
        foreach (var line in lines) {
            for (var x = 0; x < line.Length; x++) {
                var value = int.Parse(line[x].ToString());
                result.Grid[x, y] = value;
            }
            y++;
        }
        return result;
    }
}

#region Sample
string sample =
@"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526
";
#endregion