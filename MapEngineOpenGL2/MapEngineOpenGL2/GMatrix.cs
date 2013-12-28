using System;
using Android.Opengl;

namespace MapEngineOpenGL2
{
	public class GMatrix
	{
		public float[] matrix = new float[16];        //行列

		public GMatrix() {
			Identity();
		}

		/**
         * 単位行列
         */
		public void Identity() {
			Matrix.SetIdentityM(matrix, 0);
		}

		/**
         * 回転
         * @param angle
         * @param x
         * @param y
         * @param z
         */
		public void Rotate(float angle, float x, float y, float z) {
			Matrix.RotateM(matrix, 0, -angle, x, y, z);
		}

		/**
         * 指定されたポイントを行列変換する
         * @param x
         * @param y
         * @param z
         * @param res
         */
		public void TransformPoint(double x, double y, double z, GVector3D res) {
			res.x =   (matrix[4*0 + 0] * x)
			            + (matrix[4*1 + 0] * y)
			            + (matrix[4*2 + 0] * z)
			            +  matrix[4*3 + 0];
			res.y =   (matrix[4*0 + 1] * x)
			            + (matrix[4*1 + 1] * y)
			            + (matrix[4*2 + 1] * z)
			            +  matrix[4*3 + 1];
			res.z =   (matrix[4*0 + 2] * x)
			            + (matrix[4*1 + 2] * y)
			            + (matrix[4*2 + 2] * z)
			            +  matrix[4*3 + 2];
		}

		/**
         * 行列のコピー
         * @param m
         */
		public void Copy(GMatrix m) {
			Array.Copy (matrix, m.matrix, 16);
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
			Matrix.SetLookAtM(this.matrix, 0, eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ);
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
			Matrix.FrustumM(this.matrix, 0, left, right, bottom, top, near, far);
		}
	}
}

