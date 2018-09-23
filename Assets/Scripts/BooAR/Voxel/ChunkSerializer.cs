using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace BooAR.Voxel
{
	public class ChunkSerializer
	{
		readonly int _length;

		public ChunkSerializer(int length)
		{
			_length = length;
		}

		public void Serialize(Array3<byte> blocks, Stream target)
		{
			using (GZipOutputStream s = new GZipOutputStream(target))
			{
				for (int i = 0; i < _length * _length * _length; i++)
				{
					s.WriteByte(blocks[i]);
				}
			}
		}

		public void Deserialize(Array3<byte> blocks, Stream source)
		{
			using (GZipInputStream s = new GZipInputStream(source))
			{
				for (int i = 0; i < _length * _length * _length; i++)
				{
					blocks[i] = (byte) s.ReadByte();
				}
			}
		}
	}
}