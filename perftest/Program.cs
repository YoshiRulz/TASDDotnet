using IO.TASD.V0;

Span<int> pressedCounts = stackalloc int[PerfTests.BUTTON_COUNT];
Span<int> heldCounts = stackalloc int[PerfTests.BUTTON_COUNT];
var buf = PerfTests.SampleData;
#if !SLIM
Console.WriteLine("Starting...");
var timer = System.Diagnostics.Stopwatch.StartNew();
#endif
PerfTests.DoCalc(buf, pressedCounts: pressedCounts, heldCounts: heldCounts);
#if !SLIM
timer.Stop();
Console.WriteLine($"{timer.ElapsedMilliseconds} ms");
#endif
Console.WriteLine($"{nameof(pressedCounts)}:\t{pressedCounts[0],6},\t{pressedCounts[1],6},\t{pressedCounts[2],6},\t{pressedCounts[3],6},\t{pressedCounts[4],6},\t{pressedCounts[5],6},\t{pressedCounts[6],6},\t{pressedCounts[7],6},\t{pressedCounts[8],6},\t{pressedCounts[9],6},\t{pressedCounts[10],6},\t{pressedCounts[11],6},\t{pressedCounts[12],6},\t{pressedCounts[13],6},\t{pressedCounts[14],6},\t{pressedCounts[15],6}");
Console.WriteLine($"   {nameof(heldCounts)}:\t{   heldCounts[0],6},\t{   heldCounts[1],6},\t{   heldCounts[2],6},\t{   heldCounts[3],6},\t{   heldCounts[4],6},\t{   heldCounts[5],6},\t{   heldCounts[6],6},\t{   heldCounts[7],6},\t{   heldCounts[8],6},\t{   heldCounts[9],6},\t{   heldCounts[10],6},\t{   heldCounts[11],6},\t{   heldCounts[12],6},\t{   heldCounts[13],6},\t{   heldCounts[14],6},\t{   heldCounts[15],6}");
// see /test/v0/PerfTests.cs for expected values
