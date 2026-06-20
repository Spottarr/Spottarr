namespace Spottarr.Services.Helpers;

/// <summary>
/// A <see cref="TextReader"/> that streams over a sequence of string segments as if they were a single
/// concatenated string, without ever materialising that concatenation in memory. Segments are pulled
/// lazily from the underlying enumerator and only a single segment is held at a time, which keeps memory
/// usage low when stitching together the (potentially many and large) X-XML header values of a spot.
/// </summary>
internal sealed class SegmentTextReader : TextReader
{
    private readonly IEnumerator<string> _segments;
    private string _current = string.Empty;
    private int _position;

    public SegmentTextReader(IEnumerable<string> segments)
    {
        ArgumentNullException.ThrowIfNull(segments);
        _segments = segments.GetEnumerator();
    }

    public override int Peek() => EnsureCurrent() ? _current[_position] : -1;

    public override int Read() => EnsureCurrent() ? _current[_position++] : -1;

    public override int Read(char[] buffer, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index + count, buffer.Length);

        var read = 0;
        while (count > 0 && EnsureCurrent())
        {
            var available = Math.Min(count, _current.Length - _position);
            _current.CopyTo(_position, buffer, index, available);
            _position += available;
            index += available;
            count -= available;
            read += available;
        }

        return read;
    }

    /// <summary>
    /// Ensures <see cref="_current"/> points at a segment with unread characters, advancing past any
    /// exhausted or empty segments. Returns <see langword="false"/> once the input is fully consumed.
    /// </summary>
    private bool EnsureCurrent()
    {
        while (_position >= _current.Length)
        {
            if (!_segments.MoveNext())
                return false;

            _current = _segments.Current ?? string.Empty;
            _position = 0;
        }

        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _segments.Dispose();

        base.Dispose(disposing);
    }
}
