<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day12
//
// Links
//   https://adventofcode.com/2021/day/12
//
// Answer
//   Part1 := 5254
//   Part2 := 149385
//
async Task Main() {
    // sample data
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day12-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    Part1(data);
    Part1(data, true);
}

void Part1(Data data, bool canVisitSmallTwice = false) {

    List<string> paths = new();
    var stack = new Stack<(string Name, List<string> Visited, string SmallVisitedCave, int SmallVisistedCaveCount)>();

    stack.Push(("start", new List<string> { "start" }, "", 0));

    int breakCount = int.MaxValue;
    do {
        var position = stack.Pop();
        foreach (var nextCave in data.Nodes[position.Name].NextCaveNames) {
            var path = new List<string>(position.Visited);
            path.Add(nextCave);

            bool isAllLowercase = IsAllLowercase(nextCave);
            bool haveVisited = position.Visited.Contains(nextCave);
            var smallVisitedCave = position.SmallVisitedCave;
            var smallVisitedCaveCount = position.SmallVisistedCaveCount;
            

            //226
            if (nextCave == "end") {
                paths.Add(string.Join(",", path));
            } else if (!isAllLowercase) {
                stack.Push((nextCave, path, position.SmallVisitedCave, position.SmallVisistedCaveCount));
            }else if (!(isAllLowercase && haveVisited)) {
                stack.Push((nextCave, path, position.SmallVisitedCave, position.SmallVisistedCaveCount));
            }else if(canVisitSmallTwice && isAllLowercase && haveVisited && smallVisitedCaveCount == 0){
                stack.Push((nextCave, path, nextCave, 1));
            }
        }
        breakCount--;
    } while (stack.Count > 0 && breakCount > 0);

    if (!canVisitSmallTwice)
        paths.Count.Dump("Part1");
    else
        paths.Count.Dump("Part2");
}


bool IsAllLowercase(string text) {
    foreach (var ch in text)
        if (Char.IsUpper(ch)) return false;
    return true;
}

partial class Node {
    public Node() { }
    public Node(string name, string path) {
        Name = name;
        NextCaveNames.Add(path);
    }
    public string Name { get; set; } = "";
    public List<string> NextCaveNames { get; set; } = new();
}

partial class Data {
    public Data() { }
    public Data(Data data) {
        Nodes = new Dictionary<string, UserQuery.Node>(data.Nodes);
    }

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
            else if (!result.Nodes[nodeLeft].NextCaveNames.Contains(nodeRight)) result.Nodes[nodeLeft].NextCaveNames.Add(nodeRight);

            if (!result.Nodes.ContainsKey(nodeRight)) result.Nodes.Add(nodeRight, new Node(nodeRight, nodeLeft));
            else if (!result.Nodes[nodeRight].NextCaveNames.Contains(nodeLeft)) result.Nodes[nodeRight].NextCaveNames.Add(nodeLeft);

        } while (true);

        foreach (var node in result.Nodes)
            node.Value.NextCaveNames.Remove("start");

        result.Nodes["end"].NextCaveNames.Clear();

        return result;
    }
}

#region Sample
string sample =
// 10, 36
@"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

// 19, 103
//@"dc-end
//HN-start
//start-kj
//dc-start
//dc-HN
//LN-dc
//HN-end
//kj-sa
//kj-HN
//kj-dc";

// 226, 3509
//@"fs-end
//he-DX
//fs-he
//start-DX
//pj-DX
//end-zg
//zg-sl
//zg-pj
//pj-he
//RW-he
//fs-DX
//pj-RW
//zg-RW
//start-pj
//he-WI
//zg-he
//pj-fs
//start-RW";
#endregion