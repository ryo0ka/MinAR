using System;
using System.Collections.Generic;
using System.Threading;

namespace BooAR.Voxel
{
	// https://0fps.net/2012/06/30/meshing-in-a-minecraft-game
	public class VoxelQuadBuilder
	{
		enum Faces
		{
			Front,
			Back,
			None
		}

		readonly List<Quad> _quads;
		readonly int _length;
		readonly Faces[] _faces;
		readonly Blocks[] _blocks;
		readonly Func<Vector3i, Lookup?> _lookup;
		CancellationToken _canceller;

		public VoxelQuadBuilder(int initCapacity, int length, Func<Vector3i, Lookup?> lookup)
		{
			_quads = new List<Quad>(initCapacity);
			_length = length;
			_faces = new Faces[_length * _length];
			_blocks = new Blocks[_length * _length];
			_lookup = lookup;
		}

		public IEnumerable<Quad> Build(CancellationToken canceller)
		{
			_quads.Clear();
			_canceller = canceller;

			for (int dAxis = 0; dAxis < 3; dAxis++) // D (depth) axis index
			for (int dPos = 0; dPos <= _length; dPos++) // depth position
			{
				_canceller.ThrowIfCancellationRequested();

				CalculateFaces(dAxis, dPos);
				GenerateQuads(dAxis, dPos);
			}

			return _quads;
		}

		void CalculateFaces(int dAxis, int dPos)
		{
			int uAxis = (dAxis + 1) % 3; // U (width) axis index
			int vAxis = (dAxis + 2) % 3; // V (height) axis index

			int faceIndex = 0;

			// Compute faces of this depth
			for (int vPos = 0; vPos < _length; vPos++)
			for (int uPos = 0; uPos < _length; uPos++)
			{
				_canceller.ThrowIfCancellationRequested();

				// voxel at this UV position
				Vector3i thisVoxPos = new Vector3i();
				thisVoxPos[dAxis] = dPos;
				thisVoxPos[uAxis] = uPos;
				thisVoxPos[vAxis] = vPos;

				// voxel in the previous depth
				Vector3i lastVoxPos = new Vector3i();
				lastVoxPos[dAxis] = dPos - 1;
				lastVoxPos[uAxis] = uPos;
				lastVoxPos[vAxis] = vPos;

				Faces outbound;
				if (dPos == 0)
				{
					outbound = Faces.Front;
				}
				else if (dPos == _length)
				{
					outbound = Faces.Back;
				}
				else
				{
					outbound = Faces.None;
				}

				CalculateFace(
					outbound,
					_lookup(lastVoxPos),
					_lookup(thisVoxPos),
					out Faces face,
					out Blocks block);

				_faces[faceIndex] = face;
				_blocks[faceIndex] = block;

				faceIndex += 1;

				//Debug.Log($"({cd}, {cu}, {cv}) -- {a}, {b}, {c}, {d} -> {e}; {co}, {q}");
			}
		}

		void CalculateFace(
			Faces outbound,
			Lookup? lastVoxSrc,
			Lookup? thisVoxSrc,
			out Faces face,
			out Blocks block)
		{
			if (lastVoxSrc.HasValue && thisVoxSrc.HasValue)
			{
				Lookup lastVox = lastVoxSrc.Value;
				Lookup thisVox = thisVoxSrc.Value;

				if (lastVox.IsOpaque && thisVox.IsOpaque)
				{
					face = Faces.None;
					block = 0;
				}
				else if (!lastVox.IsOpaque && thisVox.IsOpaque)
				{
					if (outbound == Faces.Back)
					{
						// Exposed voxel in the neiboring chunk is not empty.
						// Do not render the face, because it's done by that chunk.
						face = Faces.None;
						block = 0;
					}
					else
					{
						face = Faces.Front;
						block = thisVox.Block;
					}
				}
				else if (lastVox.IsOpaque && !thisVox.IsOpaque)
				{
					if (outbound == Faces.Front)
					{
						// Exposed voxel in the neiboring chunk is not empty.
						// Do not render the face, because it's done by that chunk.
						face = Faces.None;
						block = 0;
					}
					else
					{
						face = Faces.Back;
						block = lastVox.Block;
					}
				}
				else
				{
					face = Faces.None;
					block = 0;
				}
			}
			else
			{
				// border is void (either sealed or invalid)
				face = Faces.None;
				block = 0;
			}
		}

		void GenerateQuads(int dAxis, int dPos)
		{
			int uAxis = (dAxis + 1) % 3; // U (width) axis index
			int vAxis = (dAxis + 2) % 3; // V (height) axis index

			int faceIndex = 0;

			// Generate quads in this depth
			for (int vPos = 0; vPos < _length; vPos++)
			for (int uPos = 0; uPos < _length;)
			{
				_canceller.ThrowIfCancellationRequested();

				//Debug.Log($"{ad}, ({cd}, {cu}, {cv}) -- {mask[n]}");

				Faces face = _faces[faceIndex];
				if (face != Faces.None)
				{
					Blocks block = _blocks[faceIndex];

					int uLen = Width(uPos, faceIndex, face, block);
					int vLen = Height(vPos, uLen, faceIndex, face, block);

					Vector3i uDelta = new Vector3i();
					uDelta[uAxis] = uLen;

					Vector3i vDelta = new Vector3i();
					vDelta[vAxis] = vLen;

					Vector3i aCoord = new Vector3i();
					aCoord[dAxis] = dPos;
					aCoord[uAxis] = uPos;
					aCoord[vAxis] = vPos;

					Vector3i bCoord = aCoord + uDelta;
					Vector3i cCoord = bCoord + vDelta;
					Vector3i dCoord = aCoord + vDelta;

					Quad quad = (face == Faces.Back)
						? new Quad(aCoord, bCoord, cCoord, dCoord, block)
						: new Quad(dCoord, cCoord, bCoord, aCoord, block); // reverse order

					_quads.Add(quad);

					// Initialize faces of this quad for the next run
					for (int u = 0; u < uLen; u++)
					for (int v = 0; v < vLen; v++)
					{
						_canceller.ThrowIfCancellationRequested();

						_faces[faceIndex + u + v * _length] = Faces.None;
					}

					// Move on to the next point
					uPos += uLen;
					faceIndex += uLen;
				}
				else
				{
					// Move on to the next point
					uPos += 1;
					faceIndex += 1;
				}
			}
		}

		int Width(int initPosition, int faceIndex, Faces face, Blocks block)
		{
			// Compute mesh length in U axis from the initial point
			// expanding to the first wrong face or the end of volume
			int uLen = 1;
			while ((uLen + initPosition) < _length // until end of volume
			       && (uLen + faceIndex) < _faces.Length // until end of face array
			       && _faces[uLen + faceIndex] == face // until different direction
			       && _blocks[uLen + faceIndex] == block) // until different block
			{
				_canceller.ThrowIfCancellationRequested();

				uLen += 1;
			}

			return uLen;
		}

		int Height(int initPosition, int width, int faceIndex, Faces face, Blocks block)
		{
			// Compute mesh length in V axis from the initial point
			// expanding to the first wrong face or the end of volume
			int vLen;
			bool reachedWrongFace = false;
			for (vLen = 1; (initPosition + vLen) < _length; vLen++)
			{
				for (int u = 0; u < width; u++)
				{
					_canceller.ThrowIfCancellationRequested();

					int i = faceIndex + u + vLen * _length;
					if (_faces[i] != face || _blocks[i] != block)
					{
						// stop expanding when a wrong face is reached
						reachedWrongFace = true;
						break;
					}
				}

				if (reachedWrongFace) break;
			}

			return vLen;
		}
	}
}