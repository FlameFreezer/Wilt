Shader "Custom/ASCII2DSprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _AsciiAtlas("ASCII Atlas", 2D) = "white" {}

        [Header(ASCII Grid)]
        _CellsX("Horizontal Cells", Float) = 32
        _CellsY("Vertical Cells", Float) = 32
        _CharacterCount("Character Count", Float) = 10

        [Header(ASCII Appearance)]
        _AsciiColor("ASCII Solid Color", Color) = (1, 1, 1, 1)
        [Toggle] _UseSpriteColor("ASCII Uses Sprite Colors", Float) = 0
        _AsciiStrength("ASCII Opacity", Range(0, 1)) = 0.7
        _BaseOpacity("Base Sprite Opacity", Range(0, 1)) = 1

        [Header(Brightness)]
        _Brightness("Brightness", Range(0.1, 3)) = 1
        [Toggle] _InvertBrightness("Invert Brightness", Float) = 0
        _AlphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.01

        [Header(Outline)]
        [Toggle] _UseOutline("Enable ASCII Outline", Float) = 0
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 3)) = 1
        _OutlineOpacity("Outline Opacity", Range(0, 1)) = 1

        [Header(Shadow)]
        [Toggle] _UseShadow("Enable ASCII Shadow", Float) = 0
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
        _ShadowOffsetX("Shadow Offset X", Range(-4, 4)) = 1
        _ShadowOffsetY("Shadow Offset Y", Range(-4, 4)) = -1
        _ShadowOpacity("Shadow Opacity", Range(0, 1)) = 0.7

        [Header(Animation)]
        [Toggle] _AnimateWobble("Enable Wobble", Float) = 0
        _WobbleStrength("Wobble Strength", Range(0, 0.15)) = 0.01
        _WobbleSpeed("Wobble Speed", Range(0, 20)) = 3
        _WobbleFrequency("Wobble Frequency", Range(1, 50)) = 12

        [Toggle] _AnimateCorruption("Enable Corruption", Float) = 0
        _CorruptionStrength("Corruption Strength", Range(0, 0.25)) = 0.02
        _CorruptionSpeed("Corruption Speed", Range(0, 20)) = 4
        _CorruptionAmount("Corruption Amount", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "ASCII Sprite Advanced"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_AsciiAtlas);
            SAMPLER(sampler_AsciiAtlas);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;

                float4 _AsciiColor;
                float4 _OutlineColor;
                float4 _ShadowColor;

                float _CellsX;
                float _CellsY;
                float _CharacterCount;

                float _UseSpriteColor;
                float _AsciiStrength;
                float _BaseOpacity;

                float _Brightness;
                float _InvertBrightness;
                float _AlphaCutoff;

                float _UseOutline;
                float _OutlineThickness;
                float _OutlineOpacity;

                float _UseShadow;
                float _ShadowOffsetX;
                float _ShadowOffsetY;
                float _ShadowOpacity;

                float _AnimateWobble;
                float _WobbleStrength;
                float _WobbleSpeed;
                float _WobbleFrequency;

                float _AnimateCorruption;
                float _CorruptionStrength;
                float _CorruptionSpeed;
                float _CorruptionAmount;
            CBUFFER_END

            float Hash21(float2 value)
            {
                value = frac(value * float2(123.34, 456.21));
                value += dot(value, value + 45.32);
                return frac(value.x * value.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;

                return output;
            }

            half4 SampleCellAverage(float2 centerUV, float2 cellCount, half4 vertexColor)
            {
                float2 sampleOffset = float2(0.25 / cellCount.x, 0.25 / cellCount.y);

                half4 color = 0;

                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, centerUV);
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, centerUV + float2(sampleOffset.x, 0));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, centerUV - float2(sampleOffset.x, 0));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, centerUV + float2(0, sampleOffset.y));
                color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, centerUV - float2(0, sampleOffset.y));

                color /= 5.0;
                color *= vertexColor;

                return color;
            }

            float SampleGlyphMask(float characterIndex, float2 characterUV, float characterCount)
            {
                float2 atlasUV;
                atlasUV.x = (characterIndex + characterUV.x) / characterCount;
                atlasUV.y = characterUV.y;

                half4 sampleColor = SAMPLE_TEXTURE2D(_AsciiAtlas, sampler_AsciiAtlas, atlasUV);

                return sampleColor.r * sampleColor.a;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 spriteColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                spriteColor *= input.color;

                if (spriteColor.a <= _AlphaCutoff)
                {
                    discard;
                }

                float2 cellCount = max(float2(_CellsX, _CellsY), float2(1.0, 1.0));
                float safeCharacterCount = max(_CharacterCount, 1.0);

                float2 asciiUV = input.uv;

                // Smooth animated distortion across the sprite.
                float wobbleX = sin((asciiUV.y * _WobbleFrequency) + (_Time.y * _WobbleSpeed));
                float wobbleY = cos((asciiUV.x * _WobbleFrequency) + (_Time.y * _WobbleSpeed * 0.83));

                float2 wobbleOffset = float2(wobbleX, wobbleY) * _WobbleStrength * saturate(_AnimateWobble);
                asciiUV += wobbleOffset;

                float2 cellID = floor(asciiUV * cellCount);

                // Random block corruption that periodically shifts selected cells.
                float corruptionFrame = floor(_Time.y * _CorruptionSpeed);
                float corruptionRandom = Hash21(cellID + corruptionFrame);

                float corruptionMask = step(1.0 - _CorruptionAmount, corruptionRandom);
                corruptionMask *= saturate(_AnimateCorruption);

                float horizontalDirection = Hash21(cellID + corruptionFrame + 14.37) - 0.5;
                float verticalDirection = Hash21(cellID + corruptionFrame + 61.82) - 0.5;

                float2 corruptionOffset = float2(horizontalDirection, verticalDirection);
                corruptionOffset *= _CorruptionStrength * corruptionMask;

                asciiUV += corruptionOffset;

                cellID = floor(asciiUV * cellCount);

                float2 cellCenterUV = (cellID + 0.5) / cellCount;

                half4 cellColor = SampleCellAverage(cellCenterUV, cellCount, input.color);

                float luminance = dot(cellColor.rgb, float3(0.299, 0.587, 0.114));
                luminance = saturate(luminance * _Brightness);
                luminance = lerp(luminance, 1.0 - luminance, saturate(_InvertBrightness));

                float characterIndex = floor(luminance * (safeCharacterCount - 1.0));

                float2 characterUV = frac(asciiUV * cellCount);

                float characterMask = SampleGlyphMask(characterIndex, characterUV, safeCharacterCount);

                // One atlas pixel relative to the current glyph cell.
                float2 glyphPixelSize = float2(1.0 / 16.0, 1.0 / 16.0);

                // Outline samples around the glyph.
                float outlineMask = characterMask;

                if (_UseOutline > 0.5)
                {
                    float2 outlineOffset = glyphPixelSize * _OutlineThickness;

                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV + float2(outlineOffset.x, 0), safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV - float2(outlineOffset.x, 0), safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV + float2(0, outlineOffset.y), safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV - float2(0, outlineOffset.y), safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV + outlineOffset, safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV - outlineOffset, safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV + float2(outlineOffset.x, -outlineOffset.y), safeCharacterCount));
                    outlineMask = max(outlineMask, SampleGlyphMask(characterIndex, characterUV + float2(-outlineOffset.x, outlineOffset.y), safeCharacterCount));
                }

                float outlineOnlyMask = saturate(outlineMask - characterMask);
                outlineOnlyMask *= _OutlineOpacity * saturate(_UseOutline);

                // Shadow lookup.
                float2 shadowOffset = float2(_ShadowOffsetX, _ShadowOffsetY) * glyphPixelSize;
                float shadowMask = SampleGlyphMask(characterIndex, characterUV - shadowOffset, safeCharacterCount);
                shadowMask *= _ShadowOpacity * saturate(_UseShadow);

                float3 chosenAsciiColor = lerp(_AsciiColor.rgb, cellColor.rgb, saturate(_UseSpriteColor));

                float baseAlpha = spriteColor.a * _BaseOpacity;
                float asciiAlpha = characterMask * _AsciiStrength * _AsciiColor.a * spriteColor.a;
                float outlineAlpha = outlineOnlyMask * _OutlineColor.a * spriteColor.a;
                float shadowAlpha = shadowMask * _ShadowColor.a * spriteColor.a;

                // Prevent shadow from drawing over the main glyph.
                shadowAlpha *= 1.0 - characterMask;

                float3 resultRGB = spriteColor.rgb;
                float resultAlpha = baseAlpha;

                // Shadow over base.
                resultRGB = lerp(resultRGB, _ShadowColor.rgb, shadowAlpha);
                resultAlpha = shadowAlpha + resultAlpha * (1.0 - shadowAlpha);

                // Outline over shadow.
                resultRGB = lerp(resultRGB, _OutlineColor.rgb, outlineAlpha);
                resultAlpha = outlineAlpha + resultAlpha * (1.0 - outlineAlpha);

                // Main ASCII glyph over everything.
                resultRGB = lerp(resultRGB, chosenAsciiColor, asciiAlpha);
                resultAlpha = asciiAlpha + resultAlpha * (1.0 - asciiAlpha);

                return half4(resultRGB, resultAlpha);
            }

            ENDHLSL
        }
    }
}