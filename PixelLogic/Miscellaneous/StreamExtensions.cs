namespace GOoDkat.PixelLogic.Miscellaneous;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public static class StreamExtensions
{
    internal static int Read(this Stream stream, byte[] buffer)
    {
        return stream.Read(buffer, 0, buffer.Length);
    }

    internal static async Task<byte[]> ReadAsync(this Stream stream)
    {
        var buffer = new byte[1024];

        using (var memoryStream = new MemoryStream())
        {
            int count;
            do
            {
                count = await stream.ReadAsync(buffer);
                memoryStream.Write(buffer, 0, count);
            } while (count == buffer.Length);

            return memoryStream.ToArray();
        }
    }

    internal static Task<int> ReadAsync(this Stream stream, byte[] buffer)
    {
        return stream.ReadAsync(buffer, 0, buffer.Length);
    }

    internal static Task<int> ReadAsync(this Stream stream, byte[] buffer, CancellationToken cancellation)
    {
        return stream.ReadAsync(buffer, 0, buffer.Length, cancellation);
    }

    internal static void ReadFixed(this Stream stream, byte[] buffer)
    {
        stream.ReadFixed(buffer, 0, buffer.Length);
    }

    internal static void ReadFixed(this Stream stream, byte[] buffer, int offset, int count)
    {
        while (count > 0)
        {
            int read = stream.Read(buffer, offset, count);
            if (read == 0)
                throw new EndOfStreamException();
            offset += read;
            count -= read;
        }
    }

    internal static void ReadFixed(this Stream stream, Span<byte> buffer)
    {
        while (buffer.Length > 0)
        {
            int read = stream.Read(buffer);
            if (read == 0)
                throw new EndOfStreamException();

            buffer = buffer.Slice(read);
        }
    }

    internal static byte[] ReadFixed(this Stream stream, int count)
    {
        var buffer = new byte[count];
        stream.ReadFixed(buffer);
        return buffer;
    }

    internal static Task ReadFixedAsync(this Stream stream, byte[] buffer)
    {
        return ReadFixedAsync(stream, buffer, 0, buffer.Length, CancellationToken.None);
    }

    internal static Task ReadFixedAsync(this Stream stream, byte[] buffer, CancellationToken cancellation)
    {
        return ReadFixedAsync(stream, buffer, 0, buffer.Length, cancellation);
    }

    internal static Task ReadFixedAsync(this Stream stream, byte[] buffer, int offset, int count)
    {
        return ReadFixedAsync(stream, buffer, offset, count, CancellationToken.None);
    }

    internal static async Task ReadFixedAsync(this Stream stream, byte[] buffer, int offset, int count,
                                              CancellationToken cancellation)
    {
        while (count > 0)
        {
            int read = await stream.ReadAsync(buffer, offset, count, cancellation);
            if (read == 0)
                throw new EndOfStreamException();
            offset += read;
            count -= read;
        }
    }

    internal static async Task ReadFixedAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellation)
    {
        while (buffer.Length > 0)
        {
            int read = await stream.ReadAsync(buffer, cancellation).ConfigureAwait(false);
            if (read == 0)
                throw new EndOfStreamException();

            buffer = buffer.Slice(read);
        }
    }

    internal static async Task<byte[]> ReadFixedAsync(this Stream stream, int count)
    {
        var buffer = new byte[count];
        await stream.ReadFixedAsync(buffer);
        return buffer;
    }

    internal static async Task<byte[]> ReadFixedAsync(this Stream stream, int count,
                                                      CancellationToken cancellationToken)
    {
        var buffer = new byte[count];
        await stream.ReadFixedAsync(buffer, cancellationToken);
        return buffer;
    }

    internal static void Write(this Stream stream, byte[] buffer)
    {
        stream.Write(buffer, 0, buffer.Length);
    }

    internal static Task WriteAsync(this Stream stream, byte[] buffer)
    {
        return stream.WriteAsync(buffer, 0, buffer.Length);
    }

    internal static Task WriteAsync(this Stream stream, byte[] buffer, CancellationToken cancellation)
    {
        return stream.WriteAsync(buffer, 0, buffer.Length, cancellation);
    }

    internal static Task<int> ReadByteAsync(this Stream stream)
    {
        return stream.ReadByteAsync(CancellationToken.None);
    }

    internal static async Task<int> ReadByteAsync(this Stream stream, CancellationToken cancellation)
    {
        var buffer = new byte[1];
        int read = await stream.ReadAsync(buffer, 0, 1, cancellation);
        return read == 1 ? buffer[0] : -1;
    }

    internal static ValueTask WriteByteAsync(this Stream stream, byte value)
    {
        return stream.WriteAsync(new[] {value});
    }
}