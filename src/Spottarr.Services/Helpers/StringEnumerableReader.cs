namespace Spottarr.Services.Helpers;

internal sealed class StringEnumerableReader : TextReader
{
    private readonly IEnumerator<string> _enumerator;
    private StringReader? _currentLineReader;

    public StringEnumerableReader(IEnumerable<string> source)
    {
        _enumerator = source.GetEnumerator();
        _currentLineReader = null;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        var hasNextLine = _enumerator.MoveNext();

        if (_currentLineReader == null && !hasNextLine)
            return 0; // End of input

        // Either we have no current line reader, or the end of the line has been reached
        if (_currentLineReader == null || _currentLineReader.Peek() == -1)
        {
            _currentLineReader?.Dispose();
            _currentLineReader = null;

            if (!hasNextLine || string.IsNullOrEmpty(_enumerator.Current)) return 0;

            // Create a new reader
            _currentLineReader = new StringReader(_enumerator.Current);
        }

        // Read the requested chunk from the current line reader
        return _currentLineReader.Read(buffer, index, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _currentLineReader?.Dispose();
            _enumerator.Dispose();
        }

        base.Dispose(disposing);
    }
}