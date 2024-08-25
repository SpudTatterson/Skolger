Shader "Custom/Outline Mask" {
  Properties {
    [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
    _StencilRef("Stencil Ref", Float) = 2
  }

  SubShader {
    Tags {
      "Queue" = "Transparent+200"
      "RenderType" = "Transparent"
    }

    Pass {
      Name "Mask"
      Cull Off
      ZTest [_ZTest]
      ZWrite Off
      ColorMask 0

      Stencil {
        Ref [_StencilRef]
        Pass Replace
      }
    }
  }
}
