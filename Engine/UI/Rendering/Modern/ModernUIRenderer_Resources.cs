/*
File:    ModernUIRenderer_Resources.cs
Folder:  Engine/UI/Rendering/Modern/
Purpose:  Core UI rendering component for SAS Zombie Assault TD.
*/

//============================================================================
//ModernUIRenderer_Resources.cs (Modernized)
//============================================================================

using System;
//
using SASZombieAssaultTD.Engine.Rendering;

using SASZombieAssaultTD.Engine.Diagnostics;

namespace SASZombieAssaultTD.Engine.UI.Rendering.Modern
{
    public partial class ModernUIRenderer
    {
        private UITextureAtlasManager _atlasManager;
        private UIShaderSystem _shaderSystem;
        private RenderResourcePool _resourcePool;

        //--------------------------------------------------------------------
        //INITIALIZATION
        //--------------------------------------------------------------------

        internal void InitializeResources(
            UITextureAtlasManager atlasManager,
            UIShaderSystem shaderSystem,
            RenderResourcePool resourcePool)
        {
            _atlasManager = atlasManager
                ?? throw new ArgumentNullException(nameof(atlasManager));

            _shaderSystem = shaderSystem
                ?? throw new ArgumentNullException(nameof(shaderSystem));

            _resourcePool = resourcePool
                ?? throw new ArgumentNullException(nameof(resourcePool));
        }

        //--------------------------------------------------------------------
        //GET ATLAS TEXTURE (Modern Pipeline)
        //--------------------------------------------------------------------

        ///<summary>
        ///Retrieves a texture from the UI atlas.
        ///</summary>
        public Texture GetAtlasTexture(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var texture = _atlasManager.GetTexture(name);

            if (texture == null)
                //TODO: RenderDiagnostics.Record doesn't exist
                //RenderDiagnostics.Record("ModernUIRenderer", $"Atlas texture '{name}' not found.");

            return texture;
            return null;
        }

        //--------------------------------------------------------------------
        //GET MATERIAL (Modern Pipeline)
        //--------------------------------------------------------------------

        ///<summary>
        ///Retrieves a material from the shader system.
        ///</summary>
        public UIMaterial GetMaterial(string materialName)
        {
            if (string.IsNullOrWhiteSpace(materialName))
                return null;

            var material = _shaderSystem.GetMaterial(materialName);

            if (material == null)
                //TODO: RenderDiagnostics.Record doesn't exist
                //RenderDiagnostics.Record("ModernUIRenderer", $"Material '{materialName}' not found.");

            return material;
            return null;
        }

        //--------------------------------------------------------------------
        //GET RENDER TARGET (Modern Pipeline)
        //--------------------------------------------------------------------

        ///<summary>
        ///Retrieves a render target from the render resource pool.
        ///</summary>
        public IRenderTarget GetRenderTarget(string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
                return null;

            var target = _resourcePool.GetRenderTarget(targetName);

            if (target == null)
                //TODO: RenderDiagnostics.Record doesn't exist
                //RenderDiagnostics.Record("ModernUIRenderer", $"RenderTarget '{targetName}' not found.");

            return target;
            return null;
        }

        //--------------------------------------------------------------------
        //RELEASE RESOURCE (Modern Pipeline)
        //--------------------------------------------------------------------

        public void ReleaseResource(string name)
        {
            RenderDiagnostics.Record("ModernUIRenderer",
                $"ReleaseResource('{name}') called — no manual action required.");
        }
    }

    public class Texture
    {
    }

    internal class RenderResourcePool
    {
        internal IRenderTarget GetRenderTarget(string targetName)
        {
            return NI.Hit<IRenderTarget>();
        }
    }

    internal class UIShaderSystem
    {
        internal IRenderEffect CreateEffect(string v)
        {
            return NI.Hit<IRenderEffect>();
        }

        internal UIMaterial GetMaterial(string materialName)
        {
            return NI.Hit<UIMaterial>();
        }
    }
}
