using System.Collections.Generic;
using INUlib.Gameplay.Grids.Algorithms.Interfaces;
using INUlib.Core.Math;

namespace INUlib.Gameplay.Grids.Interfaces
{
    public interface IGridEntity // Entities that can occupy a WorldGrid
    {
        int2 EntitySize { get; set; }
        float3 WorldPos { get; set; }
        List<IPath> OccupyingCels { get; set; } // list of cels the structure is occupying
    }
}
