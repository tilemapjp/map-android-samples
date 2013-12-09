package jp.co.yahoo.map.codezine_sample;

import android.opengl.GLES20;

public class Shader {
	/**
	 * 頂点シェーダプログラム
	 */
	private final String VSHADER_SOURCE =
			"precision highp float;" +
			"attribute vec4 a_Position;\n" +
			"uniform mat4 u_ViewMatrix;\n" +
			"uniform mat4 u_ProjMatrix;\n" +
			"varying vec4 v_Color;\n" +
			"void main() {\n" +
			"  gl_Position = u_ProjMatrix * u_ViewMatrix * a_Position;\n" +
			"  gl_PointSize = 16.0;" +
			"}\n";
	
	/**
	 * フラグメントシェーダプログラム
	 */
	private final String FSHADER_SOURCE =
			"precision highp float;" +
			"void main() {\n" +
			"  gl_FragColor = vec4(1.0,1.0,1.0,1.0);\n" +
			"}\n";
	
	/**
	 * シェーダープログラムを読み込みコンパイルします。
	 * @return
	 */
	public int initShaders() {
		int program = createProgram();
		GLES20.glUseProgram(program);
		return program;
	}
	
	/**
	 * プログラムオブジェクトを作成します。
	 * @return
	 */
	public int createProgram() {
		// シェーダオブジェクトを作成する
		int vertexShader = loadShader(GLES20.GL_VERTEX_SHADER, this.VSHADER_SOURCE);
		int fragmentShader = loadShader(GLES20.GL_FRAGMENT_SHADER, this.FSHADER_SOURCE);
		
		// プログラムオブジェクトを作成する
		int program = GLES20.glCreateProgram();
		if (program == 0) {
			throw new RuntimeException("failed to create program");
		}
		
		// シェーダオブジェクトを設定する
		GLES20.glAttachShader(program, vertexShader);
		GLES20.glAttachShader(program, fragmentShader);
		
		// プログラムオブジェクトをリンクする
		GLES20.glLinkProgram(program);
		
		// リンク結果をチェックする
		int[] linked = new int[1];
		GLES20.glGetProgramiv(program, GLES20.GL_LINK_STATUS, linked, 0);
		if (linked[0] != GLES20.GL_TRUE) {
			String error = GLES20.glGetProgramInfoLog(program);
			throw new RuntimeException("failed to link program: " + error);
		}
		return program;
	}
	
	/**
	 * シェーダーを読み込みます。
	 * @param type
	 * @param source
	 * @return
	 */
	public int loadShader(int type, String source) {
		// シェーダオブジェクトを作成する
		int shader = GLES20.glCreateShader(type);
		if (shader == 0) {
			throw new RuntimeException("unable to create shader");
		}
		// シェーダのプログラムを設定する
		GLES20.glShaderSource(shader, source);
		// シェーダをコンパイルする
		GLES20.glCompileShader(shader);
		// コンパイル結果を検査する
		int[] compiled = new int[1];
		GLES20.glGetShaderiv(shader, GLES20.GL_COMPILE_STATUS, compiled, 0);
		if (compiled[0] != GLES20.GL_TRUE) {
			String error = GLES20.glGetShaderInfoLog(shader);
			throw new RuntimeException("failed to compile shader: " + error);
		}
		return shader;
	}
}
