<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// Day08
//
// Links
//   https://adventofcode.com/2021/day/X
//
// Answer
//   Part1 := 392
//   Part2 := 1004688
//
async Task Main() {

    // sample line
    //var data = await Data.ParseAsync(new StringReader("acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab | cdfeb fcadb cdfeb cdbaf"));

    // sample data    
    //var data = await Data.ParseAsync(new StringReader(sample));

    // data
    string path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath ?? "") ?? "", "Day08-input.txt");
    var data = await Data.ParseAsync(new StringReader(await File.ReadAllTextAsync(path)));

    //Part1(data);
    Part2(data);
}

void Part1(Data data) {
    var knownOutputDigits = 0;
    var knownLengths = new int[] { 2, 4, 3, 7 };

    foreach (var entry in data.Entries) {
        var countKnown = entry.Digits.Where(digit => knownLengths.Contains(digit.Length)).Count();
        knownOutputDigits += countKnown;
    }

    knownOutputDigits.Dump("Part1");
}

void Part2(Data data) {

    long value = 0;
    
    foreach (var entry in data.Entries) {
        value += entry.Value;
    }
    
    value.Dump("Part2");
}

#region Notes
/*
  0:      1:      2:      3:      4:
 aaaa    ....    aaaa    aaaa    ....
b    c  .    c  .    c  .    c  b    c
b    c  .    c  .    c  .    c  b    c
 ....    ....    dddd    dddd    dddd
e    f  .    f  e    .  .    f  .    f
e    f  .    f  e    .  .    f  .    f
 gggg    ....    gggg    gggg    ....

  5:      6:      7:      8:      9:
 aaaa    aaaa    aaaa    aaaa    aaaa
b    .  b    .  .    c  b    c  b    c
b    .  b    .  .    c  b    c  b    c
 dddd    dddd    ....    dddd    dddd
.    f  e    f  .    f  e    f  .    f
.    f  e    f  .    f  e    f  .    f
 gggg    gggg    ....    gggg    gggg
*/
#endregion

partial class Entry {
    public List<string> SignalPatterns { get; set; } = new();
    public List<string> Digits { get; set; } = new();
    public long Value { get; set; }

    private Dictionary<string, int> DigitLookup { get; set; } = new();
}

partial class Data {
    public List<Entry> Entries { get; set; } = new();
}

partial class SegmentDisplay {
    public int Digit { get; set; }
    public string Segments { get; set; } = "";

    #region Normal Segment Displays
    public static List<SegmentDisplay> Digits = new List<SegmentDisplay> {
        new SegmentDisplay(){ Digit = 0, Segments = "abcefg" },
        new SegmentDisplay(){ Digit = 1, Segments = "cf" },
        new SegmentDisplay(){ Digit = 2, Segments = "acdeg" },
        new SegmentDisplay(){ Digit = 3, Segments = "acdfg" },
        new SegmentDisplay(){ Digit = 4, Segments = "bcdf" },
        new SegmentDisplay(){ Digit = 5, Segments = "abdfg" },
        new SegmentDisplay(){ Digit = 6, Segments = "abdefg" },
        new SegmentDisplay(){ Digit = 7, Segments = "acf" },
        new SegmentDisplay(){ Digit = 8, Segments = "abcdefg" },
        new SegmentDisplay(){ Digit = 9, Segments = "abcdfg" },
    };
    #endregion
}

partial class Data {
    public static async Task<Data> ParseAsync(TextReader reader) {
        var result = new Data();
        do {
            var line = await reader.ReadLineAsync();
            if (line == null) break;

            var entry = new Entry();
            var parts = line.Trim().Split("|");

            foreach (var signalPattern in parts[0].Trim().Split(" ")) {
                entry.SignalPatterns.Add(signalPattern);
            }
            foreach (var digit in parts[1].Trim().Split(" ")) {
                entry.Digits.Add(digit);
            }
            entry.ComputeEntryDigits();
            entry.ComputeValue();
            result.Entries.Add(entry);
        } while (true);

        return result;
    }
}

partial class Entry {

    public string SortIndex(string index) => new string(index.OrderBy(x => x).ToArray());
    public int FindDigit(string lookup) => DigitLookup[SortIndex(lookup)];

    public void ComputeEntryDigits() {
        string[] mappedSegments = new string[10];

        var remainingPatterns = new List<string>(this.SignalPatterns);

        mappedSegments[1] = remainingPatterns.Where(s => s.Length == 2).First();
        remainingPatterns.Remove(mappedSegments[1]);

        mappedSegments[4] = remainingPatterns.Where(s => s.Length == 4).First();
        remainingPatterns.Remove(mappedSegments[4]);

        mappedSegments[7] = remainingPatterns.Where(s => s.Length == 3).First();
        remainingPatterns.Remove(mappedSegments[7]);

        mappedSegments[8] = remainingPatterns.Where(s => s.Length == 7).First();
        remainingPatterns.Remove(mappedSegments[8]);

        var a = mappedSegments[7].Except(mappedSegments[1]).First().ToString();

        // Find 9
        // join pattern 4 + a (7). for 6 segments, only 9 will have 1 segement left -> g
        var join4A = mappedSegments[4] + a;
        var g = remainingPatterns.Where(s => s.Length == 6)
            .Select(s => new string(s.Except(join4A).ToArray()))
            .Where(s => s.Count() == 1)
            .First();
        var join4AG = mappedSegments[4] + a + g;
        mappedSegments[9] = remainingPatterns.Where(s => s.Intersect(join4AG).Count() == join4AG.Length).First();
        remainingPatterns.Remove(mappedSegments[9]);

        // Find 0
        // for 6 segments, find the remaing that overlap with 1
        mappedSegments[0] = remainingPatterns.Where(s => s.Length == 6)
            .Where(s => (new string(s.Intersect(mappedSegments[1]).ToArray())).Length == 2)
            .Select(s => s)
            .First();
        remainingPatterns.Remove(mappedSegments[0]);

        // Have 6
        // The last length 6 has to be 6
        mappedSegments[6] = remainingPatterns.Where(s => s.Length == 6).Select(s => s).First();
        remainingPatterns.Remove(mappedSegments[6]);

        var d = mappedSegments[8].Except(mappedSegments[0]).First().ToString();

        // Make 3 and find it
        var join7GD = mappedSegments[7] + g + d;
        mappedSegments[3] = remainingPatterns.Where(s => s.Intersect(join7GD).Count() == join7GD.Length).First();
        remainingPatterns.Remove(mappedSegments[3]);

        var e = mappedSegments[8].Except(mappedSegments[9]).First().ToString();

        // Find 5
        // does not contain e
        mappedSegments[5] = remainingPatterns.Where(s => !s.Contains(e)).First();
        remainingPatterns.Remove(mappedSegments[5]);

        // Last we have 2
        mappedSegments[2] = remainingPatterns.First();

        this.DigitLookup.Add(SortIndex(mappedSegments[0]), 0);
        this.DigitLookup.Add(SortIndex(mappedSegments[1]), 1);
        this.DigitLookup.Add(SortIndex(mappedSegments[2]), 2);
        this.DigitLookup.Add(SortIndex(mappedSegments[3]), 3);
        this.DigitLookup.Add(SortIndex(mappedSegments[4]), 4);
        this.DigitLookup.Add(SortIndex(mappedSegments[5]), 5);
        this.DigitLookup.Add(SortIndex(mappedSegments[6]), 6);
        this.DigitLookup.Add(SortIndex(mappedSegments[7]), 7);
        this.DigitLookup.Add(SortIndex(mappedSegments[8]), 8);
        this.DigitLookup.Add(SortIndex(mappedSegments[9]), 9);
    }
    public void ComputeValue(){
        var value = 0;
        foreach(var digit in this.Digits){
            value = value * 10;
            value = value + FindDigit(digit);
        }
        this.Value = value;
    }
}

#region Sample
string sample =
@"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce
";
#endregion