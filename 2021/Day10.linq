<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day10
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 341823
//   Part2 := 2801302861, too low
//
async Task Main() {
    // sample data
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day10-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    Part1(data);
}

void Part1(Data data) {

    var lines = new List<string>(data.Lines);
    var corruptedLines = new List<string>();

    var corruptedScore = 0;
    foreach (var line in lines) {
        var stack = new Stack<char>();
        foreach (var ch in line) {
            if (data.Chunks.Keys.Contains(ch)) {
                stack.Push(data.Chunks[ch].CloseSymbol);
            } else {
                if (stack.Count == 0) continue;
                var chEnd = stack.Pop();

                if (ch != chEnd) {
                    corruptedLines.Add(line);
                    var errorValue = data.Chunks.Values.Where(v => v.CloseSymbol == ch).First().CorruptValue;
                    corruptedScore += errorValue;
                    break;
                }
            }
        }
    }
    corruptedScore.Dump("Part1");

    foreach (var line in corruptedLines) lines.Remove(line);

    var lineScores = new List<long>();
    foreach (var line in lines) {
        long lineScore = 0;
        var stack = new Stack<char>();
        foreach (var ch in line) {
            if (data.Chunks.Keys.Contains(ch)) {
                stack.Push(data.Chunks[ch].CloseSymbol);
            } else {
                if (stack.Count == 0) continue;
                var chEnd = stack.Pop();
                if (ch != chEnd) throw new Exception("This line is corrupted");
            }
        }
        while(stack.Count>0){
            var chEnd = stack.Pop();
            var completedValue = data.Chunks.Where(c => c.Value.CloseSymbol == chEnd).First().Value.CompleteValue;
            lineScore = (lineScore * 5) + completedValue;
        }        
        lineScores.Add(lineScore);
    }
    
    var index = (lineScores.Count - 1) / 2;

    lineScores.OrderBy(score => score).Skip(index).Take(1).First().Dump("Part2");
}

partial class Data {
    public List<string> Lines { get; set; } = new();
    public Dictionary<char, (char CloseSymbol, int CorruptValue, int CompleteValue)> Chunks { get; private set; } = new();
}

partial class Data {

    public Data() {
        Chunks.Add('(', (CloseSymbol: ')', CorruptValue: 3, CompleteValue: 1));
        Chunks.Add('[', (CloseSymbol: ']', CorruptValue: 57, CompleteValue: 2));
        Chunks.Add('{', (CloseSymbol: '}', CorruptValue: 1197, CompleteValue: 3));
        Chunks.Add('<', (CloseSymbol: '>', CorruptValue: 25137, CompleteValue: 4));
    }

    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            result.Lines.Add(line.Trim());
        } while (true);
        return result;
    }
}

#region Sample
string sample =
@"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]
";
#endregion