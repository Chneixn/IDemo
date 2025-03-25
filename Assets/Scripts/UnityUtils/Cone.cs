using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public class ConeMesh
    {
        public Vector3[] Vertices { get; }
        public Vector3[] Normals { get; }
        public int[] Triangles { get; }
        public Vector2[] UVs { get; }


        // 圆锥的高度(圆锥顶点与底圆中心点的距离) 
        private float mHeight;
        // 底圆周长上的顶点数量(越多越圆)
        private int mCircleVertCount;
        // 底圆的半径
        private float mRadius;

        private Mesh mMesh;

        public ConeMesh(float radius, int circleVertCount, float height)
        {
            mRadius = radius;
            mHeight = height;
            mCircleVertCount = circleVertCount;

            Vertices = new Vector3[(mCircleVertCount + 1) * 3];
            Normals = new Vector3[(mCircleVertCount + 1) * 3];
            UVs = new Vector2[(mCircleVertCount + 1) * 3];

            // 三角形包含(底圆+侧面)
            Triangles = new int[circleVertCount * 3 + circleVertCount * 3];

            GenerateVerts();
            GenerateNormals();
            GenerateTriangles();
            GenerateUVs();
        }

        private void GenerateVerts()
        {
            // 锥顶点坐标的位置(Y值乘以0.5是为了最终锚点在圆锥的中心,所以底圆的顶点y值会相应的-0.5*mHeight)
            var topPoint = new Vector3(0, mHeight * 0.5f, 0);
            var bottomCenterPoint = new Vector3(0, -mHeight * 0.5f, 0);

            // 存入底圆中心的顶点
            Vertices[0] = bottomCenterPoint;

            for (int i = 0; i < mCircleVertCount; i++)
            {
                var ratio = (float)i / mCircleVertCount;
                // 角度从0到360 每次间隔增加 2PI/mCircleVertCount
                var rad = Mathf.PI * 2 * ratio;
                var cos = Mathf.Cos(rad) * mRadius;
                var sin = Mathf.Sin(rad) * mRadius;
                // 底圆上每个顶点的坐标
                var circleVertPos = new Vector3(cos, -mHeight * 0.5f, sin);

                // 保存底圆周长上的顶点(保存在1 到 mCircleVertCount 中)
                Vertices[i + 1] = circleVertPos;

                // 侧面的顶点保存在(mCircleVertCount + 1 到 allVertCount 中)
                var startIndex = mCircleVertCount + 1;
                Vertices[startIndex + i * 2] = circleVertPos;
                Vertices[startIndex + i * 2 + 1] = topPoint;
            }
            // 最后一个三角形由最后一个底圆顶点和第一个底圆顶点和圆锥顶点共同组成
            Vertices[mCircleVertCount + 1 + mCircleVertCount * 2] = Vertices[1];
            Vertices[mCircleVertCount + 1 + mCircleVertCount * 2 + 1] = topPoint;
        }

        private void GenerateNormals()
        {
            for (int i = 0; i <= mCircleVertCount; i++)
            {
                // 底面(第0位底圆中心 + mCircleVertCount数量的周长顶点)
                Normals[i] = Vector3.down;

                // 侧面的顶点的法线保存在(mCircleVertCount + 1 到 allVertCount 中)
                var ratio = (float)i / mCircleVertCount;
                // 角度从0到360 每次间隔增加 2PI/mCircleVertCount
                var rad = Mathf.PI * 2 * ratio;
                var cos = Mathf.Cos(rad);
                var sin = Mathf.Sin(rad);
                var startIndex = mCircleVertCount + 1;
                Normals[startIndex + i * 2] = new Vector3(cos, 0, sin);
                Normals[startIndex + i * 2 + 1] = new Vector3(cos, 0, sin);
            }
        }

        private void GenerateTriangles()
        {
            // 侧面三角形起始位置
            int sideTraStart = (mCircleVertCount) * 3;
            // 侧面三角形顶点起始位置
            int sideTraVertStart = mCircleVertCount + 1;

            int i;
            for (i = 0; i < mCircleVertCount - 1; i++)
            {
                // 底面三角形
                Triangles[i * 3] = 0;
                Triangles[i * 3 + 1] = i + 1;
                Triangles[i * 3 + 2] = i + 2;

                // 侧面三角形
                Triangles[sideTraStart + i * 3] = sideTraVertStart + i * 2;  // 当前
                Triangles[sideTraStart + i * 3 + 1] = sideTraVertStart + i * 2 + 1;  // 顶
                Triangles[sideTraStart + i * 3 + 2] = sideTraVertStart + (i + 1) * 2;  // 下一个
            }
            // 最后一个底边三角形
            Triangles[i * 3] = 0;
            Triangles[i * 3 + 1] = mCircleVertCount;
            Triangles[i * 3 + 2] = 1;
            // 最后一个侧面三解形
            Triangles[sideTraStart + i * 3] = sideTraVertStart + i * 2;  // 当前
            Triangles[sideTraStart + i * 3 + 1] = sideTraVertStart + i * 2 + 1;  // 顶
            Triangles[sideTraStart + i * 3 + 2] = sideTraVertStart + (i + 1) * 2;  // 下一个
        }

        private void GenerateUVs()
        {
            // 底面中心顶点UV
            UVs[0] = new Vector2(0.5f, 0.5f);
            int i;
            var startIndex = mCircleVertCount + 1;
            for (i = 0; i < mCircleVertCount; i++)
            {
                // 底面圆边上顶点UV
                var ratio = (float)i / mCircleVertCount;
                var rad = Mathf.PI * 2 * ratio;
                var cos = Mathf.Cos(rad);
                var sin = Mathf.Sin(rad);
                UVs[i + 1] = new Vector2(cos, sin);

                // 侧面顶点UV
                UVs[startIndex + i * 2] = new Vector2(ratio, 0);
                UVs[startIndex + i * 2 + 1] = new Vector2(ratio, 1);
            }
            // 侧面最后顶点UV
            UVs[startIndex + i * 2] = new Vector2(1, 0);
            UVs[startIndex + i * 2 + 1] = new Vector2(1, 1);
        }

        public Mesh ToMesh()
        {
            if (mMesh == null)
            {
                mMesh = new Mesh();
                mMesh.name = "Cone";
                mMesh.vertices = Vertices;
                mMesh.normals = Normals;
                mMesh.uv = UVs;
                mMesh.triangles = Triangles;
            }
            return mMesh;
        }
    }
}