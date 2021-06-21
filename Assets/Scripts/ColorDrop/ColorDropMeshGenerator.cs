using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorDrop
{
    public interface IColorDropMeshGenerator
    {
        //Mesh GenerateParticleMesh(int scaleMultiplier, Vector3 vertCoord);
    }

    public class ColorDropMeshGenerator : MonoBehaviour, IColorDropMeshGenerator
    {
        /// <summary>
        /// Generate a scaled mesh for the particle.
        /// </summary>
        /*public Mesh GenerateParticleMesh(int scaleMultiplier, Vector3 vertCoord)
        {
            int meshSize = 2;
            Mesh mesh = new Mesh();
            Vector3[] vert = new Vector3[meshSize * meshSize];
            Vector2[] uv = new Vector2[meshSize * meshSize];
            Vector3[] normals = new Vector3[meshSize * meshSize];
            int[] triangles = new int[(meshSize - 1) * (meshSize - 1) * 6];
            int triangleIndex = 0;

            for (int index = 0, row = 0; row < meshSize; row++)
            {
                for (int col = 0; col < meshSize; index++, col++)
                {
                    vert[index] = new Vector3(vertCoord.x + col, vertCoord.y + row, vertCoord.z);
                    uv[index] = new Vector2(col, row);
                    normals[index] = new Vector3(0, 0, -1);

                    if (col < meshSize - 1 && row < meshSize - 1)
                    {
                        AddTriangle(triangles ,triangleIndex, index, index + ((meshSize - 1) / 2), index + 1);
                        triangleIndex += 3;

                        AddTriangle(triangles, triangleIndex, index + 1, index + ((meshSize - 1) / 2), index + ((meshSize - 1) / 2) + 1);
                        triangleIndex += 3;
                    }
                }
            }

            mesh.vertices = vert;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();

            return mesh;
        }

        private void AddTriangle(int[] triangles, int triangleIndex, int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
        }*/
    }

}