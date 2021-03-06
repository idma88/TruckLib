﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class TriggerAction : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the action.
        /// </summary>
        public Token Name { get; set; }

        public List<float> NumParams { get; set; } = new List<float>();

        public List<string> StringParams { get; set; } = new List<string>();

        public List<Token> TargetTags { get; set; } = new List<Token>();

        public float TargetRange { get; set; }

        private const int typeStart = 0;
        private const int typeLength = 4; // presumably
        public ActionType Type 
        {
            get => (ActionType)actionFlags.GetBitString(typeStart, typeLength);
            set
            {
                actionFlags.SetBitString(typeStart, typeLength, (uint)value);
            }
        }

        private FlagField actionFlags = new FlagField();

        public void Deserialize(BinaryReader r)
        {
            Name = r.ReadToken();

            var numParamCount = r.ReadUInt32();
            // if there are no custom params of any kind, 
            // this value is 0xFFFFFFFF.
            if (numParamCount == uint.MaxValue)
                return;

            for (int i = 0; i < numParamCount; i++)
            {
                NumParams.Add(r.ReadSingle());
            }

            var stringParamCount = r.ReadUInt32();
            for (int i = 0; i < stringParamCount; i++)
            {
                var strLen = (int)r.ReadUInt64();
                var strBytes = r.ReadBytes(strLen);
                var str = Encoding.Default.GetString(strBytes);
                StringParams.Add(str);
            }

            var targetTagsCount = r.ReadUInt32();
            for (int i = 0; i < targetTagsCount; i++)
            {
                TargetTags.Add(r.ReadToken());
            }


            TargetRange = r.ReadSingle();
            actionFlags = new FlagField(r.ReadUInt32());
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Name);

            w.Write(NumParams.Count);
            foreach (var param in NumParams)
            {
                w.Write(param);
            }

            w.Write(StringParams.Count);
            foreach (var param in StringParams)
            {
                w.Write((ulong)param.Length);
                w.Write(Encoding.Default.GetBytes(param));
            }

            w.Write(TargetTags.Count);
            foreach (var tag in TargetTags)
            {
                w.Write(tag);
            }

            w.Write(TargetRange);
            w.Write(actionFlags.Bits);
        }
    }

    public enum ActionType
    {
        Default = 0,
        Condition = 1,
        Fallback = 2,
        Mandatory = 3,
        ConditionRetry = 4
    }
}
