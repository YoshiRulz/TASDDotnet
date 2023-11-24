namespace Net.TASBot.TASDDotnet;

using static System.Buffers.Binary.BinaryPrimitives;
using static System.Runtime.CompilerServices.MethodImplOptions;

using I = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.ObsoleteAttribute;

/// <remarks>TODO codegen this and publish?</remarks>
public static class BufferExtensions {
	private const string ERR_MSG_USE_EXPLICIT_SIZE_BE = "specify width explicitly (use Write{U16,U32,U64}BE overload)";

	private const string ERR_MSG_USE_EXPLICIT_SIZE_LE = "specify width explicitly (use Write{U16,U32,U64}LE overload)";

	private const string ERR_MSG_USE_INDEXER = "just use the indexer";

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this bbuf buf)
		=> buf[0];

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this bbuf buf, int offset)
		=> buf[offset];

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this rwbbuf buf)
		=> buf[0];

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this rwbbuf buf, int offset)
		=> buf[offset];

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this u8[] buf)
		=> buf[0];

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_INDEXER)]
	public static u8 ReadU8(this u8[] buf, int offset)
		=> buf[offset];

	[I(AggressiveInlining)]
	public static rwbbuf WriteU8(this rwbbuf buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU8(this rwbbuf buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU8(this u8[] buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU8(this u8[] buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteBE(this rwbbuf buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteBE(this rwbbuf buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteBE(this u8[] buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteBE(this u8[] buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteLE(this rwbbuf buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteLE(this rwbbuf buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteLE(this u8[] buf, u8 value) {
		buf[0] = value;
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteLE(this u8[] buf, u8 value, int offset) {
		buf[offset] = value;
		return buf;
	}



	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this bbuf buf)
		=> ReadUInt16BigEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this bbuf buf, int offset)
		=> ReadUInt16BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this rwbbuf buf)
		=> ReadUInt16BigEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this rwbbuf buf, int offset)
		=> ReadUInt16BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this u8[] buf)
		=> ReadUInt16BigEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16BE(this u8[] buf, int offset)
		=> ReadUInt16BigEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16BE(this rwbbuf buf, u16 value) {
		WriteUInt16BigEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16BE(this rwbbuf buf, u16 value, int offset) {
		WriteUInt16BigEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16BE(this u8[] buf, u16 value)
		=> ((rwbbuf) buf).WriteU16BE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16BE(this u8[] buf, u16 value, int offset)
		=> ((rwbbuf) buf).WriteU16BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u16 value)
		=> buf.WriteU16BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u16 value, int offset)
		=> buf.WriteU16BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u16 value)
		=> ((rwbbuf) buf).WriteU16BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u16 value, int offset)
		=> ((rwbbuf) buf).WriteU16BE(value, offset);



	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this bbuf buf)
		=> ReadUInt16LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this bbuf buf, int offset)
		=> ReadUInt16LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this rwbbuf buf)
		=> ReadUInt16LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this rwbbuf buf, int offset)
		=> ReadUInt16LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this u8[] buf)
		=> ReadUInt16LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u16 ReadU16LE(this u8[] buf, int offset)
		=> ReadUInt16LittleEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16LE(this rwbbuf buf, u16 value) {
		WriteUInt16LittleEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16LE(this rwbbuf buf, u16 value, int offset) {
		WriteUInt16LittleEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16LE(this u8[] buf, u16 value)
		=> ((rwbbuf) buf).WriteU16LE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU16LE(this u8[] buf, u16 value, int offset)
		=> ((rwbbuf) buf).WriteU16LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u16 value)
		=> buf.WriteU16LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u16 value, int offset)
		=> buf.WriteU16LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u16 value)
		=> ((rwbbuf) buf).WriteU16LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u16 value, int offset)
		=> ((rwbbuf) buf).WriteU16LE(value, offset);



	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this bbuf buf)
		=> ReadUInt32BigEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this bbuf buf, int offset)
		=> ReadUInt32BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this rwbbuf buf)
		=> ReadUInt32BigEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this rwbbuf buf, int offset)
		=> ReadUInt32BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this u8[] buf)
		=> ReadUInt32BigEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32BE(this u8[] buf, int offset)
		=> ReadUInt32BigEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32BE(this rwbbuf buf, u32 value) {
		WriteUInt32BigEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32BE(this rwbbuf buf, u32 value, int offset) {
		WriteUInt32BigEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32BE(this u8[] buf, u32 value)
		=> ((rwbbuf) buf).WriteU32BE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32BE(this u8[] buf, u32 value, int offset)
		=> ((rwbbuf) buf).WriteU32BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u32 value)
		=> buf.WriteU32BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u32 value, int offset)
		=> buf.WriteU32BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u32 value)
		=> ((rwbbuf) buf).WriteU32BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u32 value, int offset)
		=> ((rwbbuf) buf).WriteU32BE(value, offset);



	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this bbuf buf)
		=> ReadUInt32LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this bbuf buf, int offset)
		=> ReadUInt32LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this rwbbuf buf)
		=> ReadUInt32LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this rwbbuf buf, int offset)
		=> ReadUInt32LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this u8[] buf)
		=> ReadUInt32LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u32 ReadU32LE(this u8[] buf, int offset)
		=> ReadUInt32LittleEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32LE(this rwbbuf buf, u32 value) {
		WriteUInt32LittleEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32LE(this rwbbuf buf, u32 value, int offset) {
		WriteUInt32LittleEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32LE(this u8[] buf, u32 value)
		=> ((rwbbuf) buf).WriteU32LE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU32LE(this u8[] buf, u32 value, int offset)
		=> ((rwbbuf) buf).WriteU32LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u32 value)
		=> buf.WriteU32LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u32 value, int offset)
		=> buf.WriteU32LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u32 value)
		=> ((rwbbuf) buf).WriteU32LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u32 value, int offset)
		=> ((rwbbuf) buf).WriteU32LE(value, offset);



	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this bbuf buf)
		=> ReadUInt64BigEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this bbuf buf, int offset)
		=> ReadUInt64BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this rwbbuf buf)
		=> ReadUInt64BigEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this rwbbuf buf, int offset)
		=> ReadUInt64BigEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this u8[] buf)
		=> ReadUInt64BigEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64BE(this u8[] buf, int offset)
		=> ReadUInt64BigEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64BE(this rwbbuf buf, u64 value) {
		WriteUInt64BigEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64BE(this rwbbuf buf, u64 value, int offset) {
		WriteUInt64BigEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64BE(this u8[] buf, u64 value)
		=> ((rwbbuf) buf).WriteU64BE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64BE(this u8[] buf, u64 value, int offset)
		=> ((rwbbuf) buf).WriteU64BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u64 value)
		=> buf.WriteU64BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this rwbbuf buf, u64 value, int offset)
		=> buf.WriteU64BE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u64 value)
		=> ((rwbbuf) buf).WriteU64BE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_BE)]
	public static rwbbuf WriteBE(this u8[] buf, u64 value, int offset)
		=> ((rwbbuf) buf).WriteU64BE(value, offset);



	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this bbuf buf)
		=> ReadUInt64LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this bbuf buf, int offset)
		=> ReadUInt64LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this rwbbuf buf)
		=> ReadUInt64LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this rwbbuf buf, int offset)
		=> ReadUInt64LittleEndian(buf.Slice(start: offset));

	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this u8[] buf)
		=> ReadUInt64LittleEndian(buf);

	[I(AggressiveInlining)]
	public static u64 ReadU64LE(this u8[] buf, int offset)
		=> ReadUInt64LittleEndian(buf.AsSpan(start: offset));

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64LE(this rwbbuf buf, u64 value) {
		WriteUInt64LittleEndian(buf, value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64LE(this rwbbuf buf, u64 value, int offset) {
		WriteUInt64LittleEndian(buf.Slice(start: offset), value);
		return buf;
	}

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64LE(this u8[] buf, u64 value)
		=> ((rwbbuf) buf).WriteU64LE(value);

	[I(AggressiveInlining)]
	public static rwbbuf WriteU64LE(this u8[] buf, u64 value, int offset)
		=> ((rwbbuf) buf).WriteU64LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u64 value)
		=> buf.WriteU64LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this rwbbuf buf, u64 value, int offset)
		=> buf.WriteU64LE(value, offset);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u64 value)
		=> ((rwbbuf) buf).WriteU64LE(value);

	[I(AggressiveInlining)]
	[O(ERR_MSG_USE_EXPLICIT_SIZE_LE)]
	public static rwbbuf WriteLE(this u8[] buf, u64 value, int offset)
		=> ((rwbbuf) buf).WriteU64LE(value, offset);
}
