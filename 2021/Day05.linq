<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Base
//
// Links
//   https://adventofcode.com/2021/day/5
//
// Answer
//   Part1 := 5092
//   Part2 := 20484
//
using static System.Math;

async Task Main() {

	// sample data
	//var data = await Data.ParseAsync(new StringReader(sample));//.Split("\n").Select(str => int.Parse(str)).ToArray();

	// data
	string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day05-input.txt");
	var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

	Part1(data);
	Part2(data);
}

Data ComputeLines(Data data, bool onlyHorizontalVertical = true) {
	
	var result = new Data(data);
	
	//int breakCount = 3;
	foreach (var lineSegment in result.LineSegments) {

		var (x1, y1, x2, y2) = lineSegment;
		if (onlyHorizontalVertical && !lineSegment.IsHorizontalOrVertical()) continue;

		int step = 0;
		var (x, y) = (x1, y1);
		List<(int, int)> visitedPoints = new();
		do {
			var dx = step * lineSegment.DX;
			var dy = step * lineSegment.DY;

			x = x1 + (int)Round(dx);
			y = y1 + (int)Round(dy);

			if (!visitedPoints.Any(p => p == (x, y))) {
				result[x, y] = result[x, y] + 1;
				visitedPoints.Add((x, y));
			}

			if (x == x2 && y == y2) break;
			step++;
		} while (true);
		//if (--breakCount == 0) break;
	}
	return result;
}

void Part1(Data data) {
	var newData = ComputeLines(data);
	var dangerPointsCount = newData.GetPointValues().Where(v => v >= 2).Count();
	dangerPointsCount.Dump("Part1");
}

void Part2(Data data) {
	var newData = ComputeLines(data, false);
	var dangerPointsCount = newData.GetPointValues().Where(v => v >= 2).Count();
	dangerPointsCount.Dump("Part2");
	//newData.Points.Dump();
	//newData.LineSegments.Dump();
}

partial class LineSegment {
	public int X1 { get; set; }
	public int Y1 { get; set; }
	public int X2 { get; set; }
	public int Y2 { get; set; }
	public double DX { get; set; }
	public double DY { get; set; }
	public double Length { get; set; }
}

partial class Data {
	public List<LineSegment> LineSegments { get; set; } = new();
	public int Width { get; set; }
	public int Height { get; set; }
	public int[,] Points { get; set; } = new int[0, 0];
}

partial class Data {

	public Data() {}
	public Data(Data source) {
		this.Width = source.Width;
		this.Height = source.Height;
		this.LineSegments = new List<UserQuery.LineSegment>(source.LineSegments);
		this.Points = new int[Width, Height];
		for(var x = 0; x < Width;x ++){
			for(var y = 0; y < Height;y++){
				this.Points[x,y] = source[x,y];
			}
		}
	}

	public int this[int x, int y]
	{
		// reverse so linqpad dump matches ui sample
		get => Points[y, x];
		set => Points[y, x] = value;
	}

	public List<int> GetPointValues() {
		var result = new List<int>();
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				result.Add(this[x, y]);
			}
		}
		return result;
	}

	public static async Task<Data> ParseAsync(TextReader reader) {
		var result = new Data();

		do {
			var line = await reader.ReadLineAsync();
			if (line == null) break;

			var input = LineSegment.Parse(line);
			if (input != null) result.LineSegments.Add(input);

		} while (true);

		result.Width = Max(result.LineSegments.Max(l => l.X1), result.LineSegments.Max(l => l.X2)) + 1;
		result.Height = Max(result.LineSegments.Max(l => l.Y1), result.LineSegments.Max(l => l.Y2)) + 1;
		result.Points = new int[result.Width, result.Height];

		return result;
	}
}

partial class LineSegment {

	public void Deconstruct(out int x1, out int y1, out int x2, out int y2) {
		x1 = X1;
		y1 = Y1;
		x2 = X2;
		y2 = Y2;
	}

	public bool IsHorizontalOrVertical() => (X1 == X2) || (Y1 == Y2);

	public static LineSegment? Parse(string line) {
		var result = new LineSegment();
		const string pattern = @"^(?<x1>\d+),(?<y1>\d+)\s*->\s*(?<x2>\d+),(?<y2>\d+)$";

		var match = Regex.Match(line.Trim(), pattern);
		if (!match.Success) return null;

		result.X1 = int.Parse(match.Groups["x1"].Value);
		result.Y1 = int.Parse(match.Groups["y1"].Value);
		result.X2 = int.Parse(match.Groups["x2"].Value);
		result.Y2 = int.Parse(match.Groups["y2"].Value);

		double a = (result.Y2 - result.Y1);
		double b = (result.X2 - result.X1);
		double a2 = a * a;
		double b2 = b * b;

		result.Length = Sqrt(a2 + b2);

		var dx = b / result.Length;
		var dy = a / result.Length;

		result.DX = dx;
		result.DY = dy;
		
		return result;
	}
}

#region Sample
string sample =
@"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2";
#endregion