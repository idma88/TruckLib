﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Holds terrain and vegetation data for one side of a standalone terrain segment.
    /// </summary>
    public class TerrainSide
    {
        /// <summary>
        /// The edge model on this side.
        /// </summary>
        public Token Edge { get; set; }

        /// <summary>
        /// The look of the edge model.
        /// </summary>
        public Token EdgeLook { get; set; }

        /// <summary>
        /// The terrain on this side.
        /// </summary>
        public RoadTerrain Terrain { get; set; } = new RoadTerrain();

        /// <summary>
        /// The vegetation on this side.
        /// </summary>
        public RoadVegetation[] Vegetation { get; set; }

        /// <summary>
        /// Determines if vegetation has collision.
        /// </summary>
        public bool VegetationCollision = false;

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation = true;

        public float NoDetailVegetationFrom { get; set; }

        public float NoDetailVegetationTo { get; set; }

        public float UVRotation { get; set; }

        public TerrainSide()
        {
            const int vegetationCount = 3;
            Vegetation = (new RoadVegetation[vegetationCount])
                .Select(h => new RoadVegetation()).ToArray();
        }
    }
}
