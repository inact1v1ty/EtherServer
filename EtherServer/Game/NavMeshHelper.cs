using SharpNav;
using SharpNav.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace EtherServer.Game
{
    public class NavMeshHelper
    {
        public static NavMesh GetNavMesh()
        {
            var startPoint = new SharpNav.Geometry.Vector3(0f, -4.72f, 0f);
            //prepare the geometry from your mesh data
            var tris = TriangleEnumerable.FromIndexedVector3(
                new SharpNav.Geometry.Vector3[]
                {
                    new SharpNav.Geometry.Vector3(-100, 0, -100) + startPoint,
                    new SharpNav.Geometry.Vector3(0, 0, 200) + startPoint,
                    new SharpNav.Geometry.Vector3(100, 0, -100) + startPoint,
                    //new SharpNav.Geometry.Vector3(10, 0, -10) + startPoint
                },
                new int[]
                {
                    0,
                    1,
                    2,
                    //0,
                    //2,
                    //3
                },
                0, 1, 0, 1);

            //use the default generation settings
            var settings = NavMeshGenerationSettings.Default;
            settings.AgentHeight = 1.75f;
            settings.AgentRadius = 0.4f;
            settings.VertsPerPoly = 3;

            //generate the mesh
            var navMesh = NavMesh.Generate(tris, NavMeshGenerationSettings.Default);
            return navMesh;
        }
    }
}
