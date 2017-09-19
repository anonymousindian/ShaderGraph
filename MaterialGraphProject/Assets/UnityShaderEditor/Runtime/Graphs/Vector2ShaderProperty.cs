﻿using System;

namespace UnityEngine.MaterialGraph
{
    [Serializable]
    public class Vector2ShaderProperty : VectorShaderProperty
    {
        public override PropertyType propertyType
        {
            get { return PropertyType.Vector2; }
        }

        public override PreviewProperty GetPreviewMaterialProperty()
        {
            return new PreviewProperty()
            {
                m_Name = name,
                m_PropType = PropertyType.Vector2,
                m_Vector4 = value
            };
        }

    }
}