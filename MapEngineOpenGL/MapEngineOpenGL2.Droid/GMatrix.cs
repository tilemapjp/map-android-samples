using System;
#if __ANDROID__
using OpenTK.Platform.Android;
#elif __IOS__
using OpenTK.Platform.iPhoneOS;
#endif
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

namespace MapEngineOpenGL
{
	public class GMatrix
	{
		public Matrix4 matrix;       //行列

		public GMatrix() {
			Identity();
		}

		/**
         * 単位行列
         */
		public void Identity() {
			matrix = Matrix4.Identity;
		}

		/**
         * 回転
         * @param angle
         * @param x
         * @param y
         * @param z
         */
		public void Rotate(float angle, float x, float y, float z) {
			matrix = Matrix4.Mult (Matrix4.CreateFromAxisAngle(new Vector3 (x, y, z), angle), matrix);
		}

		/**
         * 指定されたポイントを行列変換する
         * @param x
         * @param y
         * @param z
         * @param res
         */
		public void TransformPoint(double x, double y, double z, GVector3D res) {
			res.x = matrix.M11 * x + matrix.M21 * y + matrix.M31 * z + matrix.M41;
			res.y = matrix.M12 * x + matrix.M22 * y + matrix.M32 * z + matrix.M42;
			res.z = matrix.M13 * x + matrix.M23 * y + matrix.M33 * z + matrix.M43;
		}

		/**
         * 行列のコピー
         * @param m
         */
		public void Copy(GMatrix m) {
			m.matrix = matrix;
		}

		/**
         * @param eyeX
         * @param eyeY
         * @param eyeZ
         * @param centerX
         * @param centerY
         * @param centerZ
         * @param upX
         * @param upY
         * @param upZ
         */
		public void SetLook( float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ) {
			matrix = Matrix4.LookAt (eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
		}

		/**
         * 透視投影
         * @param left
         * @param right
         * @param bottom
         * @param top
         * @param near
         * @param far
         */
		public void Frustum(float left, float right, float bottom, float top, float near, float far) {
			matrix = Matrix4.CreatePerspectiveOffCenter (left, right, bottom, top, near, far);
		}
	}
}

