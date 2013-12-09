package jp.co.yahoo.map.codezine_sample;

import android.util.Log;
import android.view.MotionEvent;
import android.view.GestureDetector.SimpleOnGestureListener;
import android.view.ScaleGestureDetector;
import android.view.ScaleGestureDetector.SimpleOnScaleGestureListener;

public class MapControl extends SimpleOnGestureListener {
	private MapView mMv;
	public ScaleGestureDetector scaleGestureDetect = null;
	public float x = 0.0f; //視点の座標X
	public float y = 0.0f; //視点の座標Y
	public float z = 0.0f; //視点の座標Z
	public float lx = 0.0f;	//注視点の座標X
	public float ly = 0.0f;	//注視点の座標Y
	public float lz = 0.0f;	//注視点の座標Z
	static final float BASE_Z = 1.0f;	//規定Z値
	public float scale = 200000.0f;	//縮尺値
	public double cameradist = 0;	//カメラまでの距離
	
	public MapControl(MapView mv) {
		mMv = mv;
		scaleGestureDetect = new ScaleGestureDetector(mMv.getContext(), onScaleGesture);
	}
	
	public void setEyePosition(float x,float y,float z,float angle) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.lx = x;
		this.ly = y;
		this.cameradist = Math.sqrt(
				Math.pow(this.x-this.lx,2) +
				Math.pow(this.y-this.ly,2) +
				Math.pow(this.z-this.lz,2)
			);
	}
	
	/** 
	 * フリック移動
	 */
	@Override
	public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
		float dx = e2.getX() - e1.getX();
		float dy = e2.getY() - e1.getY();
		Log.d("map","onFling:"+dx+","+dy);
		return true;
	}
	
	/**
	 * シングルタップ
	 */
	@Override
	public boolean onSingleTapConfirmed(MotionEvent event) {
		Log.d("map","onSingleTapConfirmed");
		return true;
	}
	
	/**
	 * シングルタップ（ダブルタップ時も呼ばれる）
	 */
	@Override
	public boolean onSingleTapUp(MotionEvent event) {
		Log.d("map","onSingleTapUp");
		return true;
	}
	
	/**
	 * ダブルタップ
	 */
	@Override
	public boolean onDoubleTap(MotionEvent event) {
		Log.d("map","onDoubleTap");
		return true;
	}
	
	/**
	 * ダブルタップ中イベント
	 */
	@Override
	public boolean onDoubleTapEvent(MotionEvent e) {
		Log.d("map","onDoubleTapEvent");
		return true;
	}
	
	/**
	 * 押下
	 */
	@Override
	public boolean onDown(MotionEvent e) {
		Log.d("map","onDown");
		return true;
	}
	
	/**
	 * 長押し
	 */
	@Override
	public void onLongPress(MotionEvent e) {
		Log.d("map","onLongPress");
	}
	
	/**
	 * デバイス座標からワールド座標へ変換する
	 * @param deviceX
	 * @param deviceY
	 * @param angle
	 * @param zPos
	 * @return
	 */
	public GVector3D deviceCoord2MapCoord(double deviceX, double deviceY, float angle, double zPos) {
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
		
		int height = mMv.getHeight();
		
		//引数で渡された座標を画面中心からの相対座標に変換
		deviceX = deviceX*this.cameradist/height;
		deviceY = deviceY*this.cameradist/height;
		
		//ワールド座標のy方向と画面座標のｙ方向が反対向きなので、-1をかけて向きをワールドの向きにそろえる
		deviceY*=-1;
		
		//視線ベクトル
		GVector3D posEye = new GVector3D();
		posEye.set(-posEyeX, -posEyeY, -posEyeZ);
		posEye.normalize();
		
		//視錐台の前面の中心座標
		double posFrontCenterX = posEyeX + posEye.x * near;
		double posFrontCenterY = posEyeY + posEye.y * near;
		double posFrontCenterZ = posEyeZ + posEye.z * near;
		
		//視錐台の前面を構成する平面の基本ベクトルをワールド座標系に変換
		//ｘ方向
		GMatrix m = new GMatrix();
		GVector3D vecX = new GVector3D();
		m.rotate(-angle, 0, 0, 1);
		m.transformPoint(1, 0, 0, vecX);
		
		//y方向
		GVector3D vecY = new GVector3D();
		GVector3D.crossProduct(vecX, posEye, vecY);
		vecY.normalize();
		
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
	@Override
	public boolean onScroll(MotionEvent e1, MotionEvent e2,float distanceX, float distanceY) {
		//移動元と移動先のデバイス座標取得
		float x2 = e2.getX();
		float y2 = e2.getY();
		float x1 = x2 - distanceX;
		float y1 = y2 - distanceY;
		
		//デバイスからワールド座標に変換
		GVector3D pos1 = deviceCoord2MapCoord(x1, y1, 0, 0);
		GVector3D pos2 = deviceCoord2MapCoord(x2, y2, 0, 0);
		
		//移動値を求める
		double dx = pos2.x-pos1.x;
		double dy = pos2.y-pos1.y;
		
		//カメラの位置を移動させる
		x += dx;
		y -= dy;
		lx += dx;
		ly -= dy;
		
		setPosition();
		return true;
	}
	
	/**
	 * 位置を変更する
	 */
	public void setPosition() {
		//カメラまでの距離を求める
		cameradist = Math.sqrt(
				Math.pow(x-lx,2) +
				Math.pow(y-ly,2) +
				Math.pow(z-lz,2)
			);
		//視点（カメラ）の位置を設定します。
		GMatrix viewmatrix = new GMatrix();
		viewmatrix.setLook(x, y, z, lx, ly, lz, 0.0f, 1.0f, 0.0f);
		
		int width = mMv.getWidth();
		int height = mMv.getHeight();
		
		//投影・視点変換行列の作成
		GMatrix projectionmatrix = new GMatrix();
		float fov = 45.0f; //画角
		float raito = (float) width/height;  //画面の縦横比
		float near = (float) (this.cameradist / 2);
		float far = 100000000.0f; //視点から遠平面までの距離
		float top = near * (float)Math.tan(Math.toRadians(fov)); //画角から近平面のTOPを求める
		float bottom = -top;//近平面のbottom
		float left = bottom * raito; //近平面のleft
		float right = top * raito;  //近平面のright
		projectionmatrix.frustum(left, right, bottom, top, near, far);
		
		//行列を設定する
		mMv.getSurfaceView().setMatrix(viewmatrix, projectionmatrix);
	}
	
	/**
	 * 押下（押下時のドラッグなどでは呼ばれない）
	 */
	@Override
	public void onShowPress(MotionEvent e) {
		Log.d("map","onShowPress");
	}
	
	/**
	 * ピンチのジェスチャー取得
	 */
	private SimpleOnScaleGestureListener onScaleGesture = new ScaleGestureDetector.SimpleOnScaleGestureListener() {
		/**
		 * 縮尺変更開始
		 */
		@Override
		public boolean onScaleBegin(ScaleGestureDetector detector) {
			Log.d("map", "onScaleBegin : "+ detector.getScaleFactor());
			return super.onScaleBegin(detector);
		}
		
		/**
		 * 縮尺変更終了
		 */
		@Override
		public void onScaleEnd(ScaleGestureDetector detector) {
			Log.d("map", "onScaleEnd : "+ detector.getScaleFactor());
			super.onScaleEnd(detector);
		}
		
		/**
		 * 縮尺変更
		 */
		@Override
		public boolean onScale(ScaleGestureDetector detector) {
			Log.d("map", "onScale : "+ detector.getScaleFactor());
			z /= detector.getScaleFactor();
			setPosition();
			return true;
		};
	};
}