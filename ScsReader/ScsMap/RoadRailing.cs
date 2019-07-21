﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Represents one railing model of a road.
    /// </summary>
    public struct RoadRailing
    {
        /// <summary>
        /// The name of the model.
        /// </summary>
        public Token Model;

        private float offset;
        public float Offset
        {
            get => offset;
            set => offset = Utils.SetIfInRange(value, -327.66f, 327.66f);
        }
    }

}