<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day02
//
// Links
//   https://adventofcode.com/2021/day/2
//
// Answer
//   Part1 := 1635930
//   Part2 := 1781819478
//
async Task Main() {

    // sample data
    //var data = sample.Split("\n").ToArray();

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day02-input.txt");
    var data = (await File.ReadAllLinesAsync(path));

    Part1(data);
    Part2(data);
}

record Position(int Horizontal, int Depth);
void Part1(string[] data) {
    var position = new Position(0, 0);

    foreach (var move in data) {
        (string command, int value) = (move.Split(" ")[0], int.Parse(move.Split(" ")[1]));
        position = (command, value) switch {
            _ when command == "forward" => new Position(position.Horizontal + value, position.Depth),
            _ when command == "down" => new Position(position.Horizontal, position.Depth + value),
            _ when command == "up" => new Position(position.Horizontal, position.Depth - value),
            _ => throw new Exception($"Input Error: Command: {command}, Value: {value}"),
        };
    }

    $"{position.Horizontal * position.Depth}".Dump("Part1");
}

record Position2(int Horizontal, int Depth, int Aim);
void Part2(string[] data) {

    var position = new Position2(0, 0, 0);
    
    foreach (var move in data) {
        (string command, int value) = (move.Split(" ")[0], int.Parse(move.Split(" ")[1]));
        position = (command, value) switch {
            _ when command == "forward" => new Position2(position.Horizontal + value, position.Depth + (position.Aim * value), position.Aim),
            _ when command == "down" => new Position2(position.Horizontal, position.Depth, position.Aim + value),
            _ when command == "up" => new Position2(position.Horizontal, position.Depth, position.Aim - value),
            _ => throw new Exception($"Input Error: Command: {command}, Value: {value}"),
        };
    }

    $"{position.Horizontal * position.Depth}".Dump("Part1");
}

#region Sample
string sample =
@"forward 5
down 5
forward 8
up 3
down 8
forward 2";
#endregion