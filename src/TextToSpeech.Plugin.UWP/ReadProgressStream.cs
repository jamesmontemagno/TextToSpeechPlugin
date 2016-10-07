using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;


namespace Plugin.TextToSpeech
{
    public class ReadProgressStream : IRandomAccessStream
    {
        public event EventHandler EndOfStream;
        public event EventHandler BytesRead;
        readonly IRandomAccessStream inner;



        public ReadProgressStream(IRandomAccessStream innerStream)
        {
            inner = innerStream;
        }


        public void Dispose()
        {
            inner.Dispose();
        }


        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            var result = inner.ReadAsync(buffer, count, options);
            BytesRead?.Invoke(this, EventArgs.Empty);
            if (Position == Size)
                EndOfStream?.Invoke(this, EventArgs.Empty);

            return result;
        }


        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return inner.WriteAsync(buffer);
        }


        public IAsyncOperation<bool> FlushAsync()
        {
            return inner.FlushAsync();
        }


        public IInputStream GetInputStreamAt(ulong position)
        {
            return inner.GetInputStreamAt(position);
        }


        public IOutputStream GetOutputStreamAt(ulong position)
        {
            return inner.GetOutputStreamAt(position);
        }


        public void Seek(ulong position)
        {
            inner.Seek(position);
        }

        public IRandomAccessStream CloneStream()
        {
            return inner.CloneStream();
        }


        public bool CanRead => inner.CanRead;
        public bool CanWrite => inner.CanWrite;
        public ulong Position => inner.Position;
        public ulong Size
        {
            get { return inner.Size; }
            set { inner.Size = value; }
        }
    }
}
