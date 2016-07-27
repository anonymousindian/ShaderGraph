
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
    public abstract class BaseLightFunction
    {
        public const string kNormalSlotName = "Normal";
        public const int NormalSlotId = 1;

        public virtual string GetLightFunctionName() { return ""; }
        public virtual string GetSurfaceOutputStructureName() { return ""; }
        public virtual void GenerateLightFunctionBody(ShaderGenerator visitor) {}

        public virtual void GenerateLightFunctionName(ShaderGenerator visitor)
        {
            visitor.AddPragmaChunk(GetLightFunctionName());
        }

        public virtual void GenerateSurfaceOutputStructureName(ShaderGenerator visitor)
        {
            visitor.AddPragmaChunk(GetSurfaceOutputStructureName());
        }

        public abstract void DoSlotsForConfiguration(PixelShaderNode node);

        public virtual int GetFirstPassSlotId()
        {
            return NormalSlotId;
        }
    }

    class PBRMetalicLightFunction : BaseLightFunction
    {
        public const string AlbedoSlotName = "Albedo";
        public const string MetallicSlotName = "Metallic";
        public const string EmissionSlotName = "Emission";
        public const string SmoothnessSlotName = "Smoothness";
        public const string OcclusionSlotName = "Occlusion";
        public const string AlphaSlotName = "Alpha";

        public const int AlbedoSlotId = 0;
        public const int MetallicSlotId = 2;
        public const int EmissionSlotId = 3;
        public const int SmoothnessSlotId = 4;
        public const int OcclusionSlotId = 5;
        public const int AlphaSlotId = 6;

        public override string GetLightFunctionName() { return "Standard"; }
        public override string GetSurfaceOutputStructureName() { return "SurfaceOutputStandard"; }
        public override void DoSlotsForConfiguration(PixelShaderNode node)
        {
            node.AddSlot(new MaterialSlot(AlbedoSlotId, AlbedoSlotName, AlbedoSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(NormalSlotId, kNormalSlotName, kNormalSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(EmissionSlotId, EmissionSlotName, EmissionSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(MetallicSlotId, MetallicSlotName, MetallicSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
            node.AddSlot(new MaterialSlot(SmoothnessSlotId, SmoothnessSlotName, SmoothnessSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
            node.AddSlot(new MaterialSlot(OcclusionSlotId, OcclusionSlotName, OcclusionSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
            node.AddSlot(new MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
          
            // clear out slot names that do not match the slots 
            // we support
            node.RemoveSlotsNameNotMatching(
                new[]
                {
                    AlbedoSlotId,
                    NormalSlotId,
                    EmissionSlotId,
                    MetallicSlotId,
                    SmoothnessSlotId,
                    OcclusionSlotId,
                    AlphaSlotId
                });
        }
    }

    class PBRSpecularLightFunction : BaseLightFunction
    {
        public const string kAlbedoSlotName = "Albedo";
        public const string kSpecularSlotName = "Specular";
        public const string kEmissionSlotName = "Emission";
        public const string kSmoothnessSlotName = "Smoothness";
        public const string kOcclusionSlotName = "Occlusion";
        public const string kAlphaSlotName = "Alpha";
        
        public const int kAlbedoSlotId = 0;
        public const int kSpecularSlotId = 2;
        public const int kEmissionSlotId = 3;
        public const int kSmoothnessSlotId = 4;
        public const int kOcclusionSlotId = 5;
        public const int kAlphaSlotId = 6;

        public override string GetLightFunctionName() { return "StandardSpecular"; }
        public override string GetSurfaceOutputStructureName() { return "SurfaceOutputStandardSpecular"; }
        public override void DoSlotsForConfiguration(PixelShaderNode node)
        {
            node.AddSlot(new MaterialSlot(kAlbedoSlotId, kAlbedoSlotName, kAlbedoSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(NormalSlotId, kNormalSlotName, kNormalSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(kSpecularSlotId, kSpecularSlotName, kSpecularSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(kEmissionSlotId, kEmissionSlotName, kEmissionSlotName, SlotType.Input, SlotValueType.Vector3, Vector4.zero));
            node.AddSlot(new MaterialSlot(kSmoothnessSlotId, kSmoothnessSlotName, kSmoothnessSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
            node.AddSlot(new MaterialSlot(kOcclusionSlotId, kOcclusionSlotName, kOcclusionSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));
            node.AddSlot(new MaterialSlot(kAlphaSlotId, kAlphaSlotName, kAlphaSlotName, SlotType.Input, SlotValueType.Vector1, Vector4.zero));

            // clear out slot names that do not match the slots 
            // we support
            node.RemoveSlotsNameNotMatching(
                new[]
                {
                    kAlbedoSlotId,
                    NormalSlotId,
                    kSpecularSlotId,
                    kEmissionSlotId,
                    kSmoothnessSlotId,
                    kOcclusionSlotId,
                    kAlphaSlotId
                });
        }
    }
}