<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day15
//
// Links
//   https://adventofcode.com/2021/day/15
//
// Notes
//   [Pathfinding Algorithms in C#]( https://www.codeproject.com/Articles/1221034/Pathfinding-Algorithms-in-Csharp )
//   [The Basics of A* for Path Planning](Game Programming Gems pg 254)
//
// Answer
//   Part1 := 685
//   Part2 := 2995
//
using static System.Math;

async Task Main() {
    Util.RawHtml("<style> body, * { font-family: 'consolas'; } </style>").Dump();

    // sample data
    var data = await Data.ParseAsync(new StringReader(sample));

    // data
    //string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day15-input.txt");
    //var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    //Part1(data, "Part1");
    Part2(data, "Part2", 5);
}

record GridSolution(bool FoundPath, int TotalRisk, long NodeVisits, List<GridCell> Path);

void DrawPath(Grid<GridCell> grid, List<GridCell> path) {
    grid.DumpGrid(gridCell => {
        var (x, y) = gridCell;
        if (path.Any(p => p.X == x && p.Y == y)) {
            return gridCell.RiskLevel.ToString();
        } else {
            return "-";
        }
    });
}

void PrintGridSolution(string title, GridSolution gridSolution, Grid<GridCell> grid) {
    var totalRisk = gridSolution.Path.Sum(p => p.RiskLevel);
    totalRisk.Dump($"{title} - Node Visits: {gridSolution.NodeVisits}");

    if (gridSolution.FoundPath)
        DrawPath(grid, gridSolution.Path);
}

GridSolution SolveGrid(GridCell start, GridCell end, Grid<GridCell> grid) {
    var openStates = new HashSet<GridCell>();
    var closedStates = new HashSet<GridCell>();
    var path = new List<GridCell>();

    openStates.Add(start);

    bool foundPath = false;
    long nodeVisits = 0;
    do {
        var position = openStates.OrderBy(s => s.CostFromStart).First();
        openStates.Remove(position);

        // if we found the goal, build the path and break
        if (position == end) {
            do {
                if (position == null) throw new Exception("position cannot be null");
                path.Add(position);
                position = position.PreviousGridCell;
            } while (position != start);
            foundPath = true;
            break;
        } else {
            nodeVisits++;
            foreach (var successor in GetSuccessors(position, grid).OrderBy(s => s.CostFromStart)) {

                var newCost = position.CostFromStart + successor.RiskLevel;

                // ignore if no path improvement
                if ((openStates.Contains(successor) || closedStates.Contains(successor)) &&
                    successor.CostFromStart <= newCost) {
                    continue;
                } else { // store better path
                    successor.PreviousGridCell = position;
                    successor.CostFromStart = newCost;
                    if (closedStates.Contains(successor)) {
                        closedStates.Remove(successor);
                    }
                    if (!openStates.Contains(successor)) {
                        if (successor != start)
                            openStates.Add(successor);
                    }
                }
            }
        }
        closedStates.Add(position);
    } while (openStates.Count > 0);

    var totalRisk = path.Sum(p => p.RiskLevel);

    return new GridSolution(foundPath, totalRisk, nodeVisits, path);

    List<GridCell> GetSuccessors(GridCell gridCell, Grid<GridCell> grid) {
        var (x, y) = gridCell;
        List<GridCell> result = new();

        if (x > 0) result.Add(grid[x - 1, y]);
        if (y > 0) result.Add(grid[x, y - 1]);
        if (x < grid.Width - 1) result.Add(grid[x + 1, y]);
        if (y < grid.Height - 1) result.Add(grid[x, y + 1]);

        return result;
    }
}

void Part1(Data data, string title) {
    if (data.Grid == null) return;

    var grid = new Grid<GridCell>(data.Grid);

    var baseWidth = data.Grid.Width;
    var start = grid[0, 0];
    var end = grid[grid.Width - 1, grid.Height - 1];

    var gridSolution = SolveGrid(start, end, grid);

    PrintGridSolution(title, gridSolution, grid);
}

void Part2(Data data, string title, int sizeMulitplier) {

    if (data.Grid == null) return;

    var baseWidth = data.Grid.Width;
    var baseHeight = data.Grid.Height;
    var gridWidth = data.Grid.Width * sizeMulitplier;
    var gridHeight = data.Grid.Height * sizeMulitplier;

    var grid = new Grid<GridCell>(baseWidth * sizeMulitplier, baseHeight * sizeMulitplier);

    // Create riskLevels accross the grid
    for (var x = 0; x < gridWidth; x++) {
        for (var y = 0; y < gridHeight; y++) {
            int riskLevelAdjustX = (int)Floor((double)x / baseWidth);
            int riskLevelAdjustY = (int)Floor((double)y / baseHeight);

            var originalX = x % baseWidth;
            var originalY = y % baseHeight;

            var originalRiskLevel = data.Grid[originalX, originalY].RiskLevel;
            var riskLevel = (originalRiskLevel + riskLevelAdjustX + riskLevelAdjustY) % 9;
            if (riskLevel == 0) riskLevel = 9;

            grid[x, y] = new GridCell(x, y, riskLevel);
        }
    }

    var position = grid[0, 0];
    var end = grid[grid.Width - 1, grid.Height - 1];

    $"position: {position.X},{position.Y} -> end: {end.X},{end.Y}  |  {end.X},{end.Y}".Dump();
    var solution = SolveGrid(position, end, grid);
    PrintGridSolution(title, solution, grid);
}

class GridCell {
    public GridCell() { }
    public GridCell(int x, int y, int riskLevel) => (X, Y, RiskLevel) = (x, y, riskLevel);

    public int X { get; set; }
    public int Y { get; set; }
    public int RiskLevel { get; set; }
    public double CostFromStart { get; set; }
    public double CostToGoal { get; set; }
    public GridCell? PreviousGridCell { get; set; }

    public void Deconstruct(out int X, out int Y) {
        X = this.X;
        Y = this.Y;
    }

    public override string ToString() => RiskLevel.ToString();
}

#region Grid<TCell>
partial class Grid<TCell> where TCell : new() {
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

    public void DumpGrid(Func<TCell, string>? displayText = null) {
        for (var y = 0; y < Height; y++) {
            List<string> cells = new();
            for (var x = 0; x < Width; x++) {
                if (displayText == null) {
                    if (this[x, y] != null)
                        cells.Add(this[x, y].ToString());
                    else
                        cells.Add(" ");
                } else {
                    cells.Add(displayText(this[x, y]));
                }
            }
            string.Join(" ", cells).Dump();
        }
    }
}

partial class Grid<TCell> {
    public void ForEach(Action<TCell> action) {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                action(this[x, y]);
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
    public Grid<GridCell>? Grid { get; set; }
}

partial class Data {
    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();
        int width = 0;
        int height = 0;

        var lines = new List<string>();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            lines.Add(line);

            width = Math.Max(width, line.Length);

        } while (true);
        height = lines.Count;

        result.Grid = new Grid<GridCell>(width, height);
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var value = int.Parse((lines[y])[x].ToString());
                result.Grid[x, y] = new GridCell(x, y, value);
            }
        }

        return result;
    }
}

#region Sample
string sample =
@"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581";
#endregion