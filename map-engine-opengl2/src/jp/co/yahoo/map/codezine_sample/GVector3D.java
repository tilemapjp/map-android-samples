package jp.co.yahoo.map.codezine_sample;

public class GVector3D {
	public double x;
	public double y;
	public double z;
	
	void set(double x, double y, double z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	/**
	 * @param a
	 * @param b
	 * @param dest
	 */
	static void crossProduct(GVector3D a, GVector3D b, GVector3D dest) {
		double dx = a.y*b.z - a.z*b.y;
		double dy = a.z*b.x - a.x*b.z;
		double dz = a.x*b.y - a.y*b.x;
		dest.set(dx, dy, dz);
	}
	
	/**
	 * 正規化
	 */
	void normalize() {
		double l = (double)Math.sqrt(x*x + y*y + z*z);
		if(l==0) {
			this.set(0, 0, 0);
		} else {
			this.set(x/l, y/l, z/l);
		}
	}
}
