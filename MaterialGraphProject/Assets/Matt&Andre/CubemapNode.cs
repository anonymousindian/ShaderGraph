using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Graphing;

namespace UnityEngine.MaterialGraph
{
	[Title("Input/Cubemap")]
	public class CubemapNode : PropertyNode, IGeneratesBodyCode, IMayRequireMeshUV
	{
		protected const string kUVSlotName = "Refl";
		protected const string kOutputSlotRGBAName = "RGBA";
		protected const string kOutputSlotRName = "R";
		protected const string kOutputSlotGName = "G";
		protected const string kOutputSlotBName = "B";
		protected const string kOutputSlotAName = "A";

		public const int UvSlotId = 0;
		public const int OutputSlotRgbaId = 1;
		public const int OutputSlotRId = 2;
		public const int OutputSlotGId = 3;
		public const int OutputSlotBId = 4;
		public const int OutputSlotAId = 5;

		[SerializeField]
		private string m_SerializedCube;

		//[SerializeField]
		//private TextureType m_TextureType;

		[Serializable]
		private class CubemapHelper
		{
			public Cubemap cube;
		}

		public override bool hasPreview { get { return true; } }

		#if UNITY_EDITOR
		public Cubemap defaultCube
		{
			get
			{
				if (string.IsNullOrEmpty(m_SerializedCube))
					return null;

				var tex = new CubemapHelper();
				EditorJsonUtility.FromJsonOverwrite(m_SerializedCube, tex);
				return tex.cube;
			}
			set
			{
				if (defaultCube == value)
					return;

				var tex = new CubemapHelper();
				tex.cube = value;
				m_SerializedCube = EditorJsonUtility.ToJson(tex, true);

				if (onModified != null)
				{
					onModified(this, ModificationScope.Node);
				}
			}
		}
		#else
		public Cubemap defaultCube {get; set; }
		#endif

		public CubemapNode()
		{
			name = "Cubemap";
			UpdateNodeAfterDeserialization();
		}

		public sealed override void UpdateNodeAfterDeserialization()
		{
			AddSlot(new MaterialSlot(OutputSlotRgbaId, kOutputSlotRGBAName, kOutputSlotRGBAName, SlotType.Output, SlotValueType.Vector4, Vector4.zero));
			AddSlot(new MaterialSlot(OutputSlotRId, kOutputSlotRName, kOutputSlotRName, SlotType.Output, SlotValueType.Vector1, Vector4.zero));
			AddSlot(new MaterialSlot(OutputSlotGId, kOutputSlotGName, kOutputSlotGName, SlotType.Output, SlotValueType.Vector1, Vector4.zero));
			AddSlot(new MaterialSlot(OutputSlotBId, kOutputSlotBName, kOutputSlotBName, SlotType.Output, SlotValueType.Vector1, Vector4.zero));
			AddSlot(new MaterialSlot(OutputSlotAId, kOutputSlotAName, kOutputSlotAName, SlotType.Output, SlotValueType.Vector1, Vector4.zero));
			AddSlot(new MaterialSlot(UvSlotId, kUVSlotName, kUVSlotName, SlotType.Input, SlotValueType.Vector3, Vector3.zero, false));
			RemoveSlotsNameNotMatching(validSlots);
		}

		public override PreviewMode previewMode {
			get {
				return PreviewMode.Preview3D;
			}
		}

		protected int[] validSlots
		{
			get { return new[] {OutputSlotRgbaId, OutputSlotRId, OutputSlotGId, OutputSlotBId, OutputSlotAId, UvSlotId}; }
		}

		// Node generations
		public virtual void GenerateNodeCode(ShaderGenerator visitor, GenerationMode generationMode)
		{
			var uvSlot = FindInputSlot<MaterialSlot>(UvSlotId);
			if (uvSlot == null)
				return;

			var uvName = string.Format("{0}.xyz", UVChannel.uv0.GetUVName());
			var edges = owner.GetEdges(uvSlot.slotReference).ToList();

			if (edges.Count > 0)
			{
				var edge = edges[0];
				var fromNode = owner.GetNodeFromGuid<AbstractMaterialNode>(edge.outputSlot.nodeGuid);
				uvName = ShaderGenerator.AdaptNodeOutput(fromNode, edge.outputSlot.slotId, ConcreteSlotValueType.Vector3, true);
			}

			string body = "texCUBE (" + propertyName + ", " + uvName + ")";
			visitor.AddShaderChunk(precision + "4 " + GetVariableNameForNode() + " = " + body + ";", true);
		}

		public override string GetVariableNameForSlot(int slotId)
		{
			string slotOutput;
			switch (slotId)
			{
			case OutputSlotRId:
				slotOutput = ".r";
				break;
			case OutputSlotGId:
				slotOutput = ".g";
				break;
			case OutputSlotBId:
				slotOutput = ".b";
				break;
			case OutputSlotAId:
				slotOutput = ".a";
				break;
			default:
				slotOutput = "";
				break;
			}
			return GetVariableNameForNode() + slotOutput;
		}

		public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
		{
			properties.Add(GetPreviewProperty());
		}

		// Properties
		public override void GeneratePropertyBlock(PropertyGenerator visitor, GenerationMode generationMode)
		{
			visitor.AddShaderProperty(
				new CubemapPropertyChunk(
					propertyName,
					description,
					defaultCube,
					PropertyChunk.HideState.Visible,
					exposedState == ExposedState.Exposed ?
					CubemapPropertyChunk.ModifiableState.Modifiable
					: CubemapPropertyChunk.ModifiableState.NonModifiable));
		}

		public override void GeneratePropertyUsages(ShaderGenerator visitor, GenerationMode generationMode)
		{
			visitor.AddShaderChunk("samplerCUBE " + propertyName + ";", true);
		}

		public override PreviewProperty GetPreviewProperty()
		{
			return new PreviewProperty
			{
				m_Name = propertyName,
				m_PropType = PropertyType.Cubemap,
				m_Cubemap = defaultCube
			};
		}

		public override PropertyType propertyType { get { return PropertyType.Cubemap; } }

		public bool RequiresMeshUV(UVChannel channel)
		{
			if (channel != UVChannel.uv0)
			{
				return false;
			}

			var uvSlot = FindInputSlot<MaterialSlot>(UvSlotId);
			if (uvSlot == null)
				return true;

			var edges = owner.GetEdges(uvSlot.slotReference).ToList();
			return edges.Count == 0;
		}
	}
}