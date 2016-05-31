Shader "Unlit/LightingTexture"
{
	Properties {
      _MainTex ("Texture Image", 2D) = "white" {} 
      _MainTex1 ("Texture Image", 2D) = "white" {} 
        
   }
   SubShader {
      Pass {    
         CGPROGRAM
         #pragma vertex vert  
         #pragma fragment frag 
 
         uniform sampler2D _MainTex;    
           uniform sampler2D _MainTex1;    
           
 
         struct vertexInput {
            fixed4 vertex : POSITION;//根据最近对移动平台的性能研究 ，建议一般用fix类型
            fixed4 texcoord : TEXCOORD0;
            fixed4 texcoord1 : TEXCOORD1;
            fixed4 clor : COLOR;
         };
         struct vertexOutput {
            fixed4 pos : SV_POSITION;
            fixed4 tex : TEXCOORD0;
            fixed4 tex1 : TEXCOORD1;
            fixed4 cor :COLOR;
         };
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.tex =mul(UNITY_MATRIX_TEXTURE0,input.texcoord).xyzw;
            output.tex1 =input.texcoord1;
            output.cor = input.clor;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
         fixed4 frag(vertexOutput input) : COLOR
         {
            fixed4 color1 = input.cor;
            {
            fixed4 texture1 =  tex2D(_MainTex, fixed2(input.tex));    
            fixed4 prea = input.cor;
            color1 = texture1.rgba;
            }
            {
            fixed4 texture2 =  tex2D(_MainTex1, fixed2(input.tex1));    
            fixed4 prea = color1;
            color1.rgb = (texture2.rgb +prea.rgb);
            color1.a = texture2.a;
            }
            
            if(color1.a<=0.01)//采用这种方式实现透明效果
            {
              discard;
            }
            return color1; 
         }
         ENDCG
      }
   }
}
