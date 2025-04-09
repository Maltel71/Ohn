Shader "3D Mask/Mask URP"
{
	SubShader
	{
		Tags { "Queue" = "Transparent+1" }
		Pass
		{
			Blend Zero One 
		}
	}
}