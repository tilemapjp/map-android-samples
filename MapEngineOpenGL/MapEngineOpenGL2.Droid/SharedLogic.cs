using System;
using System.IO;
using System.Collections.Generic;

#if __ANDROID__
using Android.Content.Res;
using Android.Content;
#elif __IOS__
using MonoTouch.Foundation;
#endif

namespace MapEngineOpenGL
{
	public interface IMapPresenter
	{
		int  Height { get; }
		int  Width  { get; }
		void SetMatrix(GMatrix viewmatrix, GMatrix projectionmatrix);
	}

	public class MapController
	{
		WeakReference _mapPresenter;
		IMapPresenter MapPresenter {
			get { 
				return (IMapPresenter)_mapPresenter.Target;
			}
		}

		private float[] _coordList;
		public float[] CoordList {
			get { 
				return _coordList;
			}
		}

		public float x = 0.0f; //視点の座標X
		public float y = 0.0f; //視点の座標Y
		public float z = 0.0f; //視点の座標Z
		public float lx = 0.0f;        //注視点の座標X
		public float ly = 0.0f;        //注視点の座標Y
		public float lz = 0.0f;        //注視点の座標Z
		public static float BASE_Z = 1.0f;        //規定Z値
		public float scale = 200000.0f;        //縮尺値
		public double cameradist = 0;        //カメラまでの距離

		public MapController (IMapPresenter mapPresenter, string filename) : base()
		{
			_mapPresenter = new WeakReference (mapPresenter);
			_coordList    = SharedLogic.CoordList (filename);
		}

		public void SetEyePosition(float x,float y,float z,float angle) {
			this.x = x;
			this.y = y;
			this.z = z;
			this.lx = x;
			this.ly = y;
			this.cameradist = Math.Sqrt(
				Math.Pow((double)this.x-(double)this.lx,2.0) +
				Math.Pow((double)this.y-(double)this.ly,2.0) +
				Math.Pow((double)this.z-(double)this.lz,2.0)
			);
		}

		/**
         * デバイス座標からワールド座標へ変換する
         * @param deviceX
         * @param deviceY
         * @param angle
         * @param zPos
         * @return
         */
		public GVector3D DeviceCoord2MapCoord(double deviceX, double deviceY, float angle, double zPos) {
			GVector3D mapPos = new GVector3D();
			if(this.cameradist == 0) return mapPos;
			double posEyeX = 0;
			double posEyeY = 0;
			double posEyeZ = 0;
			double near = 0;
			posEyeX = x - lx;
			posEyeY = y - ly;
			posEyeZ = z - lz;
			near = this.cameradist / 2;

			int height = MapPresenter.Height;

			//引数で渡された座標を画面中心からの相対座標に変換
			deviceX = deviceX*this.cameradist/height;
			deviceY = deviceY*this.cameradist/height;

			//ワールド座標のy方向と画面座標のｙ方向が反対向きなので、-1をかけて向きをワールドの向きにそろえる
			deviceY*=-1;

			//視線ベクトル
			GVector3D posEye = new GVector3D();
			posEye.Set(-posEyeX, -posEyeY, -posEyeZ);
			posEye.Normalize();

			//視錐台の前面の中心座標
			double posFrontCenterX = posEyeX + posEye.x * near;
			double posFrontCenterY = posEyeY + posEye.y * near;
			double posFrontCenterZ = posEyeZ + posEye.z * near;

			//視錐台の前面を構成する平面の基本ベクトルをワールド座標系に変換
			//ｘ方向
			GMatrix m = new GMatrix();
			GVector3D vecX = new GVector3D();
			m.Rotate(-angle, 0, 0, 1);
			m.TransformPoint(1, 0, 0, vecX);

			//y方向
			GVector3D vecY = new GVector3D();
			GVector3D.CrossProduct(vecX, posEye, vecY);
			vecY.Normalize();

			//視錐台の前面上の指定点のワールド座標
			double posFrontX = posFrontCenterX + deviceX * vecX.x + deviceY * vecY.x;
			double posFrontY = posFrontCenterY + deviceX * vecX.y + deviceY * vecY.y;
			double posFrontZ = posFrontCenterZ + deviceX * vecX.z + deviceY * vecY.z;

			//視点から指定点への方向ベクトル
			double direcVecX = posFrontX - posEyeX;
			double direcVecY = posFrontY - posEyeY;
			double direcVecZ = posFrontZ - posEyeZ;

			//地図面上の指定点の位置o
			double fact=(zPos-posEyeZ) / direcVecZ;
			mapPos.x = posEyeX + direcVecX * fact;
			mapPos.y = posEyeY + direcVecY * fact;
			mapPos.y *= -1;

			return mapPos;
		}

		/**
         * ドラッグ
         */
		public void OnScroll(float x2, float y2, float distanceX, float distanceY) {
			//移動元と移動先のデバイス座標取得
			float x1 = x2 - distanceX;
			float y1 = y2 - distanceY;

			Console.WriteLine ("Scroll: x1: {0} y1: {1} x2: {2} y2: {3} dX: {4} dY: {5}", x1, y1, x2, y2, distanceX, distanceY);

			//デバイスからワールド座標に変換
			GVector3D pos1 = DeviceCoord2MapCoord(x1, y1, 0, 0);
			GVector3D pos2 = DeviceCoord2MapCoord(x2, y2, 0, 0);

			//移動値を求める
			double dx = pos2.x-pos1.x;
			double dy = pos2.y-pos1.y;

			//カメラの位置を移動させる
			x  += (float)dx;
			y  -= (float)dy;
			lx += (float)dx;
			ly -= (float)dy;

			SetPosition();
		}

		/*				*
         * 位置を変更する
         */
		public void SetPosition() {
			//カメラまでの距離を求める
			cameradist = Math.Sqrt(
				Math.Pow(x-lx,2) +
				Math.Pow(y-ly,2) +
				Math.Pow(z-lz,2)
			);
			//視点（カメラ）の位置を設定します。
			GMatrix viewmatrix = new GMatrix();
			viewmatrix.SetLook(x, y, z, lx, ly, lz, 0.0f, 1.0f, 0.0f);

			int width = MapPresenter.Width;
			int height = MapPresenter.Height;

			//投影・視点変換行列の作成
			GMatrix projectionmatrix = new GMatrix();
			float fov = 45.0f; //画角
			float raito = (float) width/height;  //画面の縦横比
			float near = (float) (this.cameradist / 2);
			float far = 100000000.0f; //視点から遠平面までの距離
			float top = near * (float)Math.Tan(fov/180*Math.PI); //画角から近平面のTOPを求める
			float bottom = -top;//近平面のbottom
			float left = bottom * raito; //近平面のleft
			float right = top * raito;  //近平面のright
			projectionmatrix.Frustum(left, right, bottom, top, near, far);

			//行列を設定する
			MapPresenter.SetMatrix(viewmatrix, projectionmatrix);
		}

		/**
         * 縮尺変更:OnScaleGestureから
         */
		public void OnScale(float scaleFactor) {
			z /= scaleFactor;
			SetPosition ();
		}

		public void Release() 
		{
			_mapPresenter = null;
			_coordList = null;
		}
	}

	public class SharedLogic
	{
#if __ANDROID__
		static Context _context;
		private static Context context {
			get { 
				return _context;
			}
		}
#endif

		private SharedLogic ()
		{
		}

#if __ANDROID__
		public static void SetSharedContext(Context context) {
			_context = context;
		}
#endif

		public static float[] CoordList(string filename) {
			StreamReader isreader;
			var strList = new List<string[]>();

			//ファイル読み込みなど操作するときはtry{}catch{}で囲みます。
			try{
#if __ANDROID__
				var istream = context.Assets.Open(filename);
				isreader = new StreamReader(istream);
#elif __IOS__
				var bundlePath = NSBundle.MainBundle.BundlePath + "/" + filename;
				isreader = new StreamReader(bundlePath);
#endif
				string str;
				while ((str = isreader.ReadLine()) != null) {
					var split_str = str.Split('\t');
					strList.Add(split_str);
				}
				isreader.Close();
			} catch {
				Console.WriteLine("open and input text : 読み込み失敗");
			}

			//読み込んだファイルをFloat配列へ変換する
			float[] res = new float[strList.Count * 3];  //x,y,zの順に格納されたfloat配列
			for(int i=0; i<strList.Count; i++){
				var str = strList [i];
				res[i * 3 + 0] = float.Parse(str[0]);    //X座標
				res[i * 3 + 1] = float.Parse(str[1]);    //Y座標
				res[i * 3 + 2] = 0.0f;                   //Z座標
			}
			strList.Clear ();

			return res;
		}
	}
}

