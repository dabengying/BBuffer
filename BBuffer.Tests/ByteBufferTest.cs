﻿using BBuffer;
using NUnit.Framework;

namespace BBufferTests {
	[TestFixture]
	public class ByteBufferTest {
		[Test]
		public void Test() {
			// https://www.h-schmidt.net/FloatConverter/IEEE754.html

			byte[] tmp = new byte[400];
			Endianness[] end = new Endianness[] { Endianness.Big, Endianness.Little };

			byte[] expectedMessage = new byte[] {
				0x56, 0x47, 0x68, 0x70, 0x63, 0x79, 0x42, 0x70,
				0x63, 0x79, 0x42, 0x68, 0x49, 0x48, 0x52, 0x6c,
				0x63, 0x33, 0x51, 0x67, 0x62, 0x57, 0x56, 0x7a,
				0x63, 0x32, 0x46, 0x6e, 0x5a, 0x53, 0x45, 0x3d
			};

			for (int off = 0; off < 30; off++) {
				for (int len = 200; len < 230; len++) {
					foreach (var e in end) {
						ByteBuffer b = new ByteBuffer(tmp, off, len);
						b.endianness = e;
						{
							Assert.AreEqual(off, b.absPosition);
							Assert.AreEqual(0, b.Position);
							Assert.AreEqual(off + len, b.absLength);
							Assert.AreEqual(len, b.Length);
							Assert.AreEqual(off, b.absOffset);
							Assert.AreEqual(e, b.endianness);
						}

						if (e == Endianness.Big) {
							b.Put((ushort) 0x5647);
							b.Put((byte) 0x11);
							b.Put((byte) 0x70);
							b.PutAt(1, (ushort) 0x4768);
							b.Put(0x63794270);
							b.SkipBytes(8);
							b.Put((ulong) 0x633351676257567a);
							b.Position = 8;
							b.Put(4598024348957759504384f);
							b.Put(820518.75f);
							b.SkipBytes(8);
							b.Put(6.8969621286228941E+169d);
						}
						else {
							b.Put((ushort) 0x4756);
							b.Put((byte) 0x11);
							b.Put((byte) 0x70);
							b.PutAt(1, (ushort) 0x6847);
							b.Put(0x70427963);
							b.SkipBytes(8);
							b.Put((ulong) 0x7a56576267513363);
							b.Position = 8;
							b.Put(3.67351315E+24f);
							b.Put(1.01686312E+27f);
							b.SkipBytes(8);
							b.Put(1.5152749180821361E-13d);
						}

						Assert.IsTrue(b.GetCropToCurrentPosition().BufferEquals(new ByteBuffer(expectedMessage)), "off=" + off + ", len=" + len + ", endian=" + e);

						Assert.AreEqual(off + expectedMessage.Length, b.absPosition);
						Assert.AreEqual(off, b.absOffset);
						Assert.AreEqual(expectedMessage.Length, b.Position);
						Assert.AreEqual(len, b.Length);
						Assert.AreEqual(off + len, b.absLength);
						Assert.AreEqual(e, b.endianness);

						Assert.IsTrue(b.BufferEquals(b));
					}
				}
			}
		}

		[Test]
		public void TestVariableLength() {
			// https://www.h-schmidt.net/FloatConverter/IEEE754.html

			byte[] tmp = new byte[2000];
			Endianness[] end = new Endianness[] { Endianness.Big, Endianness.Little };

			for (int off = 0; off < 30; off++) {
				for (int len = 200; len < 230; len++) {
					foreach (var e in end) {
						ByteBuffer b = new ByteBuffer(tmp, off, len);
						b.endianness = e;

						b.PutVariableLength(0);
						b.PutVariableLength(1);
						b.PutVariableLength(-1);
						b.PutVariableLength((uint) 0x7f);
						b.PutVariableLength((uint) 0x80);
						b.PutVariableLength((uint) 0x81);
						b.PutVariableLength((uint) 0xff);
						b.PutVariableLength(0x7f);
						b.PutVariableLength(0x80);
						b.PutVariableLength(0xff);
						b.PutVariableLength((uint) 0x7f);
						b.PutVariableLength((uint) 0x7fff);
						b.PutVariableLength((uint) 0xffff);
						b.PutVariableLength((uint) 0xffffff);
						b.PutVariableLength((uint) 0xffffffff);
						b.PutVariableLength((ulong) 0xffffffffff);
						b.PutVariableLength((ulong) 0xffffffffffff);
						b.PutVariableLength((ulong) 0xffffffffffffff);
						b.PutVariableLength((ulong) 0xffffffffffffffff);
						b.PutVariableLength(0xffffffffff);
						b.PutVariableLength(0xffffffffffff);


						b.Position = 0;


						Assert.AreEqual(0, b.GetIntVariableLength());
						Assert.AreEqual(1, b.GetIntVariableLength());
						Assert.AreEqual(-1, b.GetIntVariableLength());
						Assert.AreEqual((uint) 0x7f, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0x80, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0x81, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0xff, b.GetUIntVariableLength());
						Assert.AreEqual(0x7f, b.GetIntVariableLength());
						Assert.AreEqual(0x80, b.GetIntVariableLength());
						Assert.AreEqual(0xff, b.GetIntVariableLength());
						Assert.AreEqual((uint) 0x7f, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0x7fff, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0xffff, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0xffffff, b.GetUIntVariableLength());
						Assert.AreEqual((uint) 0xffffffff, b.GetUIntVariableLength());
						Assert.AreEqual((ulong) 0xffffffffff, b.GetULongVariableLength());
						Assert.AreEqual((ulong) 0xffffffffffff, b.GetULongVariableLength());
						Assert.AreEqual((ulong) 0xffffffffffffff, b.GetULongVariableLength());
						Assert.AreEqual((ulong) 0xffffffffffffffff, b.GetULongVariableLength());
						Assert.AreEqual(0xffffffffff, b.GetLongVariableLength());
						Assert.AreEqual(0xffffffffffff, b.GetLongVariableLength());
					}
				}
			}
		}
	}
}
