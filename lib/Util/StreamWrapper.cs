using System;
using System.IO;

namespace GCheckout.Util {

  /// <summary>
  /// Class used by the Encoder Helper to stop teh XmlReader from closing the stream
  /// </summary>
  internal class StreamWrapper : Stream {
    
    Stream _bs;

    public StreamWrapper(Stream bs) {
      _bs = bs;
    }

    public override bool CanRead {
      get {
        return true;
      }
    }

    public override bool CanSeek {
      get {
        return true;
      }
    }

    public override bool CanWrite {
      get {
        return true;
      }
    }

    public override void Close() {
      //If the xml can't parse, the XmlReader Closes the stream
      //we will stop this from happening
    }

    public override void Flush() {
      _bs.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin) {
      return _bs.Seek(offset, origin);
    }

    public override long Length {
      get {
        return _bs.Length;
      }
    }

    public override long Position {
      get {
        return _bs.Position;
      }
      set {
        _bs.Position = value;
      }
    }

    public override void Write(byte[] buffer, int offset, int count) {
      _bs.Write(buffer, offset, count);
    }

    public override int Read(byte[] buffer, int offset, int count) {
      return _bs.Read(buffer, offset, count);
    }


    public override void SetLength(long value) {
      _bs.SetLength(value);
    }

  }
}

