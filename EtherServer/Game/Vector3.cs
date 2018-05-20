using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator SharpNav.Geometry.Vector3(Vector3 v)
        {
            return new SharpNav.Geometry.Vector3(v.x, v.y, v.z);
        }

        public static implicit operator Vector3(SharpNav.Geometry.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
