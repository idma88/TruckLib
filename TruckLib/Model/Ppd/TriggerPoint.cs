﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Model.Ppd
{
    /// <summary>
    /// Represents an area that triggers an action, such as a rest area.
    /// </summary>
    public class TriggerPoint : IBinarySerializable
    {
        public uint TriggerId { get; set; }

        /// <summary>
        /// The trigger action.
        /// </summary>
        public Token Action { get; set; }

        public Vector3 Position { get; set; }

        /// <summary>
        /// Range of the trigger point or area of activation.
        /// <para>If SphereTrigger is true, this property defines the radius 
        /// around the trigger point. Otherwise, it defines the vertical range
        /// of the trigger area.</para>
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// How long the player has to be outside the trigger area until
        /// it can be activated again, in seconds.
        /// </summary>
        public float ResetDelay { get; set; }

        public float ResetDistance { get; set; }

        public int[] Neighbours { get; set; } = new int[2];

        protected FlagField Flags = new FlagField();

        /// <summary>
        /// Determines if the player has to activate the trigger action manually.
        /// </summary>
        public bool ManualActivation
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if this trigger point is a sphere trigger.
        /// In this case, this point should not be connected to any other trigger point.
        /// </summary>
        public bool SphereTrigger
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Determines if the trigger activates as soon as the player 
        /// touches the border.
        /// </summary>
        public bool PartialActivation
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Determines if this trigger point can only be activated once.
        /// </summary>
        public bool OneTimeActivation
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        public void Deserialize(BinaryReader r)
        {
            TriggerId = r.ReadUInt32();
            Action = r.ReadToken();
            Range = r.ReadSingle();
            ResetDelay = r.ReadSingle();
            ResetDistance = r.ReadSingle();
            Flags = new FlagField(r.ReadUInt32());
            Position = r.ReadVector3();
            for (int i = 0; i < Neighbours.Length; i++)
            {
                Neighbours[i] = r.ReadInt32();
            }
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
