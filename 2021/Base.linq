<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Base
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
	//string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day0X-input.txt");
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

#region Grid<TCell>
partial class Grid<TCell> {
	public Grid(int width, int height) => (Width, Height, Map) = (width, height, new TCell[height, width]);
	public Grid(Grid<TCell> copyGrid) {
		(Width, Height, Map) = (copyGrid.Width, copyGrid.Height, new TCell[copyGrid.Height, copyGrid.Width]);
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				this[x, y] = copyGrid[x, y];
			}
		}
	}

	public int Width { get; private set; }
	public int Height { get; private set; }

	public TCell[,] Map { get; private set; }

	// reverse for linqpad dump
	public TCell this[int x, int y]
	{
		get => Map[y, x];
		set => Map[y, x] = value;
	}
}

partial class Grid<TCell> {
	// x, y, value
	public void ForEach(Action<int, int, TCell> action) {
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				var value = this[x, y];
				action(x, y, value);
			}
		}
	}

	public List<(int X, int Y, TCell Value)> GetValues() {
		var result = new List<(int X, int Y, TCell Value)>();
		for (var x = 0; x < this.Width; x++) {
			for (var y = 0; y < this.Height; y++) {
				result.Add((x, y, this[x, y]));
			}
		}
		return result;
	}
}
#endregion

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
@"
";
#endregion