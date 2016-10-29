using UnityEngine;
using System.Collections.Generic;

namespace Battlehub.Wire2
{
    public class WireGen 
    {
        private int m_sectorsCount;
        private int m_sliceCount;
        private float m_radius;
        private float m_length;

        private float m_x;
        private float m_y;
        private float m_z;

        public Mesh Generate(int sectorsCount, int sliceCount, float radius, float length)
        {
            m_sectorsCount = sectorsCount;
            m_sliceCount = sliceCount;
            m_radius = radius;
            m_length = length;

            Mesh mesh = new Mesh();

            mesh.name = "Wire";
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            
            CreateSlices(vertices, uv);
            int capVerticesOffset = vertices.Count;
            CreateCaps(vertices, uv);

            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();
            mesh.triangles = CreateTriangles(vertices, capVerticesOffset);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void SetSlice(int slice)
        {
            float s = slice;
            m_z = m_length * (s / m_sliceCount);
        }

        private void SetSector(int sector)
        {
            float angle = sector * 2.0f * Mathf.PI / m_sectorsCount;
            angle = NormalizeAngle(angle) + Mathf.PI / 2.0f;

            m_x = m_radius * Mathf.Cos(angle);
            m_y = m_radius * Mathf.Sin(angle);
        }

        private float NormalizeAngle(float angle)
        {
            return (2.0f * Mathf.PI + angle % (2.0f * Mathf.PI)) % (2.0f * Mathf.PI);
        }

        private void CreateCaps(List<Vector3> vertices, List<Vector2> uv)
        {
            SetSlice(0);
            for (int sector = 0; sector < m_sectorsCount; ++sector)
            {
                SetSector(sector);
                Vector3 vertex = new Vector3(m_x, m_y, m_z);
                vertices.Add(vertex);
                uv.Add(new Vector2(0.5f + 0.5f * vertex.x / m_radius, 0.5f + 0.5f * vertex.y / m_radius));
            }

            SetSlice(m_sliceCount);
            for (int sector = 0; sector < m_sectorsCount; ++sector)
            {
                SetSector(sector);
                Vector3 vertex = new Vector3(m_x, m_y, m_z);
                vertices.Add(vertex);
                uv.Add(new Vector2(0.5f + 0.5f * vertex.x / m_radius, 0.5f + 0.5f * vertex.y / m_radius));
            }

            vertices.Add(new Vector3(0, 0, 0));
            uv.Add(new Vector2(0.5f, 0.5f));

            vertices.Add(new Vector3(0, 0, m_length));
            uv.Add(new Vector2(0.5f, 0.5f));
        }

        private int[] CreateTriangles(List<Vector3> vertices, int capVerticesOffset)
        {
            int[] triangles = new int[m_sliceCount * m_sectorsCount * 6 + m_sectorsCount * 6];
            int index = 0;
            for (int i = 0; i < m_sliceCount; ++i)
            {
                int offset = i * m_sectorsCount;
                for (int j = 0; j < m_sectorsCount; j++)
                {
                    if(j % 2 == 0)
                    {
                        triangles[index] = (offset + ((j + 1) % m_sectorsCount));
                        index++;
                        triangles[index] = (offset + j + m_sectorsCount);
                        index++;
                        triangles[index] = (offset + j);
                        index++;

                        triangles[index] = (offset + j + m_sectorsCount);
                        index++;
                        triangles[index] = (offset + ((j + 1) % m_sectorsCount));
                        index++;
                        triangles[index] = (offset + ((j + 1) % m_sectorsCount) + m_sectorsCount);
                        index++;
                    }
                    else
                    {
                        triangles[index] = (offset + j);
                        index++;
                        triangles[index] = (offset + ((j + 1) % m_sectorsCount) + m_sectorsCount);
                        index++;
                        triangles[index] = (offset + j + m_sectorsCount);
                        index++;

                        triangles[index] = (offset + ((j + 1) % m_sectorsCount) + m_sectorsCount);
                        index++;
                        triangles[index] = (offset + j);
                        index++;
                        triangles[index] = (offset + ((j + 1) % m_sectorsCount));
                        index++;
                    }
                   
                }
            }
            for (int i = 0; i < m_sectorsCount; ++i)
            {
                triangles[index] = vertices.Count - 2;
                index++;
                if (i == m_sectorsCount - 1)
                {
                    triangles[index] = capVerticesOffset;
                }
                else
                {
                    triangles[index] = capVerticesOffset + i + 1;
                }
                index++;
                triangles[index] = capVerticesOffset + i;
                index++;
            }
            capVerticesOffset += m_sectorsCount;
            for (int i = 0; i < m_sectorsCount; ++i)
            {
                triangles[index] = vertices.Count - 1;
                index++;
                triangles[index] = capVerticesOffset + i;
                index++;
                if (i == m_sectorsCount - 1)
                {
                    triangles[index] = capVerticesOffset;
                }
                else
                {
                    triangles[index] = capVerticesOffset + i + 1;
                }
                index++;
            }
            return triangles;
        }

        private void CreateSlices(List<Vector3> vertices, List<Vector2> uv)
        {
            for (int slice = 0; slice <= m_sliceCount; ++slice)
            {
                SetSlice(slice);
                for (int sector = 0; sector < m_sectorsCount; ++sector)
                {
                    SetSector(sector);

                    Vector3 vertex = new Vector3(m_x, m_y, m_z);
                    vertices.Add(vertex);

                    uv.Add(new Vector2(0.5f + 0.5f * vertex.x / m_radius, 0.5f + 0.5f * vertex.y / m_radius));
                }
            }
        }

    }

}
