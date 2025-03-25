using UnityEngine;

namespace BehaviourTreeSystem
{
    public class SectorScanDebug : MonoBehaviour
    {
        [Tooltip("当修改了Scanner属性后, 修改该值以刷新可视化")]
        public bool RefashVisual;
        [HideInInspector]
        public SectorScan scan;
        private Mesh mesh;

        /// <summary>
        /// 创建可视化扫描器的 Mesh
        /// </summary>
        /// <returns></returns>
        private Mesh CreateWedgeMesh()
        {
            Mesh mesh = new();

            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -scan.angle, 0) * Vector3.forward * scan.distance;
            Vector3 bottomRight = Quaternion.Euler(0, scan.angle, 0) * Vector3.forward * scan.distance;

            Vector3 topCenter = bottomCenter + Vector3.up * scan.height;
            Vector3 topLeft = bottomLeft + Vector3.up * scan.height;
            Vector3 topRight = bottomRight + Vector3.up * scan.height;

            int vert = 0;

            // left side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            // right side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -scan.angle;
            float deltaAngle = (scan.angle * 2) / segments;
            for (int i = 0; i < segments; ++i)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * scan.distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * scan.distance;

                topLeft = bottomLeft + Vector3.up * scan.height;
                topRight = bottomRight + Vector3.up * scan.height;

                // far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                // top
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                // bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate()
        {
            if (scan == null) return;
            mesh = CreateWedgeMesh();
        }

        private void OnDrawGizmos()
        {
            if (scan == null) return;

            if (mesh != null && scan.debugScaner)
            {
                Gizmos.color = scan.debugColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            if (scan.debugTarget)
            {
                Gizmos.color = Color.green;
                foreach (var obj in scan.Objects)
                {
                    Gizmos.DrawSphere(obj.transform.position, 0.2f);
                }
            }

            if (scan.debugDetectRange)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, scan.distance);
            }
        }
    }
}
