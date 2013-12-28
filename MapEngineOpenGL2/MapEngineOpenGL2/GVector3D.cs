using System;

namespace MapEngineOpenGL2
{
	public class GVector3D
	{
		public double x;
		public double y;
		public double z;

		public GVector3D ()
		{
		}

		public void Set(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/**
         * @param a
         * @param b
         * @param dest
         */
		public static void CrossProduct(GVector3D a, GVector3D b, GVector3D dest) {
			double dx = a.y*b.z - a.z*b.y;
			double dy = a.z*b.x - a.x*b.z;
			double dz = a.x*b.y - a.y*b.x;
			dest.Set(dx, dy, dz);
		}

		/**
         * 正規化
         */
		public void Normalize() {
			double l = (double)Math.Sqrt(x*x + y*y + z*z);
			if(l==0) {
				this.Set(0, 0, 0);
			} else {
				this.Set(x/l, y/l, z/l);
			}
		}
	}
}

