using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace BooAR.Voxel
{
	public class BlocksSerializer
	{
		readonly int _length;

		public BlocksSerializer(int length)
		{
			_length = length;
		}

		public void Serialize(Array3<Blocks> blocks, Stream target)
		{
			using (GZipOutputStream s = new GZipOutputStream(target))
			{
				for (int i = 0; i < _length * _length * _length; i++)
				{
					s.WriteByte((byte) blocks[i]);
				}
			}
		}

		public void Deserialize(Array3<Blocks> blocks, Stream source)
		{
			using (GZipInputStream s = new GZipInputStream(source))
			{
				for (int i = 0; i < _length * _length * _length; i++)
				{
					blocks[i] = (Blocks) s.ReadByte();
				}
			}
		}
	}
}