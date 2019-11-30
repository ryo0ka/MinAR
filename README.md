MinAR
===

Yet another Minecraft-like game powered by ARKit.

You will spawn deep under the ground and sealed all sides by thick minerals. Dig around as you walk around in real life, and connect with other players to build a giant cave network in your neighborhood.

The game logic at the time of writing simulates the Minecraft Alpha version and allows player to:

- Dig blocks and store them in inventory;
- Place them wherever.

Development was pending for the "Collaborative Sessions" feature in ARKit3 and will be restarting soon.

Tech Ovewview
---

The game world is pretty much equivalent to Minecraft: 3D grids filled with blocks. Meshing is done with the ["Greedy Meshing" algorithm](https://github.com/ryo0ka/MinAR/blob/master/Assets/Scripts/BooAR/Voxel/VoxelQuadBuilder.cs) ([reference](https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/)) and the block distribution with good old Perlin noise.

Blocks are stored in Minecraft-like ["Chunk" format](https://github.com/ryo0ka/MinAR/blob/master/Assets/Scripts/BooAR/Voxel/Chunk.cs) and rendered only if visible to player for optimization. Chunks are also used to serialize the blocks for recovery, in conjunction with [ARKit relocalization (persistence) feature](https://github.com/ryo0ka/MinAR/blob/master/Assets/Scripts/BooAR/ARs/ArKitWorldPersistence.cs).

Block surface textures are packed in a single texture file and the [shader](https://github.com/ryo0ka/MinAR/blob/master/Assets/Resources/Blocks/Block.shader) will read from the specific part of the texture per block type, to minize draw calls.

Dependency to third-party libraries inludes:

- Zenject ([usage](https://github.com/ryo0ka/MinAR/blob/master/Assets/Scripts/BooAR/AppInstaller.cs)) for better testing/mocking feasibility;
- UniRX ([usage](https://github.com/ryo0ka/MinAR/blob/master/Assets/Scripts/BooAR/Games/GameController.cs)) for better input handling.
