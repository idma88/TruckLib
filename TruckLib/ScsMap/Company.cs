﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Additional data for company prefabs.
    /// </summary>
    public class Company : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.Company;

        public Token CompanyName { get; set; }

        public Token CityName { get; set; }

        /// <summary>
        /// List of easy difficulty (15 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsEasy { get; set; }

        /// <summary>
        /// List of medium difficulty (40 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsMedium { get; set; }

        /// <summary>
        /// List of hard difficulty (90 XP) parking spots.
        /// </summary>
        public List<Node> UnloadPointsHard { get; set; }

        /// <summary>
        /// List of trailer spawn points.
        /// </summary>
        public List<Node> TrailerSpawnPoints { get; set; }

        public List<Node> Unknown1 { get; set; }

        public List<Node> LongTrailerSpawnPoints { get; set; } 

        public Company() : base() { }

        internal Company(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            UnloadPointsEasy = new List<Node>();
            UnloadPointsMedium = new List<Node>();
            UnloadPointsHard = new List<Node>();
            TrailerSpawnPoints = new List<Node>();
            Unknown1 = new List<Node>();
            LongTrailerSpawnPoints = new List<Node>();
        }

        public static Company Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<Company>(map, parent, position);
        }

        public override void Move(Vector3 newPos)
        {
            throw new NotImplementedException();
        }

        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);

            var allNodes = UnloadPointsEasy.Concat(UnloadPointsMedium)
                .Concat(UnloadPointsHard).Concat(TrailerSpawnPoints)
                .Concat(Unknown1).Concat(LongTrailerSpawnPoints);

            foreach (var node in allNodes)
            {
                node.Move(node.Position + translation);
            }
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            base.UpdateNodeReferences(allNodes);

            ResolveNodeReferences(UnloadPointsEasy, allNodes);
            ResolveNodeReferences(UnloadPointsMedium, allNodes);
            ResolveNodeReferences(UnloadPointsHard, allNodes);
            ResolveNodeReferences(TrailerSpawnPoints, allNodes);
            ResolveNodeReferences(Unknown1, allNodes);
            ResolveNodeReferences(LongTrailerSpawnPoints, allNodes);
        }
    }
}
