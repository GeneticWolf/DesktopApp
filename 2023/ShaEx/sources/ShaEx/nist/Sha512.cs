using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

// This is an implementation of
// http://csrc.nist.gov/publications/fips/fips180-4/fips-180-4.pdf
// based on https://github.com/TerryJackson/Secure-Hash-Algorithms
// Since Sha512 is 37.5% faster then any preceding Sha2 on an x64 machine, I will only concentrate on Sha512
namespace ShaEx {
	using Word32 = UInt32;
	using Word64 = UInt64;

	public static class Sha512Ex {
		public enum eSha512 : Int32 {
			L224 = 224,  /* Sha512/224 */
			L256 = 256,  /* Sha512/256 */
			L384 = 384,  /* Sha384     */
			L512 = 512   /* Sha512     */
		}

		// Constants K
		static Word64[] K512;

		// Initial hash values H0
		static Word64[] H0Sha512_224;
		static Word64[] H0Sha512_256;
		static Word64[] H0Sha512_384;
		static Word64[] H0Sha512_512;

		static Sha512Ex() {
			DefineK512();

			DefineH0Sha512_224();
			DefineH0Sha512_256();
			DefineH0Sha512_384();
			DefineH0Sha512_512();
		}

		public static string Sha512(string pstrText, eSha512 pSha512 = eSha512.L512) {
			Byte[] bytText = ShaUtilities.StringToByteArray(pstrText);
			Byte[] bytData = Sha512Algorithm(bytText, (Int32)pSha512);
			String strSha = ShaUtilities.ByteArrayToHexString(bytData);

			return strSha;
		}

		#region Hash Algorithms
		static Word64[] CreateMessageScheduleSha512(Block1024 block) {
			// The message schedule.
			Word64[] W = new Word64[80];

			// Prepare the message schedule W.
			// The first 16 words in W are the same as the words of the block.
			// The remaining 80-16 =64 words in W are functions of the previously defined words. 
			for (int t = 0; t < 80; t++) {
				if (t < 16) {
					W[t] = block.words[t];
				} else {
					W[t] = sigma1_512(W[t - 2]) + W[t - 7] + sigma0_512(W[t - 15]) + W[t - 16];
				}
			}

			return W;
		}

		static void FileReadBlock(String pstrFullFileName) {
			Boolean blnExist = File.Exists(pstrFullFileName);
			if (blnExist == true) {
				using (FileStream stream = File.Open(pstrFullFileName, FileMode.Open, FileAccess.Read)) {
					long lngFileSize = stream.Length;
					Int32 lngOffSet = 0;
					Int32 lngMaxBuffer = 1024;
					//using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false)) {
						
					//	Byte[] bytBuffer = new Byte[1024];
					//	reader.Read(bytBuffer, lngOffSet, )
					//}
				}

				using (FileStream fsSource = new FileStream(pstrFullFileName, FileMode.Open, FileAccess.Read)) {
					// Read the source file into a byte array.
					byte[] bytes = new byte[fsSource.Length];
					long numBytesToRead = fsSource.Length;
					int numBytesRead = 0;
					//while (numBytesToRead > 0) {
					//	// Read may return anything from 0 to numBytesToRead.
					//	int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

					//	// Break when the end of the file is reached.
					//	if (n == 0)
					//		break;

					//	numBytesRead += n;
					//	numBytesToRead -= n;
					//}
					numBytesToRead = bytes.Length;
				}
			}
		}

		static byte[] Sha512Algorithm(byte[] plaintext, Int32 plngNumberBits) {
			Word64[] H;

			// Set the initial values of the hash[H] variable.
			switch (plngNumberBits) {
				case 224:
					//Sha512_224 = 224
					H = H0Sha512_224;
					break;

				case 256:
					//Sha512_256 = 256
					H = H0Sha512_256;
					break;

				case 384:
					//Sha384 = 384
					H = H0Sha512_384;
					break;

				case 512:
				default:
					//Sha512 = 512
					H = H0Sha512_512;
					break;
			}

			//
			Block1024[] blocks = ConvertPaddedMessageToBlock1024Array(PadPlainText1024(plaintext));

			for (int i = 0; i < blocks.Length; i++) {
				Word64[] W = CreateMessageScheduleSha512(blocks[i]);

				// Set the working variables a,...,h to the current hash values.
				Word64 a = H[0];
				Word64 b = H[1];
				Word64 c = H[2];
				Word64 d = H[3];
				Word64 e = H[4];
				Word64 f = H[5];
				Word64 g = H[6];
				Word64 h = H[7];

				for (int t = 0; t < 80; t++) {
					Word64 T1 = h + Sigma1_512(e) + Ch(e, f, g) + K512[t] + W[t];
					Word64 T2 = Sigma0_512(a) + Maj(a, b, c);
					h = g;
					g = f;
					f = e;
					e = d + T1;
					d = c;
					c = b;
					b = a;
					a = T1 + T2;
				}

				// Update the current value of the hash H after processing block i.
				H[0] += a;
				H[1] += b;
				H[2] += c;
				H[3] += d;
				H[4] += e;
				H[5] += f;
				H[6] += g;
				H[7] += h;
			}

			// Concatenate all the Word64 Hash Values
			byte[] hash = ShaUtilities.Word64ArrayToByteArray(H);
			//String strTest1 = ShaUtilities.ByteArrayToHexString(hash);

			// The number of bytes in the final output hash 
			int numberBytes = plngNumberBits / 8;
			byte[] truncatedHash = new byte[numberBytes];

			Array.Copy(hash, truncatedHash, numberBytes);
			//String strTest2 = ShaUtilities.ByteArrayToHexString(truncatedHash);

			return truncatedHash;
		}
		#endregion

		#region Plaintext preprocessing functions
		static byte[] PadPlainText1024(byte[] plaintext) {
			// After padding the total bits of the output will be divisible by 1024.
			int numberBits = plaintext.Length * 8;
			int t = (numberBits + 8 + 128) / 1024;

			// Note that 1024 * (t + 1) is the least multiple of 1024 greater than (numberBits + 8 + 128)
			// Therefore the number of zero bits we need to add is
			int k = 1024 * (t + 1) - (numberBits + 8 + 128);

			// Since numberBits % 8 = 0, we know k % 8 = 0. So n = k / 8 is the number of zero bytes to add.
			int n = k / 8;

			List<byte> paddedtext = plaintext.ToList();

			// Start the padding by concatenating 1000_0000 = 0x80 = 128
			paddedtext.Add(0x80);

			// Next add n zero bytes
			for (int i = 0; i < n; i++) {
				paddedtext.Add(0);
			}

			// Now add 16 bytes (128 bits) to represent the length of the message in bits.
			// C# does not have 128 bit integer.
			// For now just add 8 zero bytes and then 8 bytes to represent the int
			for (int i = 0; i < 8; i++) {
				paddedtext.Add(0);
			}

			byte[] B = BitConverter.GetBytes((ulong)numberBits);
			Array.Reverse(B);

			for (int i = 0; i < B.Length; i++) {
				paddedtext.Add(B[i]);
			}

			return paddedtext.ToArray();
		}

		static Block1024[] ConvertPaddedMessageToBlock1024Array(byte[] M) {
			// We are assuming M is padded, so the number of bits in M is divisible by 1024 
			int numberBlocks = (M.Length * 8) / 1024;  // same as: M.Length / 128
			Block1024[] blocks = new Block1024[numberBlocks];

			for (int i = 0; i < numberBlocks; i++) {
				// First extract the relavant subarray from M
				byte[] B = new byte[128]; // 128 * 8 = 1024

				for (int j = 0; j < 128; j++) {
					B[j] = M[i * 128 + j];
				}

				Word64[] words = ShaUtilities.ByteArrayToWord64Array(B);
				blocks[i] = new Block1024(words);
			}

			return blocks;
		}
		#endregion

		#region Functions used in the hashing process.
		// Most of these functions have a Word32 version and a Word64 version.
		// Sometimes they are the same (Ch, Maj,..) but sometimes different (Sigma0_256, Sigma0_512).
		// We do not need a RotL or Parity function for Word64 since they are only used in Sha-1.
		static Word32 ShR(int n, Word32 x) {
			// should have 0 <= n < 32
			return (x >> n);
		}

		static Word64 ShR(int n, Word64 x) {
			// should have 0 <= n < 64
			return (x >> n);
		}

		static Word32 RotR(int n, Word32 x) {
			// should have 0 <= n < 32
			return (x >> n) | (x << 32 - n);
		}

		static Word64 RotR(int n, Word64 x) {
			// should have 0 <= n < 64
			return (x >> n) | (x << 64 - n);
		}

		static Word32 RotL(int n, Word32 x) {
			// should have 0 <= n < 32
			return (x << n) | (x >> 32 - n);
		}

		static Word32 Ch(Word32 x, Word32 y, Word32 z) {
			return (x & y) ^ (~x & z);
		}

		static Word64 Ch(Word64 x, Word64 y, Word64 z) {
			return (x & y) ^ (~x & z);
		}

		static Word32 Maj(Word32 x, Word32 y, Word32 z) {
			return (x & y) ^ (x & z) ^ (y & z);
		}

		static Word64 Maj(Word64 x, Word64 y, Word64 z) {
			return (x & y) ^ (x & z) ^ (y & z);
		}

		static Word32 Parity(Word32 x, Word32 y, Word32 z) {
			return x ^ y ^ z;
		}

		static Word32 sigma0_256(Word32 x) {
			return RotR(7, x) ^ RotR(18, x) ^ ShR(3, x);
		}

		static Word32 sigma1_256(Word32 x) {
			return RotR(17, x) ^ RotR(19, x) ^ ShR(10, x);
		}

		static Word64 Sigma0_512(Word64 x) {
			return RotR(28, x) ^ RotR(34, x) ^ RotR(39, x);
		}

		static Word64 Sigma1_512(Word64 x) {
			return RotR(14, x) ^ RotR(18, x) ^ RotR(41, x);
		}

		static Word64 sigma0_512(Word64 x) {
			return RotR(1, x) ^ RotR(8, x) ^ ShR(7, x);
		}

		static Word64 sigma1_512(Word64 x) {
			return RotR(19, x) ^ RotR(61, x) ^ ShR(6, x);
		}
		#endregion

		#region Functions to define the constants K and the initial hashes H0.
		static void DefineK512() {
			// The eighty 64-bit words in the array K512 are used in Sha-384, Sha-512, Sha-512/224, Sha-512/256. 
			// They are obtained by taking the first 64 bits of the fractional
			// parts of the cube roots of the first eighty primes. 

			K512 = new Word64[]
			{
				0x428a2f98d728ae22, 0x7137449123ef65cd, 0xb5c0fbcfec4d3b2f, 0xe9b5dba58189dbbc,
				0x3956c25bf348b538, 0x59f111f1b605d019, 0x923f82a4af194f9b, 0xab1c5ed5da6d8118,
				0xd807aa98a3030242, 0x12835b0145706fbe, 0x243185be4ee4b28c, 0x550c7dc3d5ffb4e2,
				0x72be5d74f27b896f, 0x80deb1fe3b1696b1, 0x9bdc06a725c71235, 0xc19bf174cf692694,
				0xe49b69c19ef14ad2, 0xefbe4786384f25e3, 0x0fc19dc68b8cd5b5, 0x240ca1cc77ac9c65,
				0x2de92c6f592b0275, 0x4a7484aa6ea6e483, 0x5cb0a9dcbd41fbd4, 0x76f988da831153b5,
				0x983e5152ee66dfab, 0xa831c66d2db43210, 0xb00327c898fb213f, 0xbf597fc7beef0ee4,
				0xc6e00bf33da88fc2, 0xd5a79147930aa725, 0x06ca6351e003826f, 0x142929670a0e6e70,
				0x27b70a8546d22ffc, 0x2e1b21385c26c926, 0x4d2c6dfc5ac42aed, 0x53380d139d95b3df,
				0x650a73548baf63de, 0x766a0abb3c77b2a8, 0x81c2c92e47edaee6, 0x92722c851482353b,
				0xa2bfe8a14cf10364, 0xa81a664bbc423001, 0xc24b8b70d0f89791, 0xc76c51a30654be30,
				0xd192e819d6ef5218, 0xd69906245565a910, 0xf40e35855771202a, 0x106aa07032bbd1b8,
				0x19a4c116b8d2d0c8, 0x1e376c085141ab53, 0x2748774cdf8eeb99, 0x34b0bcb5e19b48a8,
				0x391c0cb3c5c95a63, 0x4ed8aa4ae3418acb, 0x5b9cca4f7763e373, 0x682e6ff3d6b2b8a3,
				0x748f82ee5defb2fc, 0x78a5636f43172f60, 0x84c87814a1f0ab72, 0x8cc702081a6439ec,
				0x90befffa23631e28, 0xa4506cebde82bde9, 0xbef9a3f7b2c67915, 0xc67178f2e372532b,
				0xca273eceea26619c, 0xd186b8c721c0c207, 0xeada7dd6cde0eb1e, 0xf57d4f7fee6ed178,
				0x06f067aa72176fba, 0x0a637dc5a2c898a6, 0x113f9804bef90dae, 0x1b710b35131c471b,
				0x28db77f523047d84, 0x32caab7b40c72493, 0x3c9ebe0a15c9bebc, 0x431d67c49c100d4c,
				0x4cc5d4becb3e42b6, 0x597f299cfc657e2a, 0x5fcb6fab3ad6faec, 0x6c44198c4a475817
			};
		}

		static void DefineH0Sha512_224() {
			// These eight 64-bit words are obtained from GenerateInitialHashSha512t(224)

			H0Sha512_224 = new Word64[]
			{
				0x8c3d37c819544da2, 0x73e1996689dcd4d6, 0x1dfab7ae32ff9c82, 0x679dd514582f9fcf,
				0x0f6d2b697bd44da8, 0x77e36f7304c48942, 0x3f9d85a86a1d36c8, 0x1112e6ad91d692a1
			};
		}

		static void DefineH0Sha512_256() {
			// These eight 64-bit words are obtained from GenerateInitialHashSha512t(256)

			H0Sha512_256 = new Word64[]
			{
				0x22312194fc2bf72c, 0x9f555fa3c84c64c2, 0x2393b86b6f53b151, 0x963877195940eabd,
				0x96283ee2a88effe3, 0xbe5e1e2553863992, 0x2b0199fc2c85b8aa, 0x0eb72ddc81c52ca2
			};
		}

		static void DefineH0Sha512_384() {
			// These eight 64-bit words are obtained by taking the first 64 bits of the 
			// fractional parts of the square roots of the ninth through sixteenth prime numbers.

			H0Sha512_384 = new Word64[]
			{
				0xcbbb9d5dc1059ed8, 0x629a292a367cd507, 0x9159015a3070dd17, 0x152fecd8f70e5939,
				0x67332667ffc00b31, 0x8eb44a8768581511, 0xdb0c2e0d64f98fa7, 0x47b5481dbefa4fa4
			};
		}

		static void DefineH0Sha512_512() {
			// These eight 64-bit words are obtained by taking the first 64 bits of the 
			// fractional parts of the square roots of the first eight prime numbers.

			H0Sha512_512 = new Word64[]
			{
				0x6a09e667f3bcc908, 0xbb67ae8584caa73b, 0x3c6ef372fe94f82b, 0xa54ff53a5f1d36f1,
				0x510e527fade682d1, 0x9b05688c2b3e6c1f, 0x1f83d9abfb41bd6b, 0x5be0cd19137e2179
			};
		}
		#endregion

	}

	// Helper Classes
	class Block512 {
		// A Block512 consists of an array of 16 elements of type Word32.
		public Word32[] words;

		public Block512(Word32[] words) {
			if (words.Length == 16) {
				this.words = words;
			} else {
				Console.WriteLine("ERROR: A block must be 16 words");
				this.words = null;
			}
		}
	}

	class Block1024 {
		// A Block1024 consists of an array of 16 elements of type Word64.
		public Word64[] words;

		public Block1024(Word64[] words) {
			if (words.Length == 16) {
				this.words = words;
			} else {
				Console.WriteLine("ERROR: A block must be 16 words");
				this.words = null;
			}
		}
	}


	static class ShaUtilities {
		#region Functions to convert between byte arrays and Word64 arrays.
		public static byte[] StringToByteArray(string plaintext) {
			char[] c = plaintext.ToCharArray();
			int numberBytes = plaintext.Length;
			byte[] b = new byte[numberBytes];

			for (int i = 0; i < numberBytes; i++) {
				b[i] = Convert.ToByte(c[i]);
			}

			return b;
		}

		// Returns an array of 4 bytes.
		public static byte[] Word32ToByteArray(Word32 x) {
			byte[] b = BitConverter.GetBytes(x);
			Array.Reverse(b);
			return b;
		}

		// Returns an array of 8 bytes.
		public static byte[] Word64ToByteArray(Word64 x) {
			byte[] b = BitConverter.GetBytes(x);
			Array.Reverse(b);
			return b;
		}

		public static byte[] Word64ArrayToByteArray(Word64[] words) {
			List<byte> b = new List<byte>();

			for (int i = 0; i < words.Length; i++) {
				b.AddRange(Word64ToByteArray(words[i]));
			}

			return b.ToArray();
		}

		public static Word32 ByteArrayToWord32(byte[] B, int startIndex) {
			// We assume: 0 <= startIndex < B. Length, and startIndex + 4 <= B.Length

			Word32 c = 256;
			Word32 output = 0;

			for (int i = startIndex; i < startIndex + 4; i++) {
				output = output * c + (Word32)B[i];
			}

			return output;
		}

		public static Word64 ByteArrayToWord64(byte[] B, int startIndex) {
			// We assume: 0 <= startIndex < B. Length, and startIndex + 8 <= B.Length
			Word64 c = 256;
			Word64 output = 0;

			for (int i = startIndex; i < startIndex + 8; i++) {
				output = output * c + B[i];
			}

			return output;
		}

		public static Word64[] ByteArrayToWord64Array(byte[] B) {
			// We assume B is not null, is not empty and number elements is divisible by 8
			int numberWords = B.Length / 8; // 8 bytes for each Word32
			Word64[] word64Array = new Word64[numberWords];

			for (int i = 0; i < numberWords; i++) {
				word64Array[i] = ByteArrayToWord64(B, 8 * i);
			}

			return word64Array;
		}
		#endregion


		#region To string methods
		public static string ByteToBinaryString(byte b) {
			string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
			return binaryString.Substring(0, 4) + "_" + binaryString.Substring(4, 4);
		}

		public static string ByteArrayToBinaryString(byte[] x) {
			string binaryString = "";

			for (int i = 0; i < x.Length; i++) {
				binaryString += ByteToBinaryString(x[i]);

				if (i < x.Length - 1) {
					binaryString += "  ";
				}
			}

			return binaryString;
		}

		public static string ByteToHexString(byte b) {
			return Convert.ToString(b, 16).PadLeft(2, '0');
		}

		public static string ByteArrayToHexString(byte[] a) {
			string hexString = "";

			for (int i = 0; i < a.Length; i++) {
				hexString += ByteToHexString(a[i]);
			}

			return hexString;
		}
		#endregion
	}
}