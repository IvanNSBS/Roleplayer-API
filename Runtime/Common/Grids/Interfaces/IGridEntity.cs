﻿using UnityEngine;
using System.Collections.Generic;
using INUlib.Common.Grids.Algorithms.Interfaces;

namespace INUlib.Common.Grids.Interfaces
{
    public interface IGridEntity // Entities that can occupy a WorldGrid
    {
        Vector2Int EntitySize { get; set; }
        Vector3 WorldPos { get; set; }
        List<IPath> OccupyingCels { get; set; } // list of cels the structure is occupying
    }
}