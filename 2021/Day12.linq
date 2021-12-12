<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day12
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
    //string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day12-input.txt");
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

partial class Node {
    public Node() { }
    public Node(string name, string path) {
        Name = name;
        Paths.Add(path);
    }
    public string Name { get; set; } = "";
    public List<string> Paths { get; set; } = new();
}

partial class Data {
    public Dictionary<string, Node> Nodes { get; set; } = new();
}

partial class Data {
    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            var nodes = line.Split("-");
            var nodeLeft = nodes[0];
            var nodeRight = nodes[1];

            if (!result.Nodes.ContainsKey(nodeLeft)) result.Nodes.Add(nodeLeft, new Node(nodeLeft, nodeRight));
            else if (!result.Nodes[nodeLeft].Paths.Contains(nodeRight)) result.Nodes[nodeLeft].Paths.Add(nodeRight);

            if (!result.Nodes.ContainsKey(nodeRight)) result.Nodes.Add(nodeRight, new Node(nodeRight, nodeLeft));
            else if (!result.Nodes[nodeRight].Paths.Contains(nodeLeft)) result.Nodes[nodeRight].Paths.Add(nodeLeft);

        } while (true);

        return result;
    }
}

#region Sample
string sample =
@"start-A
start-b
A-c
A-b
b-d
A-end
b-end";
#endregion