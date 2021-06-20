using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    public interface IColorDropMeshGenerator
    {
        Mesh GenerateParticleMesh(int scaleMultiplier, Vector3 vertCoord);
    }

    public class ColorDropMeshGenerator : MonoBehaviour, IColorDropMeshGenerator
    {
        /// <summary>
        /// Generate a scaled mesh for the particle.
        /// </summary>
        public Mesh GenerateParticleMesh(int scaleMultiplier, Vector3 vertCoord)
        {
            Mesh mesh = new Mesh();
            Vector3[] vert = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int meshSize = 2 * scaleMultiplier;
            int[] triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
            int triangleIndex = 0;

            for (int index = 0, x = 0; x < meshSize; x *= scaleMultiplier)
            {
                for (int y = 0; y < meshSize; index++, y *= scaleMultiplier)
                {
                    vert[index] = new Vector3(vertCoord.x + x, vertCoord.y + y, vertCoord.z);
                    uv[index] = new Vector2(x, y);

                    if (y < meshSize - 1 && x < meshSize - 1)
                    {
                        AddTriangle(triangles ,triangleIndex, index, index + meshSize, index + 1);
                        triangleIndex += 3;

                        AddTriangle(triangles, triangleIndex, index + 1, index + meshSize, index + meshSize + 1);
                        triangleIndex += 3;
                    }
                }
            }

            return mesh;
        }

        private void AddTriangle(int[] triangles, int triangleIndex, int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
        }
    }

}