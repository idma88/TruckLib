﻿using TruckLib.ScsMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class MiscExtensions
    {
        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj) where T : IBinarySerializable, new()
        {
            T cloned = new T();
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                obj.WriteToStream(writer);
                if (obj is IDataPart) (obj as IDataPart).WriteDataPart(writer);
                stream.Position = 0;

                using (var reader = new BinaryReader(stream))
                {
                    cloned.ReadFromStream(reader);
                    if (obj is IDataPart) (obj as IDataPart).ReadDataPart(reader);
                }
            }

            // don't allow duplicate uids
            if(cloned is IMapObject mapObject)
            {
                mapObject.Uid = Utils.GenerateUuid();
            }

            // TODO: also deepclone certain referenced
            // items such as nodes
            // TODO: then add the cloned item to the map
          
            return cloned;
        }

        /// <summary>
        /// Converts a quaternion to Euler angles.
        /// </summary>
        /// <param name="q"></param>
        /// <returns>Euler angles in radians.</returns>
        public static Vector3 ToEuler(this Quaternion q)
        {
            // via https://stackoverflow.com/a/56055813

            double x, y, z;

            // if the input quaternion is normalized, this is exactly one. 
            // Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
            float unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

            // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
            float test = q.X * q.W - q.Y * q.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                x = Math.PI / 2;
                y = 2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                x = -Math.PI / 2;
                y = -2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else // no singularity - this is the majority of cases
            {
                x = Math.Asin(2f * (q.W * q.X - q.Y * q.Z));
                y = Math.Atan2(2f * q.W * q.Y + 2f *  q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
                z = Math.Atan2(2f * q.W * q.Z + 2f *  q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
            }
   
            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Converts a quaternion to Euler angles in degrees.
        /// </summary>
        /// <param name="q"></param>
        /// <returns>Euler angles in degrees.</returns>
        public static Vector3 ToEulerDeg(this Quaternion q)
        {
            var rad = q.ToEuler();
            rad.X = (float)(rad.X * MathEx.RadToDeg);
            rad.Y = (float)(rad.Y * MathEx.RadToDeg);
            rad.Z = (float)(rad.Z * MathEx.RadToDeg);
            return rad;
        }

        /// <summary>
        /// Converts bool to byte.
        /// </summary>
        /// <param name="b">The bool.</param>
        /// <returns>1 if true, 0 if false.</returns>
        public static byte ToByte(this bool b)
        {
            return b ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Converts a bool array with length 8 to byte.
        /// </summary>
        /// <param name="bools"></param>
        /// <returns></returns>
        public static byte ToByte(this bool[] bools)
        {
            if (bools.Length < 8)
            {
                throw new InvalidCastException("Can't convert a bool[] of this length to a single byte.");
            }
            var arr = new BitArray(bools);
            var bytes = new byte[1];
            arr.CopyTo(bytes, 0);
            return bytes[0];
        }

    }
}
