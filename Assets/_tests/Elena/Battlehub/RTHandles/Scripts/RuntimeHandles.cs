using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.RTHandles
{
    public enum RuntimeHandleAxis
    {
        None,
        X,
        Y,
        Z
    }

    public static class RuntimeHandles 
    {
        public static readonly Color32 XColor = new Color32(187, 70, 45, 255);
        public static readonly Color32 YColor = new Color32(139, 206, 74, 255);
        public static readonly Color32 ZColor = new Color32(55, 115, 244, 255);
        public static readonly Color32 SelectionColor = new Color32(239, 238, 64, 255);
        private static readonly Mesh Arrows;
        private static readonly Mesh ArrowsY;
        private static readonly Mesh ArrowsX;
        private static readonly Mesh ArrowsZ;
        
        private static readonly Material ArrowsMaterial;
        private static readonly Material LinesMaterial;
        
        static RuntimeHandles()
        {
            LinesMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
            LinesMaterial.color = Color.white;

            ArrowsMaterial = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
            ArrowsMaterial.color = Color.white;

            Mesh selectionArrowMesh = CreateConeMesh(SelectionColor);

            CombineInstance yArrow = new CombineInstance();
            yArrow.mesh = selectionArrowMesh;
            yArrow.transform = Matrix4x4.TRS(Vector3.up, Quaternion.identity, Vector3.one);
            ArrowsY = new Mesh();
            ArrowsY.CombineMeshes(new[] { yArrow }, true);
            ArrowsY.RecalculateNormals();

            CombineInstance xArrow = new CombineInstance();
            xArrow.mesh = selectionArrowMesh;
            xArrow.transform = Matrix4x4.TRS(Vector3.right, Quaternion.AngleAxis(-90, Vector3.forward), Vector3.one);
            ArrowsX = new Mesh();
            ArrowsX.CombineMeshes(new[] { xArrow }, true);
            ArrowsX.RecalculateNormals();

            CombineInstance zArrow = new CombineInstance();
            zArrow.mesh = selectionArrowMesh;
            zArrow.transform = Matrix4x4.TRS(Vector3.forward, Quaternion.AngleAxis(90, Vector3.right), Vector3.one);
            ArrowsZ = new Mesh();
            ArrowsZ.CombineMeshes(new[] { zArrow }, true);
            ArrowsZ.RecalculateNormals();

            yArrow.mesh = CreateConeMesh(YColor);
            xArrow.mesh = CreateConeMesh(XColor);
            zArrow.mesh = CreateConeMesh(ZColor);
            Arrows = new Mesh();
            Arrows.CombineMeshes(new[] { yArrow, xArrow, zArrow }, true);
            Arrows.RecalculateNormals();

      
        }

        private static Mesh CreateConeMesh(Color color)
        {
            int segmentsCount = 12;
            float size = 1.0f / 5;

            Vector3[] vertices = new Vector3[segmentsCount * 3 + 1];
            int[] triangles = new int[segmentsCount * 6];
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i] = color;
            }

            float radius = size / 2.6f;
            float height = size;
            float deltaAngle = Mathf.PI * 2.0f / segmentsCount;

            float y = -height;

            vertices[vertices.Length - 1] = new Vector3(0, -height, 0);
            for (int i = 0; i < segmentsCount; i++)
            {
                float angle = i * deltaAngle;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices[i] = new Vector3(x, y, z);
                vertices[segmentsCount + i] = new Vector3(0, 0.01f, 0);
                vertices[2 * segmentsCount + i] = vertices[i];
            }

            for(int i = 0; i < segmentsCount; i++)
            {
                triangles[i * 6] = i;
                triangles[i * 6 + 1] = segmentsCount + i;
                triangles[i * 6 + 2] = (i + 1) % segmentsCount;

                triangles[i * 6 + 3] = vertices.Length - 1;
                triangles[i * 6 + 4] = 2 * segmentsCount + i;
                triangles[i * 6 + 5] = 2 * segmentsCount + (i + 1) % segmentsCount;
            }

            Mesh cone = new Mesh();
            cone.name = "Cone";
            cone.vertices = vertices;
            cone.triangles = triangles;
            cone.colors = colors;

            return cone;
        }

    
        public static float GetScreenScale(Vector3 position, Camera camera)
        {
            float h = camera.pixelHeight;
            if (camera.orthographic)
            {
                return camera.orthographicSize * 2f / h * 90;
            }

            
            Transform transform = camera.transform;
            float distance = Vector3.Dot(position - transform.position, transform.forward);
            float scale = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            return scale / h * 90;
        }

        public static void DoPositionHandle(Vector3 position, Quaternion rotation, RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None)
        {
            float scale = GetScreenScale(position, Camera.current);

            Matrix4x4 transform = Matrix4x4.TRS(position, rotation, new Vector3(scale, scale, scale));

            LinesMaterial.SetPass(0);

            GL.Begin(GL.LINES);
               
            Vector3 x = Vector3.right;
            Vector3 y = Vector3.up;
            Vector3 z = Vector3.forward;

            x = transform.MultiplyVector(x);
            y = transform.MultiplyVector(y);
            z = transform.MultiplyVector(z);

            GL.Color(selectedAxis != RuntimeHandleAxis.X ? XColor : SelectionColor);
            GL.Vertex(position);
            GL.Vertex(position + x);
            GL.Color(selectedAxis != RuntimeHandleAxis.Y ? YColor : SelectionColor);
            GL.Vertex(position);
            GL.Vertex(position + y);
            GL.Color(selectedAxis != RuntimeHandleAxis.Z ? ZColor : SelectionColor);
            GL.Vertex(position);
            GL.Vertex(position + z);

            GL.End();

            ArrowsMaterial.SetPass(0);
            Graphics.DrawMeshNow(Arrows, transform);
            if(selectedAxis == RuntimeHandleAxis.X)
            {
                Graphics.DrawMeshNow(ArrowsX, transform);
            }
            else if (selectedAxis == RuntimeHandleAxis.Y)
            {
                Graphics.DrawMeshNow(ArrowsY, transform);
            }
            else if (selectedAxis == RuntimeHandleAxis.Z)
            {
                Graphics.DrawMeshNow(ArrowsZ, transform);
            }

        }

        private static void DoRotationHandle(Quaternion rotation, Vector3 position)
        {
            
        }

        private static void DoScaleHandle(Vector3 scale, Vector3 position, Quaternion rotation, float size)
        {

        }
    }

}
