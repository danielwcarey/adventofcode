<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day13
//
// Links
//   https://adventofcode.com/2021/day/13
//
// Answer
//   Part1 := 781
//   Part2 := PERCGJPB
//
async Task Main() {
	// sample data
	//var data = await Data.ParseAsync(new StringReader(sample));

	// data
	string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day13-input.txt");
	var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

	//Part1(data, 1);
	Part2(data, int.MaxValue);
}

void Part2(Data data, int foldCount) => Part1(data, foldCount, "Part2");

void Part1(Data data, int foldCount, string text = "Part1") {
	foreach (var fold in data.FoldList) {
		if (foldCount == 0) break;
		var newPointList = new List<(int X, int Y)>();
		if (fold.Axis == "x") {
			newPointList.AddRange(data.PointList.Where(p => p.X <= fold.Position));
			newPointList.AddRange(data.PointList.Where(p => p.X > fold.Position)
				.Select(p => (fold.Position - (p.X - fold.Position), p.Y)));
		} else if (fold.Axis == "y") {
			newPointList.AddRange(data.PointList.Where(p => p.Y <= fold.Position));
			newPointList.AddRange(data.PointList.Where(p => p.Y > fold.Position)
				.Select(p => (p.X, fold.Position - (p.Y - fold.Position))));
		}
		data.PointList = newPointList;
		foldCount--;
	}
	var dots = new HashSet<(int X, int Y)>();
	foreach (var item in data.PointList) dots.Add((item.X, item.Y));
	dots.Count.Dump(text);
	data.Draw();
}

partial class Data {
	public List<(int X, int Y)> PointList { get; set; } = new();
	public List<(string Axis, int Position)> FoldList { get; set; } = new();
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

	public void Draw(string text = "") {
		var maxX = PointList.Max(p => p.X);
		var maxY = PointList.Max(p => p.Y);
		var grid = new Grid<char>(maxX + 1, maxY + 1);
		grid.ForEach((x, y, _) => grid[x, y] = PointList.Contains((x, y)) ? '█' : '.');
		if (text.Length > 0) grid.Dump(text); else grid.Dump();
	}

	public static async Task<Data> ParseAsync(TextReader reader) {
		var result = new Data();

		var state = 0;
		do {
			var line = await reader.ReadLineAsync();
			if (line == null) break;

			if (string.IsNullOrWhiteSpace(line)) {
				state++;
				continue;
			}

			switch (state) {
				case 0:
					var points = line.Split(",");
					result.PointList.Add((X: int.Parse(points[0]), Y: int.Parse(points[1])));
					break;
				case 1:
					var command = line.Split("=");
					result.FoldList.Add((Axis: command[0][^1..], Position: int.Parse(command[1])));
					break;
			}
		} while (true);
		return result;
	}
}

#region Sample
string sample =
@"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5
";
#endregion