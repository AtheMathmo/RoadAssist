using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ColossalFramework.IO;
using ColossalFramework.Math;
using ColossalFramework.Steamworks;
using ColossalFramework.Threading;
using ICities;
using UnityEngine;

namespace RoadAssist
{
    class Utils
    {
        public static void SetBuildingRender(bool toggle)
        {
            // Toggles building renders and fog.
            if (toggle)
            {
                Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Buildings");
            }
            else
            {
                Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Buildings"));
            }
        }

        public static void SetFogRender(bool toggle)
        {
            // Toggles building renders and fog.
            if (toggle)
            {
                GameObject.FindObjectOfType<RenderProperties>().m_volumeFogDensity = 0.002f;
            }
            else
            {
                GameObject.FindObjectOfType<RenderProperties>().m_volumeFogDensity = 0;
            }
        }

        public static Material GetMaterial(string matName)
        {
            Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Material material in materials)
            {
                if (material.name.Equals(matName))
                {
                    return material;
                }
            }
            return null;
        }

        public static Quaternion GetRotationMapBetweenVecs(Vector3 referenceVec, Vector3 rotatedVec, out float angle)
        {
            Vector3 axisVec = referenceVec;
            axisVec.Normalize();
            rotatedVec.Normalize();

            // Find angle between the vectors
            angle = (float)Math.Acos((double)Vector3.Dot(axisVec, rotatedVec));
            Vector3 axisOfRotation = Vector3.Cross(axisVec, rotatedVec);

            axisOfRotation.Normalize();

            // Convert to degrees
            angle = (float)(angle * 180 / Math.PI);


            return Quaternion.AngleAxis(angle, axisOfRotation);
        }

        public static Quaternion GetCameraRotationAboutYAxis(RenderManager.CameraInfo cameraInfo)
        {
            Vector3 yAxis = new Vector3(0, 1, 0);

            Quaternion cameraRotation = cameraInfo.m_rotation;
            Vector3 xAxis = new Vector3(1,0,0);
            Vector3 xTransformed = cameraRotation * xAxis;

            // Project the xAxis rotated onto the X-Z plane.
            Vector3 projection = xTransformed - (Vector3.Dot(xTransformed, yAxis) * yAxis);
            projection.Normalize();

            // Find angle between the xAxis and the projection - the rotation angle in the X-Z plane.
            float angle = (float)Math.Acos((double)Vector3.Dot(xAxis, projection));

            Vector3 axisOfRotation = Vector3.Cross(xAxis, projection);
            axisOfRotation.Normalize();

            // Convert to degrees
            angle = (float) (angle * 180 / Math.PI);

            return Quaternion.AngleAxis(angle, axisOfRotation);
        }

        public static T GetPrivateVariable<T>(object obj, string fieldName)
        {
            return (T)obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }

        public static MethodInfo GetPrivateMethod(Type type, string methodName)
        {
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        }
    }
}
