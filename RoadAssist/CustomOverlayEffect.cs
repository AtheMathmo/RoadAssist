using ColossalFramework;
using ColossalFramework.Math;
using System;
using UnityEngine;
namespace RoadAssist
{
    public class CustomOverlayEffect : MonoBehaviour
    {
        public ToolController m_toolController;
        public Shader m_overlayShader;
        public Shader m_shapeShader;
        public Shader m_shapeShaderBlend;

        // Edited to allow rotation
        public static void DrawEffect(RenderManager.CameraInfo cameraInfo, Material material, int pass, Bounds bounds)
        {
            Mesh boxMesh = GridTrigger.BoxMesh;
            if (bounds.Intersects(cameraInfo.m_nearBounds))
            { 
                if (material.SetPass(pass))
                {
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(cameraInfo.m_position + cameraInfo.m_forward * (cameraInfo.m_near + 1f), cameraInfo.m_rotation, new Vector3(100f, 100f, 1f));
                    Graphics.DrawMeshNow(boxMesh, matrix);
                }
            }
            else if (material.SetPass(pass))
            {
                Matrix4x4 matrix2 = default(Matrix4x4);
                matrix2.SetTRS(bounds.center, GridRenderManager.Rotation, bounds.size);
                Graphics.DrawMeshNow(boxMesh, matrix2);
            }
        }

        public static Mesh CreateBoxMesh()
        {
            Vector3[] array = new Vector3[8];
            int[] triangles = new int[36];
            int num = 0;
            int num2 = 0;
            array[num++] = new Vector3(-0.5f, -0.5f, -0.5f);
            array[num++] = new Vector3(0.5f, -0.5f, -0.5f);
            array[num++] = new Vector3(-0.5f, 0.5f, -0.5f);
            array[num++] = new Vector3(0.5f, 0.5f, -0.5f);
            array[num++] = new Vector3(-0.5f, -0.5f, 0.5f);
            array[num++] = new Vector3(0.5f, -0.5f, 0.5f);
            array[num++] = new Vector3(-0.5f, 0.5f, 0.5f);
            array[num++] = new Vector3(0.5f, 0.5f, 0.5f);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 0, 2, 3, 1);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 4, 5, 7, 6);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 2, 6, 7, 3);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 0, 1, 5, 4);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 4, 6, 2, 0);
            CustomOverlayEffect.CreateQuad(triangles, ref num2, 5, 1, 3, 7);
            Mesh boxMesh = new Mesh();
            boxMesh.hideFlags = HideFlags.DontSave;
            boxMesh.vertices = array;
            boxMesh.triangles = triangles;

            return boxMesh;
        }

        private static void CreateQuad(int[] triangles, ref int index, int a, int b, int c, int d)
        {
            triangles[index++] = a;
            triangles[index++] = b;
            triangles[index++] = d;
            triangles[index++] = d;
            triangles[index++] = b;
            triangles[index++] = c;
        }
        
    }
}
