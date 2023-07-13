using System.Runtime.InteropServices;
using System.Text;

namespace Networking.Packet; 

// Buffer used by both client and server to send and receive data.
public class PacketBuffer : IDisposable {
    private readonly List<byte> buffer = new();
    private int                 read_pos;
    internal byte[]?            read_buffer;

    public PacketBuffer() { }
    public PacketBuffer(uint id) { write(id); }
    public PacketBuffer(IEnumerable<byte> data) { setBytes(data); }

    public int Length          => buffer.Count;
    public int RemainingLength => Length - read_pos;

    public void setBytes(IEnumerable<byte> data) {
        buffer.AddRange(data);
        read_buffer = buffer.ToArray();
    }
    public void reset() {
        read_buffer = null;
        read_pos    = 0;
        buffer.Clear();
    }
    public void finalize() {
        writeSize();
        read_buffer = buffer.ToArray();
    }
    private void writeSize() {
        var size    = buffer.Count;
        var bytes = BitConverter.GetBytes(size);
        buffer.InsertRange(0, bytes);
    }

    // Regular Writes
    public void write(byte data)   => buffer.Add(data);
    public void write(sbyte data)  => buffer.Add((byte) data);
    public void write(char data)   => buffer.Add((byte) data);
    public void write(bool data)   => buffer.Add(Convert.ToByte(data));
    public void write(short data)  => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(ushort data) => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(int data)    => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(uint data)   => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(long data)   => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(ulong data)  => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(Half data)   => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(float data)  => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(double data) => buffer.AddRange(BitConverter.GetBytes(data));
    public void write(string data) {
        write(data.Length);
        buffer.AddRange(Encoding.ASCII.GetBytes(data));
    }
    // Add decimal support? I dont think its needed but maybe ill add it eventually (its not as trivial tmk)

    // Array Writes
    public void write(IEnumerable<byte> data) {
        write(buffer.Count);
        buffer.AddRange(data);
    }
    // TODO: sbyte  enumerable write
    // TODO: char   enumerable write
    // TODO: bool   enumerable write
    // TODO: short  enumerable write
    // TODO: ushort enumerable write
    // TODO: int    enumerable write
    // TODO: uint   enumerable write
    // TODO: long   enumerable write
    // TODO: ulong  enumerable write
    // TODO: half   enumerable write
    // TODO: float  enumerable write
    // TODO: double enumerable write
    // TODO: string enumerable write

    public byte   readByte() {
        if (!verifySpace(1))
            throw PacketException.toSmall("byte");
        
        var value = read_buffer![read_pos];
        read_pos++;
        return value;
    }
    public sbyte  readSByte() {
        if (!verifySpace(1))
            throw PacketException.toSmall("sbyte");
        
        var value = (sbyte) read_buffer![read_pos];
        read_pos++;
        return value;
    }
    public char   readChar() {
        if (!verifySpace(1))
            throw PacketException.toSmall("char");

        var value = (char) read_buffer![read_pos];
        read_pos++;
        return value;
    }
    public bool   readBool() {
        if (!verifySpace(1))
            throw PacketException.toSmall("bool");

        var value = Convert.ToBoolean(read_buffer![read_pos]);
        read_pos++;
        return value;
    }
    public ushort readUShort() {
        if (!verifySpace(sizeof(ushort)))
            throw PacketException.toSmall("ushort");

        var value = BitConverter.ToUInt16(read_buffer!, read_pos);
        read_pos += sizeof(ushort);
        return value;
    }
    public short  readShort() {
        if (!verifySpace(sizeof(short)))
            throw PacketException.toSmall("short");

        var value = BitConverter.ToInt16(read_buffer!, read_pos);
        read_pos += sizeof(short);
        return value;
    }
    public uint   readUInt() {
        if (!verifySpace(sizeof(uint)))
            throw PacketException.toSmall("uint");

        var value = BitConverter.ToUInt32(read_buffer!, read_pos);
        read_pos += sizeof(uint);
        return value;
    }
    public int    readInt() {
        if (!verifySpace(sizeof(uint)))
            throw PacketException.toSmall("int");

        var value = BitConverter.ToInt32(read_buffer!, read_pos);
        read_pos += sizeof(uint);
        return value;
    }
    public ulong  readULong() {
        if (!verifySpace(sizeof(ulong)))
            throw PacketException.toSmall("ulong");

        var value = BitConverter.ToUInt64(read_buffer!, read_pos);
        read_pos += sizeof(ulong);
        return value;
    }
    public long   readLong() {
        if (!verifySpace(sizeof(ulong)))
            throw PacketException.toSmall("long");

        var value = BitConverter.ToInt64(read_buffer!, read_pos);
        read_pos += sizeof(ulong);
        return value;
    }
    public Half   readHalf() {
        if (!verifySpace((uint) Marshal.SizeOf<Half>()))
            throw PacketException.toSmall("half");

        var value = BitConverter.ToHalf(read_buffer!, read_pos);
        read_pos += Marshal.SizeOf<Half>();
        return value;
    }
    public float  readFloat() {
        if (!verifySpace(sizeof(float)))
            throw PacketException.toSmall("float");

        var value = BitConverter.ToSingle(read_buffer!, read_pos);
        read_pos += sizeof(float);
        return value;
    }
    public double readDouble() {
        if (!verifySpace(sizeof(double)))
            throw PacketException.toSmall("double");

        var value = BitConverter.ToDouble(read_buffer!, read_pos);
        read_pos += sizeof(double);
        return value;
    }
    public string readString() {
        try {
            var length = readInt();
            var value = Encoding.ASCII.GetString(read_buffer!, read_pos, length);
            read_pos += length;
            return value;
        } catch (Exception) {
            throw PacketException.toSmall("string");
        }
    }

    public byte[] readBytes(int length) {
        if (!verifySpace((uint) length))
            throw PacketException.toSmall("byte[]");
        
        var value = new byte[length];
        Array.Copy(read_buffer!, read_pos, value, 0, length);
        read_pos += length;
        return value;
    }
    // TODO: sbyte  array read
    // TODO: char   array read
    // TODO: bool   array read
    // TODO: short  array read
    // TODO: ushort array read
    // TODO: int    array read
    // TODO: uint   array read
    // TODO: long   array read
    // TODO: ulong  array read
    // TODO: half   array read
    // TODO: float  array read
    // TODO: double array read
    // TODO: string array read
    
    public byte   peekByte() {
        if (!verifySpace(1))
            throw PacketException.toSmall("byte");
        
        var value = read_buffer![read_pos];
        return value;
    }
    public sbyte  peekSByte() {
        if (!verifySpace(1))
            throw PacketException.toSmall("sbyte");
        
        var value = (sbyte) read_buffer![read_pos];
        return value;
    }
    public char   peekChar() {
        if (!verifySpace(1))
            throw PacketException.toSmall("char");

        var value = (char) read_buffer![read_pos];
        return value;
    }
    public bool   peekBool() {
        if (!verifySpace(1))
            throw PacketException.toSmall("bool");

        var value = Convert.ToBoolean(read_buffer![read_pos]);
        return value;
    }
    public ushort peekUShort() {
        if (!verifySpace(sizeof(ushort)))
            throw PacketException.toSmall("ushort");

        var value = BitConverter.ToUInt16(read_buffer!, read_pos);
        return value;
    }
    public short  peekShort() {
        if (!verifySpace(sizeof(short)))
            throw PacketException.toSmall("short");

        var value = BitConverter.ToInt16(read_buffer!, read_pos);
        return value;
    }
    public uint   peekUInt() {
        if (!verifySpace(sizeof(uint)))
            throw PacketException.toSmall("uint");

        var value = BitConverter.ToUInt32(read_buffer!, read_pos);
        return value;
    }
    public int    peekInt() {
        if (!verifySpace(sizeof(uint)))
            throw PacketException.toSmall("int");

        var value = BitConverter.ToInt32(read_buffer!, read_pos);
        return value;
    }
    public ulong  peekULong() {
        if (!verifySpace(sizeof(ulong)))
            throw PacketException.toSmall("ulong");

        var value = BitConverter.ToUInt64(read_buffer!, read_pos);
        return value;
    }
    public long   peekLong() {
        if (!verifySpace(sizeof(ulong)))
            throw PacketException.toSmall("long");

        var value = BitConverter.ToInt64(read_buffer!, read_pos);
        return value;
    }
    public Half   peekHalf() {
        if (!verifySpace((uint) Marshal.SizeOf<Half>()))
            throw PacketException.toSmall("half");

        var value = BitConverter.ToHalf(read_buffer!, read_pos);
        return value;
    }
    public float  peekFloat() {
        if (!verifySpace(sizeof(float)))
            throw PacketException.toSmall("float");

        var value = BitConverter.ToSingle(read_buffer!, read_pos);
        return value;
    }
    public double peekDouble() {
        if (!verifySpace(sizeof(double)))
            throw PacketException.toSmall("double");

        var value = BitConverter.ToDouble(read_buffer!, read_pos);
        return value;
    }
    public string peekString() {
        try {
            var length = peekInt();
            var value = Encoding.ASCII.GetString(read_buffer!, read_pos + sizeof(int), length);
            return value;
        } catch (Exception) {
            throw PacketException.toSmall("string");
        }
    }
    
    // TODO: byte   array peek
    // TODO: sbyte  array peek
    // TODO: char   array peek
    // TODO: bool   array peek
    // TODO: short  array peek
    // TODO: ushort array peek
    // TODO: int    array peek
    // TODO: uint   array peek
    // TODO: long   array peek
    // TODO: ulong  array peek
    // TODO: half   array peek
    // TODO: float  array peek
    // TODO: double array peek
    // TODO: string array peek

    private bool verifySpace(uint size) => read_buffer!.Length - read_pos >= size;

    public bool Disposed { get; private set; } = false;

    // ReSharper disable once InconsistentNaming
    public void Dispose(bool dispose_managed) {
        if (Disposed)
            return;

        if (dispose_managed) {
            buffer.Clear();
            read_buffer = null;
            read_pos = 0;
        }

        Disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private class PacketException : Exception {
        public PacketException(string data_type, string reason) : base($"Unable to read {data_type} from PacketBuffer: {reason}!") { }

        public static PacketException toSmall(string data_type) => new(data_type, "Not enough bytes");
    }
}