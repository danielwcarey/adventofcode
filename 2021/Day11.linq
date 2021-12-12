<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day11
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 1637
//   Part2 := 242
//
async Task Main() {
    // sample data
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day11-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    //Part1(data);
    Part2(data);
}

void Part1(Data data) {
    if (data.Grid == null) return;

    var grid = new Grid(data.Grid);

    var flashCount = 0;
    var step = 100;
    while (step > 0) {
        // Increase by 1
        grid.ForEach((x, y, value) => grid[x, y] = value + 1);

        // Spread Flashes
        var flashing = new List<(int X, int Y)>(grid.GetValues().Where(value => value.Value == 10).Select(value => (value.X, value.Y)));
        flashCount += flashing.Count;
        do {
            List<(int X, int Y)> nextFlashing = new();
            foreach (var flash in flashing) {

                List<(int X, int Y)> testPositions = new() {
                    (flash.X - 1, flash.Y - 1),
                    (flash.X, flash.Y - 1),
                    (flash.X + 1, flash.Y - 1),
                    (flash.X - 1, flash.Y),
                    (flash.X, flash.Y),
                    (flash.X + 1, flash.Y),
                    (flash.X - 1, flash.Y + 1),
                    (flash.X, flash.Y + 1),
                    (flash.X + 1, flash.Y + 1),
                };

                foreach (var (testx, testy) in testPositions) {
                    if (grid[testx, testy] != int.MaxValue) {
                        grid[testx, testy] += 1;

                        if (grid[testx, testy] == 10) {
                            nextFlashing.Add((testx, testy));
                        }
                    }
                }
            }
            flashing = nextFlashing;
            flashCount += flashing.Count;
        } while (flashing.Count > 0);

        // Discharge flashing
        grid.ForEach((x, y, value) => { if (value > 9) grid[x, y] = 0; });
        //grid.Dump(step.ToString(flashCount.ToString()));
        step--;
    }
    flashCount.Dump("Part1");
}

void Part2(Data data) {
    if (data.Grid == null) return;

    var grid = new Grid(data.Grid);

    var flashCount = 0;
    var step = 1;
    while (true) {
        // Increase by 1
        grid.ForEach((x, y, value) => grid[x, y] = value + 1);

        // Spread Flashes
        var flashing = new List<(int X, int Y)>(grid.GetValues().Where(value => value.Value == 10).Select(value => (value.X, value.Y)));
        flashCount += flashing.Count;
        do {
            List<(int X, int Y)> nextFlashing = new();
            foreach (var flash in flashing) {

                List<(int X, int Y)> testPositions = new() {
                    (flash.X - 1, flash.Y - 1),
                    (flash.X, flash.Y - 1),
                    (flash.X + 1, flash.Y - 1),
                    (flash.X - 1, flash.Y),
                    (flash.X, flash.Y),
                    (flash.X + 1, flash.Y),
                    (flash.X - 1, flash.Y + 1),
                    (flash.X, flash.Y + 1),
                    (flash.X + 1, flash.Y + 1),
                };

                foreach (var (testx, testy) in testPositions) {
                    if (grid[testx, testy] != int.MaxValue) {
                        grid[testx, testy] += 1;

                        if (grid[testx, testy] == 10) {
                            nextFlashing.Add((testx, testy));
                        }
                    }
                }
            }
            flashing = nextFlashing;
            flashCount += flashing.Count;
        } while (flashing.Count > 0);

        // Discharge flashing
        grid.ForEach((x, y, value) => { if (value > 9) grid[x, y] = 0; });
        //grid.Dump(step.ToString(flashCount.ToString()));

        if (grid.GetValues().All(v => v.Value == 0)) break;

        step++;
    }
    step.Dump("Part2");
}

partial class Grid {
    public Grid(int width, int height) => (Width, Height, Map) = (width, height, new int[height, width]);
    public Grid(Grid copyGrid) {
        (Width, Height, Map) = (copyGrid.Width, copyGrid.Height, new int[copyGrid.Height, copyGrid.Width]);
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                this[x, y] = copyGrid[x, y];
            }
        }
    }

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

partial class Grid {
    // x, y, value
    public void ForEach(Action<int, int, int> action) {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                var value = this[x, y];
                action(x, y, value);
            }
        }
    }

    public List<(int X, int Y, int Value)> GetValues() {
        var result = new List<(int X, int Y, int Value)>();
        for (var x = 0; x < this.Width; x++) {
            for (var y = 0; y < this.Height; y++) {
                result.Add((x, y, this[x, y]));
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