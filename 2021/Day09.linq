<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day09
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 496
//   Part2 := 902880
//
async Task Main() {
    // sample data
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day09-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    //Part1(data);
    Part2(data);
}

void Part1(Data data) {
    if (data.Grid == null) return;

    var riskLevel = data.Grid.GetValues().Where(v => v.IsLowPoint).Sum(v => v.Value + 1);

    riskLevel.Dump("Part1");
    data.Grid.Dump();
}

void Part2(Data data) {
    if (data.Grid == null) return;

    var basinLowPoints = data.Grid.GetValues().Where(v => v.IsLowPoint).ToList();
    List<List<(int, int)>> basins = new();

    foreach (var basinLowPoint in basinLowPoints) {
        var basin = GetBasinPoints(data, basinLowPoint.X, basinLowPoint.Y);
        basins.Add(basin);
    }
    
    var top3Basins = basins.OrderByDescending(basin => basin.Count).Take(3).Aggregate(1, (value, basin) => value * basin.Count);
    top3Basins.Dump("Part2");
}

List<(int, int)> GetBasinPoints(Data data, int x, int y) {

    List<(int, int)> visited = new();
    if (data.Grid == null) return visited;

    Stack<(int, int)> process = new();
    process.Push((x, y));

    while (process.Count > 0) {
        var (curx, cury) = process.Pop();
        if(!visited.Contains((curx, cury)))
            visited.Add((curx, cury));

        int testx = 0; int testy = 0;

        (testx, testy) = (curx, cury - 1);
        if (data.Grid[testx, testy] < 9 && !visited.Contains((testx, testy))) process.Push((testx, testy));

        (testx, testy) = (curx - 1, cury);
        if (data.Grid[testx, testy] < 9 && !visited.Contains((testx, testy))) process.Push((testx, testy));

        (testx, testy) = (curx + 1, cury);
        if (data.Grid[testx, testy] < 9 && !visited.Contains((testx, testy))) process.Push((testx, testy));

        (testx, testy) = (curx, cury + 1);
        if (data.Grid[testx, testy] < 9 && !visited.Contains((testx, testy))) process.Push((testx, testy));
    }

    return visited;
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
    public int Width { get; set; }
    public int Height { get; set; }
    public Grid? Grid { get; set; }
}

partial class Grid {
    public bool IsLowPoint(int x, int y) {

        var value = this[x, y];

        if (this[x - 1, y] <= value) return false;
        if (this[x + 1, y] <= value) return false;
        if (this[x, y - 1] <= value) return false;
        if (this[x, y + 1] <= value) return false;

        return true;
    }

    public List<(int X, int Y, int Value, bool IsLowPoint)> GetValues() {
        var result = new List<(int X, int Y, int Value, bool IsLowPoint)>();
        for (var x = 0; x < this.Width; x++) {
            for (var y = 0; y < this.Height; y++) {
                result.Add((x, y, this[x, y], IsLowPoint(x, y)));
            }
        }
        return result;
    }
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
@"2199943210
3987894921
9856789892
8767896789
9899965678";
#endregion