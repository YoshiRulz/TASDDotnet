namespace IO.TASD.V1;

extern alias bitops_polyfill;

using System.Diagnostics.CodeAnalysis;
using System.Text;

using static bitops_polyfill::System.Numerics.BitOperations;

#pragma warning disable CS0660
#pragma warning disable CS0661
/// <seealso cref="TASDRawHeader.TryParse"/>
/// <seealso cref="TASDRawHeader.Parse"/>
/// <seealso cref="TASDRawPacket"/>
/// <seealso cref="TASDRawPacketEnumeratorSafe.TryCreate"/>
/// <seealso cref="TASDRawPacketEnumeratorThrowing.Create"/>
public readonly ref struct TASDRawHeader {
	internal const string ERR_MSG_MISSING_MAGIC_BYTES = "missing magic bytes";

	internal const string ERR_MSG_NO_DEF_CTOR = "do not use parameterless ctor";

	internal const string ERR_MSG_TOO_SHORT = "missing version field or global key length field";

	private const int OFFSET_VERSION = sizeof(u32);

	private const int OFFSET_G_KEYLEN = OFFSET_VERSION + sizeof(u16);

	public const int FIXED_LENGTH = OFFSET_G_KEYLEN + sizeof(u8);

	private const u8 MAGIC_BYTE_A = (u8) 'A';

	private const u8 MAGIC_BYTE_D = (u8) 'D';

	private const u8 MAGIC_BYTE_S = (u8) 'S';

	private const u8 MAGIC_BYTE_T = (u8) 'T';

	private const u32 MAGIC_BYTES = 0x54415344U;

	/// <value>a version 1 header which specifies that packet keys are all 2 octets wide</value>
	/// <remarks>by necessity, returns a new instance (stack allocation)</remarks>
	public static TASDRawHeader V1_Keys2o
		=> new(version: 1, globalKeyLength: 2);

	public static bool DoSanityChecks(TASDRawHeader header) {
		//TODO what am I going to do here? there's the > 0 check, and then there's a check that it's a value supported by this library
		return true;
	}

	/// <summary>Extracts the 7-octet TASD header at the start of <paramref name="buf"/>.</summary>
	/// <exception cref="ArgumentException"><paramref name="buf"/> too short or does not start with <see cref="MAGIC_BYTES"/></exception>
	/// <remarks><paramref name="buf"/> is not copied. Do not write to it while the returned struct is still in scope.</remarks>
	/// <seealso cref="TASDRawHeader.TryParse"/>
	public static TASDRawHeader Parse(bbuf buf)
		=> buf is [ MAGIC_BYTE_T, MAGIC_BYTE_A, MAGIC_BYTE_S, MAGIC_BYTE_D, _, _, _, .. ]
			? new(buf)
			: throw new ArgumentException(
				paramName: nameof(buf),
				message: buf is [ MAGIC_BYTE_T, MAGIC_BYTE_A, MAGIC_BYTE_S, MAGIC_BYTE_D, .. ]
					? ERR_MSG_TOO_SHORT // could specify which field is missing, but doesn't seem worth it TODO easier now, do it
					: ERR_MSG_MISSING_MAGIC_BYTES
			);

	/// <summary>Extracts the 7-octet TASD header at the start of <paramref name="buf"/>.</summary>
	/// <returns>
	/// <see langword="true"/> iff successful
	/// (value of <paramref name="result"/> is undefined when returns <see langword="false"/>)
	/// </returns>
	/// <remarks><paramref name="buf"/> is not copied. Do not write to it while the returned struct is still in scope.</remarks>
	/// <seealso cref="TASDRawHeader.Parse"/>
	public static bool TryParse(bbuf buf, [MaybeNullWhen(false)] out TASDRawHeader result) {
		result = new(buf);
		return buf is [ MAGIC_BYTE_T, MAGIC_BYTE_A, MAGIC_BYTE_S, MAGIC_BYTE_D, _, _, _, .. ];
	}

	/// <summary>
	/// Writes a TASD header of the given <paramref name="version"/> and <paramref name="globalKeyLength"/>
	/// to the start of <paramref name="dst"/>.
	/// </summary>
	/// <returns><see langword="true"/> iff successful</returns>
	/// <seealso cref="TASDRawHeader.WriteTo(rwbbuf,u16,u8)"/>
	public static bool TryWriteTo(rwbbuf dst, u16 version, u8 globalKeyLength) {
		if (dst.Length < FIXED_LENGTH) return false;
		_ = WriteTo(dst, version: version, globalKeyLength: globalKeyLength); // since length check passed, there's no other way for the impl. to throw, so no try/catch necessary
		return true;
	}

	/// <summary>
	/// Writes a TASD header of the given <paramref name="version"/> and <paramref name="globalKeyLength"/>
	/// to the start of <paramref name="dst"/>.
	/// </summary>
	/// <returns>a <see cref="rwbbuf"/> equivalent to <see langword="dst"/></returns>
	/// <seealso cref="TASDRawHeader.TryWriteTo(rwbbuf,u16,u8)"/>
	public static rwbbuf WriteTo(rwbbuf dst, u16 version, u8 globalKeyLength)
		=> dst.WriteU32BE(MAGIC_BYTES)
			.WriteU16BE(version, offset: OFFSET_VERSION)
			.WriteU8(globalKeyLength, offset: OFFSET_G_KEYLEN);

	/// <returns><see langword="true"/> iff raw headers have the same <see cref="Version"/> and <see cref="GlobalKeyLength"/></returns>
	public static bool operator ==(TASDRawHeader left, TASDRawHeader right)
		=> left.VariableBits.FastSequenceEqual(right.VariableBits);

	/// <returns><see langword="false"/> iff raw headers have the same <see cref="Version"/> and <see cref="GlobalKeyLength"/></returns>
	public static bool operator !=(TASDRawHeader left, TASDRawHeader right)
		=> !left.VariableBits.FastSequenceEqual(right.VariableBits);

	internal readonly bbuf _buf;

	/// <remarks>called <c>G_KEYLEN</c> in the spec</remarks>
	public readonly u8 GlobalKeyLength
		=> _buf[OFFSET_G_KEYLEN];

	private readonly bbuf VariableBits
		=> _buf[OFFSET_VERSION..FIXED_LENGTH];

	public readonly u16 Version
		=> _buf.ReadU16BE(offset: OFFSET_VERSION);

	internal TASDRawHeader(bbuf buf)
		=> _buf = buf;

	/// <remarks>TODO this documentation</remarks>
	public TASDRawHeader(
		u16 version,
		u8 globalKeyLength
	): this(WriteTo(new u8[FIXED_LENGTH], version: version, globalKeyLength: globalKeyLength)) {}

	[Obsolete(ERR_MSG_NO_DEF_CTOR, error: true)]
	public TASDRawHeader(): this(default(bbuf))
		=> throw new InvalidOperationException(ERR_MSG_NO_DEF_CTOR);

	public override readonly string ToString()
		=> $"{nameof(TASDRawHeader)}(version: {Version}, globalKeyLength: {GlobalKeyLength})";

	/// <summary>Writes this 7-octet TASD header to the start of <paramref name="dst"/>.</summary>
	/// <returns><see langword="true"/> iff successful</returns>
	/// <seealso cref="TASDRawHeader.TryWriteTo(rwbbuf,u16,u8)"/>
	public readonly bool TryWriteTo(rwbbuf dst)
		=> _buf[..FIXED_LENGTH].TryCopyTo(dst);

	/// <summary>Writes this 7-octet TASD header to the start of <paramref name="dst"/>.</summary>
	/// <seealso cref="TASDRawHeader.WriteTo(rwbbuf,u16,u8)"/>
	public readonly void WriteTo(rwbbuf dst)
		=> _buf[..FIXED_LENGTH].CopyTo(dst);
}

/// <seealso cref="TASDRawPacket.TryParse"/>
/// <seealso cref="TASDRawPacket.Parse"/>
/// <seealso cref="TASDRawHeader"/>
/// <seealso cref="TASDRawPacketEnumeratorSafe.TryCreate"/>
/// <seealso cref="TASDRawPacketEnumeratorThrowing.Create"/>
public readonly ref struct TASDRawPacket {
	/// <remarks>shouldn't be a problem in practice because <see cref="int.MaxValue">s32.MaxValue</see> octets is 2 gibibytes</remarks>
	internal const string ERR_MSG_LEN_TOO_HIGH = "packet length > ~2^31, but .NET only supports indexing with s32 and so cannot reach that high";

	private const string ERR_MSG_NO_TOSTRING = $"{nameof(ToString)} is not implemented for {nameof(TASDRawPacket)}";

	internal const string ERR_MSG_TOO_SHORT_LEN = "buffer not long enough for length field";

	internal const string ERR_MSG_TOO_SHORT_PAYLOAD = "buffer not long enough for payload";

	internal const string ERR_MSG_TOO_SHORT_PKHDR = "missing key field, length-length field, or length field";

	internal const u8 HARDCODED_G_KEYLEN = 2;

	private const u64 MAX_PAYLOAD_LENGTH = int.MaxValue - TASDRawHeader.FIXED_LENGTH - 3U; //TODO check off-by-ones w.r.t. this

#pragma warning disable CA2019 // doesn't allow default -_-
	[ThreadStatic]
	private static StringBuilder? _sb = default;
#pragma warning restore CA2019

	private static StringBuilder GetStringBuilder()
		=> (_sb ??= new()).Clear();

	/// <summary>
	/// Extracts the key and payload of the packet at the start of <paramref name="buf"/>.
	/// <paramref name="endOffset"/> is then set to the index immediately after it,
	/// which should point either to the next packet or to the nonexistent octet immediately after the last.
	/// </summary>
	/// <exception cref="ArgumentException">TODO this documentation</exception>
	/// <exception cref="NotSupportedException">specified payload length is too large for .NET (as indexing uses <c>s32</c>)</exception>
	/// <remarks><paramref name="buf"/> is not copied. Do not write to it while the returned struct is still in scope.</remarks>
	public static TASDRawPacket Parse(bbuf buf, TASDRawHeader header, out int endOffset) {
		if (buf.Length < header.GlobalKeyLength + 2) {
			if (buf.Length == header.GlobalKeyLength + 1) {
				if (buf[header.GlobalKeyLength] is 0) {
					endOffset = header.GlobalKeyLength + 1;
					return new(key: buf[..header.GlobalKeyLength], payload: bbuf.Empty);
				}
				throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_LEN);
			}
			throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_PKHDR);
		}

		var lenLen = buf[header.GlobalKeyLength]; // called `PEXP` in the spec
		u64 len = lenLen switch { // called `PLEN` in the spec
			0 => 0UL, // prohibited by the spec, but just in case there's a multi-octet zero length
			sizeof(u8) => buf[header.GlobalKeyLength + 1],
			sizeof(u16) => buf.Length < header.GlobalKeyLength + 3
				? throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_LEN)
				: buf.ReadU16BE(offset: header.GlobalKeyLength + 1),
			sizeof(u32) - 1 => buf.Length < header.GlobalKeyLength + 4
				? throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_LEN)
				: buf.ReadU24BE(offsetLessOne: header.GlobalKeyLength),
			sizeof(u32) => buf.Length < header.GlobalKeyLength + 5
				? throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_LEN)
				: buf.ReadU32BE(offset: header.GlobalKeyLength + 1),
			_ => throw new NotSupportedException(ERR_MSG_LEN_TOO_HIGH) // ehh the first few octets could be 0...
		};
		if (len is 0UL) {
			endOffset = unchecked((int) (header.GlobalKeyLength + 1U + lenLen));
			return new(key: buf[..header.GlobalKeyLength], payload: bbuf.Empty);
		}

		int startOffset = header.GlobalKeyLength + 1 + lenLen;
		try {
			endOffset = startOffset + (int) len;
		} catch (OverflowException e) {
			throw new ArgumentException(paramName: nameof(buf), innerException: e, message: ERR_MSG_LEN_TOO_HIGH);
		}
		if (buf.Length < endOffset) throw new ArgumentException(paramName: nameof(buf), message: ERR_MSG_TOO_SHORT_PAYLOAD);
		bbuf payload;
		try {
			payload = buf.Slice(start: startOffset, length: unchecked((int) len));
		} catch (ArgumentOutOfRangeException e) {
			throw new ArgumentException(paramName: nameof(buf), innerException: e, message: ERR_MSG_LEN_TOO_HIGH);
		}
		return new(key: buf[..header.GlobalKeyLength], payload: payload);
	}

	/// <summary>
	/// Extracts the key and payload of the packet at the start of <paramref name="buf"/>.
	/// <paramref name="endOffset"/> is then set to the index immediately after it,
	/// which should point either to the next packet or to the nonexistent octet immediately after the last.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> iff successful
	/// (value of <paramref name="result"/> is undefined when returns <see langword="false"/>)
	/// </returns>
	/// <remarks><paramref name="buf"/> is not copied. Do not write to it while the returned struct is still in scope.</remarks>
	public static bool TryParse(
		bbuf buf,
		TASDRawHeader header,
		[MaybeNullWhen(false)] out TASDRawPacket result,
		[MaybeNullWhen(false)] out int endOffset
	) {
		result = default;
		endOffset = default;
		if (buf.Length < header.GlobalKeyLength + 2) {
			var sortaValid0Len = buf.Length == header.GlobalKeyLength + 1 && buf[header.GlobalKeyLength] is 0;
			if (sortaValid0Len) {
				endOffset = header.GlobalKeyLength + 1;
				result = new(key: buf[..header.GlobalKeyLength], payload: bbuf.Empty);
			}
			return sortaValid0Len;
		}

		u64 len; // called `PLEN` in the spec
		var lenLen = buf[header.GlobalKeyLength]; // called `PEXP` in the spec
		switch (lenLen) {
			case 0: // prohibited by the spec, but just in case
				len = 0UL;
				break;
			case 1:
				len = buf[header.GlobalKeyLength + 1];
				break;
			case 2:
				if (buf.Length < header.GlobalKeyLength + 3) return false;
				len = buf.ReadU16BE(offset: header.GlobalKeyLength + 1);
				break;
			case 3:
				if (buf.Length < header.GlobalKeyLength + 4) return false;
				len = buf.ReadU24BE(offsetLessOne: header.GlobalKeyLength);
				break;
			case 4:
				if (buf.Length < header.GlobalKeyLength + 5) return false;
				len = buf.ReadU32BE(offset: header.GlobalKeyLength + 1);
				break;
			default:
				return false;
		};
		if (len is 0UL) {
			endOffset = unchecked((int) (header.GlobalKeyLength + 1U + lenLen));
			result = new(key: buf[..header.GlobalKeyLength], payload: bbuf.Empty);
			return true;
		}

		if (MAX_PAYLOAD_LENGTH <= len) return false; // ...else it is safe to cast len to s32
		int startOffset = header.GlobalKeyLength + 1 + lenLen;
		endOffset = startOffset + unchecked((int) len);
		if (buf.Length < endOffset) return false;
		result = new(
			key: buf[..header.GlobalKeyLength],
			payload: buf.Slice(start: startOffset, length: unchecked((int) len))
		);
		return true;
	}

	/// <remarks>TODO this documentation</remarks>
	public static bool TryWriteTo(rwbbuf dst, bbuf key, bbuf payload) {
		if (dst.Length < key.Length + Log2(unchecked((u32) payload.Length)) + payload.Length) return false;
		try { //TODO don't
			_ = WriteTo(dst, key: key, payload: payload);
			return true;
		} catch (Exception) {
			return false;
		}
	}

	/// <remarks>TODO this documentation</remarks>
	public static rwbbuf WriteTo(rwbbuf dst, bbuf key, bbuf payload) {
		if (payload.Length is 0) {
			dst.WriteU16BE(0x01_00, offset: key.Length); // this is the canonical encoding
		} else {
			var lenLen = Log2(unchecked((u32) payload.Length)) >> 3; // called `PEXP` in the spec
			if (lenLen is 0) lenLen++; // log2(1) == 0
			_ = lenLen switch {
				sizeof(u8) => dst.WriteU8(unchecked((u8) payload.Length), offset: key.Length + 1),
				sizeof(u16) => dst.WriteU16BE(unchecked((u16) payload.Length), offset: key.Length + 1),
				sizeof(u32) - 1 => dst.WriteU32BE(unchecked((u32) payload.Length), offset: key.Length + 1), // first octet will be overwritten by lenLen
				sizeof(u32) => dst.WriteU32BE(unchecked((u32) payload.Length), offset: key.Length + 1),
				_ => throw new NotSupportedException(ERR_MSG_LEN_TOO_HIGH)
			};
			dst[key.Length] = unchecked((u8) lenLen);
			payload.CopyTo(dst.Slice(start: key.Length + 1 + lenLen));
		}
		key.CopyTo(dst);
		return dst;
	}

	/// <returns>
	/// <see langword="true"/> iff raw packets have the same <see cref="Key"/> and <see cref="Payload"/>
	/// (values; for strict reference equality, check the fields yourself)
	/// </returns>
	public static bool operator ==(TASDRawPacket left, TASDRawPacket right)
		=> left.Key.FastSequenceEqual(right.Key) && left.Payload.FastSequenceEqual(right.Payload);

	/// <returns>
	/// <see langword="false"/> iff raw packets have the same <see cref="Key"/> and <see cref="Payload"/>
	/// (values; for strict reference equality, check the fields yourself)
	/// </returns>
	public static bool operator !=(TASDRawPacket left, TASDRawPacket right)
		=> !left.Key.FastSequenceEqual(right.Key) || !left.Payload.FastSequenceEqual(right.Payload);

	public readonly bbuf Key;

	public readonly bbuf Payload;

	public TASDRawPacket(bbuf key, bbuf payload) {
		Key = key;
		Payload = payload;
	}

	[Obsolete(TASDRawHeader.ERR_MSG_NO_DEF_CTOR, error: true)]
	public TASDRawPacket(): this(default, default)
		=> throw new InvalidOperationException(TASDRawHeader.ERR_MSG_NO_DEF_CTOR);

	/// <remarks>TODO this documentation</remarks>
	public void Deconstruct(out bbuf key, out bbuf payload) {
		key = Key;
		payload = Payload;
	}

	public override string ToString()
		=> GetStringBuilder().Append(nameof(TASDRawPacket))
			.Append("(key: ").AppendOctetsAsHex(Key)
			.Append(", payload: ").AppendOctetsAsHex(Payload, "_")
			.Append(')').ToString();

	/// <remarks>TODO this documentation</remarks>
	public readonly bool TryWriteTo(rwbbuf dst)
		=> TryWriteTo(dst, key: Key, payload: Payload);

	/// <remarks>TODO this documentation</remarks>
	public readonly void WriteTo(rwbbuf dst)
		=> _ = WriteTo(dst, key: Key, payload: Payload);
}

/// <seealso cref="TASDRawPacketEnumeratorSafe.TryCreate"/>
/// <seealso cref="TASDRawPacketEnumeratorThrowing.Create"/>
/// <seealso cref="TASDRawHeader"/>
/// <seealso cref="TASDRawPacket"/>
public ref struct TASDRawPacketEnumeratorSafe {
	public ref struct Filtered {
		private readonly bbuf _key;

		/// <remarks>cannot be <see langword="readonly"/> as that also prevents <see cref="MoveNext"/> from mutating it via instance method(s)</remarks>
		private TASDRawPacketEnumeratorSafe _iter;

		public readonly TASDRawPacket Current
			=> _iter.Current;

		internal Filtered(TASDRawPacketEnumeratorSafe iter, bbuf key) {
			_iter = iter;
			_key = key;
		}

		/// <inheritdoc cref="TASDRawPacketEnumeratorSafe.GetEnumerator"/>
		public readonly Filtered GetEnumerator()
			=> this;

		public bool MoveNext() {
			static bool FasterSequenceEqual(bbuf ex, bbuf ac)
				=> ac[1] == ex[1] && ac[0] == ex[0];
			while (true) {
				if (!_iter.MoveNext()) return false;
				if (FasterSequenceEqual(_key, _iter.Current.Key)) return true;
			}
		}
	}

	/// <remarks>TODO this documentation</remarks>
	public static bool TryCreate(
		bbuf fileBuf,
		[MaybeNullWhen(false)] out TASDRawHeader header,
		[MaybeNullWhen(false)] out TASDRawPacketEnumeratorSafe iter
	) {
		var headerParsed = TASDRawHeader.TryParse(fileBuf, out header);
#if false
		if (headerParsed.GlobalKeyLength is not TASDRawPacket.HARDCODED_G_KEYLEN) return false;
#endif
		iter = headerParsed ? new(fileBuf, header) : default;
		return headerParsed;
	}

	private readonly TASDRawHeader _header;

	private readonly bbuf _fileBuf;

	private int _offset;

	private TASDRawPacket _current;

	public readonly TASDRawPacket Current
		=> _current;

	private TASDRawPacketEnumeratorSafe(bbuf fileBuf, TASDRawHeader header) {
		_header = header;
		_fileBuf = fileBuf;
		_offset = TASDRawHeader.FIXED_LENGTH;
		_current = default;
	}

	/// <remarks>normally you'd have the collection implement this, but in this case there's already a <see cref="bbuf.GetEnumerator"/> for iterating octets...</remarks>
	public readonly TASDRawPacketEnumeratorSafe GetEnumerator()
		=> this;

	public bool MoveNext() {
		if (!TASDRawPacket.TryParse(_fileBuf[_offset..], _header, out _current, out var endOffset)) return false;
		_offset += endOffset;
		return true;
	}

	public readonly Filtered OfKey(TASDPacketKey key) {
		if (_header.GlobalKeyLength is not TASDRawPacket.HARDCODED_G_KEYLEN) throw new NotSupportedException("TODO write this message");
		return new(this, new u8[TASDRawPacket.HARDCODED_G_KEYLEN].WriteU16BE(unchecked((u16) key))); //TODO don't (make bbuf.SequenceEquals(TASDPacketKey) and maybe .StartsWith extensions)
	}
}

/// <remarks>TODO does this have any value?</remarks>
/// <seealso cref="TASDRawPacketEnumeratorThrowing.Create"/>
/// <seealso cref="TASDRawPacketEnumeratorSafe.TryCreate"/>
/// <seealso cref="TASDRawHeader"/>
/// <seealso cref="TASDRawPacket"/>
public ref struct TASDRawPacketEnumeratorThrowing {
	/// <remarks>TODO this documentation</remarks>
	public static TASDRawPacketEnumeratorThrowing Create(bbuf fileBuf, out TASDRawHeader headerParsed) {
		headerParsed = TASDRawHeader.Parse(fileBuf);
#if false
		if (headerParsed.GlobalKeyLength is not TASDRawPacket.HARDCODED_G_KEYLEN) throw new ArgumentException(paramName: nameof(fileBuf), message: "TODO write this message");
#endif
		return new(fileBuf, headerParsed);
	}

	private readonly TASDRawHeader _header;

	private readonly bbuf _fileBuf;

	private int _offset;

	private TASDRawPacket _current;

	public readonly TASDRawPacket Current
		=> _current;

	private TASDRawPacketEnumeratorThrowing(bbuf fileBuf, TASDRawHeader header) {
		_header = header;
		_fileBuf = fileBuf;
		_offset = TASDRawHeader.FIXED_LENGTH;
		_current = default;
	}

	/// <inheritdoc cref="TASDRawPacketEnumeratorSafe.GetEnumerator"/>
	public readonly TASDRawPacketEnumeratorThrowing GetEnumerator()
		=> this;

	public bool MoveNext() {
		try {
			_current = TASDRawPacket.Parse(_fileBuf[_offset..], _header, out var endOffset);
			_offset += endOffset;
			return true;
		} catch (Exception) {
			return false;
		}
	}
}
#pragma warning restore CS0660
#pragma warning restore CS0661
