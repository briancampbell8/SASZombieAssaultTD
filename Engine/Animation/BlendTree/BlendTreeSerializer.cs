using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


using SASZombieAssaultTD.Engine.Diagnostics;
namespace SASZombieAssaultTD.Engine.Animation.BlendTree
//
{
    ///<summary>
    ///P11-18-13: Serialization and deserialization for blend trees with validation and fallback behavior.
    ///Provides deterministic JSON-based serialization with comprehensive error handling.
    ///</summary>
    public static class BlendTreeSerializer
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        public static object DebugLogger { get; private set; }

        ///<summary>
        ///P11-18-13: Serializes a blend tree to JSON string with validation.
        ///Deterministic serialization with comprehensive error handling and validation.
        ///</summary>
        ///<param name="blendTree">Blend tree to serialize</param>
        ///<returns>JSON string representation of the blend tree</returns>
        public static string SerializeBlendTree(BlendTree blendTree)
        {
            if (blendTree == null)
            {
                DLogger.Log(
                    LogSubsystems.Animation, LogLevel.Error, "BlendTreeSerializer: Cannot serialize null blend tree");
                return string.Empty;
            }

            try
            {
                //Validate blend tree before serialization
                var validationResult = ValidateBlendTreeForSerialization(blendTree);
                if (!validationResult.IsValid)
                {
                    LogValidationErrors(blendTree.TreeId, validationResult.Errors);
                    return string.Empty;
                }

                //Convert to serializable format
                var serializableTree = ConvertToSerializableFormat(blendTree);

                //Serialize to JSON
                var json = JsonSerializer.Serialize(serializableTree, _serializerOptions);

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"BlendTreeSerializer: Successfully serialized blend tree '{blendTree.TreeId}'");
                return json;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"BlendTreeSerializer: Error serializing blend tree '{blendTree.TreeId}': {ex.Message}");
                return string.Empty;
            }
        }

        ///<summary>
        ///P11-18-13: Deserializes a blend tree from JSON string with validation and fallback.
        ///Deterministic deserialization with comprehensive error handling and fallback behavior.
        ///</summary>
        ///<param name="json">JSON string to deserialize</param>
        ///<param name="fallbackTree">Optional fallback blend tree if deserialization fails</param>
        ///<returns>Deserialized blend tree or fallback</returns>
        public static BlendTree DeserializeBlendTree(string json, BlendTree? fallbackTree = null)
        {
            if (string.IsNullOrEmpty(json))
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Warning, "BlendTreeSerializer: Cannot deserialize null or empty JSON string");
                return fallbackTree;
            }

            try
            {
                //Deserialize from JSON
                var serializableTree = JsonSerializer.Deserialize<SerializableBlendTree>(json, _serializerOptions);
                if (serializableTree == null)
                {
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Error, "BlendTreeSerializer: Deserialized JSON resulted in null blend tree");
                    return fallbackTree;
                }

                //Convert from serializable format
                var blendTree = ConvertFromSerializableFormat(serializableTree);
                if (blendTree == null)
                {
                    DLogger.Log(LogSubsystems.Animation, LogLevel.Error, "BlendTreeSerializer: Failed to convert from serializable format");
                    return fallbackTree;
                }

                //Validate deserialized blend tree
                var validationResult = ValidateBlendTreeForSerialization(blendTree);
                if (!validationResult.IsValid)
                {
                    LogValidationErrors(blendTree.TreeId, validationResult.Errors);
                    return fallbackTree;
                }

                DLogger.Log(LogSubsystems.Animation, LogLevel.Debug, $"BlendTreeSerializer: Successfully deserialized blend tree '{blendTree.TreeId}'");
                return blendTree;
            }
            catch (JsonException ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"BlendTreeSerializer: JSON error during deserialization: {ex.Message}");
                return fallbackTree;
            }
            catch (Exception ex)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"BlendTreeSerializer: Error deserializing blend tree: {ex.Message}");
                return fallbackTree;
            }
        }

        ///<summary>
        ///P11-18-13: Validates a blend tree for serialization compatibility.
        ///Ensures the blend tree can be safely serialized and deserialized.
        ///</summary>
        ///<param name="blendTree">Blend tree to validate</param>
        ///<returns>Validation result</returns>
        private static BlendNodeValidationResult ValidateBlendTreeForSerialization(BlendTree blendTree)
        {
            if (blendTree == null)
                return new BlendNodeValidationResult(false, new[] { "Blend tree is null" });

            var errors = new List<string>();
            var warnings = new List<string>();

            //Validate tree ID
            if (string.IsNullOrEmpty(blendTree.TreeId))
            {
                errors.Add("Blend tree ID cannot be null or empty");
            }

            //Validate display name
            if (string.IsNullOrEmpty(blendTree.DisplayName))
            {
                errors.Add("Blend tree display name cannot be null or empty");
            }

            //Validate root node
            if (blendTree.RootNode == null)
            {
                errors.Add("Blend tree root node cannot be null");
            }
            else
            {
                //Recursively validate nodes
                ValidateNodeRecursive(blendTree.RootNode, errors, warnings);
            }

            //Validate child nodes
            foreach (var childNode in blendTree.ChildNodes)
            {
                ValidateNodeRecursive(childNode, errors, warnings);
            }

            return new BlendNodeValidationResult(errors.Count == 0, errors.ToArray(), warnings.ToArray());
        }

        ///<summary>
        ///P11-18-13: Recursively validates a blend node and its children.
        ///Ensures all nodes in the tree are valid for serialization.
        ///</summary>
        ///<param name="node">Node to validate</param>
        ///<param name="errors">Error list to populate</param>
        ///<param name="warnings">Warning list to populate</param>
        private static void ValidateNodeRecursive(IBlendNode node, List<string> errors, List<string> warnings)
        {
            if (node == null)
            {
                errors.Add("Null node encountered in blend tree");
                return;
            }

            //Validate node ID
            if (string.IsNullOrEmpty(node.NodeId))
            {
                errors.Add($"Node of type {node.GetType().Name} has null or empty ID");
            }

            //Validate display name
            if (string.IsNullOrEmpty(node.DisplayName))
            {
                warnings.Add($"Node '{node.NodeId}' has null or empty display name");
            }

            //Validate node-specific properties
            var nodeValidation = node.Validate();
            errors.AddRange(nodeValidation.Errors);
            warnings.AddRange(nodeValidation.Warnings);

            //Recursively validate child nodes based on type
            if (node is LinearBlendNode linearNode)
            {
                ValidateNodeRecursive(linearNode.ChildA, errors, warnings);
                ValidateNodeRecursive(linearNode.ChildB, errors, warnings);
            }
            else if (node is TwoDBlendNode twoDNode)
            {
                ValidateNodeRecursive(twoDNode.ChildBottomLeft, errors, warnings);
                ValidateNodeRecursive(twoDNode.ChildBottomRight, errors, warnings);
                ValidateNodeRecursive(twoDNode.ChildTopLeft, errors, warnings);
                ValidateNodeRecursive(twoDNode.ChildTopRight, errors, warnings);
            }
        }

        ///<summary>
        ///P11-18-13: Converts a blend tree to serializable format.
        ///Transforms the runtime blend tree into a JSON-serializable structure.
        ///</summary>
        ///<param name="blendTree">Blend tree to convert</param>
        ///<returns>Serializable blend tree structure</returns>
        private static SerializableBlendTree ConvertToSerializableFormat(BlendTree blendTree)
        {
            var serializableTree = new SerializableBlendTree
            {
                TreeId = blendTree.TreeId,
                DisplayName = blendTree.DisplayName,
                RootNodeId = blendTree.RootNode?.NodeId,
                Nodes = new List<SerializableBlendNode>()
            };

            //Convert all nodes to serializable format
            var processedNodes = new HashSet<string>();
            ConvertNodeRecursive(blendTree.RootNode, processedNodes, serializableTree.Nodes);

            foreach (var childNode in blendTree.ChildNodes)
            {
                ConvertNodeRecursive(childNode, processedNodes, serializableTree.Nodes);
            }

            return serializableTree;
        }

        ///<summary>
        ///P11-18-13: Recursively converts blend nodes to serializable format.
        ///Transforms runtime nodes into JSON-serializable node structures.
        ///</summary>
        ///<param name="node">Node to convert</param>
        ///<param name="processedNodes">Set of already processed node IDs</param>
        ///<param name="serializableNodes">List to add serializable nodes to</param>
        private static void ConvertNodeRecursive(IBlendNode node, HashSet<string> processedNodes, List<SerializableBlendNode> serializableNodes)
        {
            if (node == null || processedNodes.Contains(node.NodeId))
                return;

            processedNodes.Add(node.NodeId);

            var serializableNode = new SerializableBlendNode
            {
                NodeId = node.NodeId,
                DisplayName = node.DisplayName,
                NodeType = node.GetType().Name,
                RequiredParameters = new List<string>(node.RequiredParameters)
            };

            //Convert node-specific properties
            if (node is SingleClipNode singleClipNode)
            {
                serializableNode.ClipId = singleClipNode.ClipId;
            }
            else if (node is LinearBlendNode linearNode)
            {
                serializableNode.BlendParameter = linearNode.BlendParameter;
                serializableNode.ChildAId = linearNode.ChildA?.NodeId;
                serializableNode.ChildBId = linearNode.ChildB?.NodeId;

                //Recursively convert children
                ConvertNodeRecursive(linearNode.ChildA, processedNodes, serializableNodes);
                ConvertNodeRecursive(linearNode.ChildB, processedNodes, serializableNodes);
            }
            else if (node is TwoDBlendNode twoDNode)
            {
                serializableNode.ParameterX = twoDNode.BlendParameterX;
                serializableNode.ParameterY = twoDNode.BlendParameterY;
                serializableNode.ChildBottomLeftId = twoDNode.ChildBottomLeft?.NodeId;
                serializableNode.ChildBottomRightId = twoDNode.ChildBottomRight?.NodeId;
                serializableNode.ChildTopLeftId = twoDNode.ChildTopLeft?.NodeId;
                serializableNode.ChildTopRightId = twoDNode.ChildTopRight?.NodeId;

                //Recursively convert children
                ConvertNodeRecursive(twoDNode.ChildBottomLeft, processedNodes, serializableNodes);
                ConvertNodeRecursive(twoDNode.ChildBottomRight, processedNodes, serializableNodes);
                ConvertNodeRecursive(twoDNode.ChildTopLeft, processedNodes, serializableNodes);
                ConvertNodeRecursive(twoDNode.ChildTopRight, processedNodes, serializableNodes);
            }

            serializableNodes.Add(serializableNode);
        }

        ///<summary>
        ///P11-18-13: Converts a serializable blend tree back to runtime format.
        ///Transforms the JSON-deserialized structure back into a functional blend tree.
        ///</summary>
        ///<param name="serializableTree">Serializable blend tree structure</param>
        ///<returns>Runtime blend tree</returns>
        private static BlendTree ConvertFromSerializableFormat(SerializableBlendTree serializableTree)
        {
            if (serializableTree == null)
                return null;

            var blendTree = new BlendTree(serializableTree.DisplayName);
            var nodeMap = new Dictionary<string, IBlendNode>();

            //First pass: create all nodes
            foreach (var serializableNode in serializableTree.Nodes)
            {
                var node = CreateNodeFromSerializable(serializableNode);
                if (node != null)
                {
                    nodeMap[serializableNode.NodeId] = node;
                    blendTree.AddChildNode(node);
                }
            }

            //Second pass: establish node relationships
            foreach (var serializableNode in serializableTree.Nodes)
            {
                if (serializableNode.NodeType == nameof(LinearBlendNode) &&
                nodeMap.TryGetValue(serializableNode.NodeId, out var linearNode))
                {
                    var typedNode = (LinearBlendNode)linearNode;
                    if (serializableNode.ChildAId != null && nodeMap.TryGetValue(serializableNode.ChildAId, out var childA))
                        typedNode.GetType().GetField("_childA", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childA);
                    if (serializableNode.ChildBId != null && nodeMap.TryGetValue(serializableNode.ChildBId, out var childB))
                        typedNode.GetType().GetField("_childB", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childB);
                }
                else if (serializableNode.NodeType == nameof(TwoDBlendNode) &&
                nodeMap.TryGetValue(serializableNode.NodeId, out var twoDNode))
                {
                    var typedNode = (TwoDBlendNode)twoDNode;
                    if (serializableNode.ChildBottomLeftId != null && nodeMap.TryGetValue(serializableNode.ChildBottomLeftId, out var childBL))
                        typedNode.GetType().GetField("_childBottomLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childBL);
                    if (serializableNode.ChildBottomRightId != null && nodeMap.TryGetValue(serializableNode.ChildBottomRightId, out var childBR))
                        typedNode.GetType().GetField("_childBottomRight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childBR);
                    if (serializableNode.ChildTopLeftId != null && nodeMap.TryGetValue(serializableNode.ChildTopLeftId, out var childTL))
                        typedNode.GetType().GetField("_childTopLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childTL);
                    if (serializableNode.ChildTopRightId != null && nodeMap.TryGetValue(serializableNode.ChildTopRightId, out var childTR))
                        typedNode.GetType().GetField("_childTopRight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(typedNode, childTR);
                }
            }

            //Set root node
            if (!string.IsNullOrEmpty(serializableTree.RootNodeId) &&
            nodeMap.TryGetValue(serializableTree.RootNodeId, out var rootNode))
            {
                blendTree.SetRootNode(rootNode);
            }

            return blendTree;
        }

        ///<summary>
        ///P11-18-13: Creates a runtime node from serializable node data.
        ///Factory method for creating appropriate node types from serialized data.
        ///</summary>
        ///<param name="serializableNode">Serializable node data</param>
        ///<returns>Runtime blend node</returns>
        private static IBlendNode CreateNodeFromSerializable(SerializableBlendNode serializableNode)
        {
            return serializableNode.NodeType switch
            {
                nameof(SingleClipNode) => new SingleClipNode(
                serializableNode.NodeId,
                serializableNode.DisplayName,
                serializableNode.ClipId),

                nameof(LinearBlendNode) => LinearBlendNode.CreateAuto(
                serializableNode.DisplayName,
                serializableNode.BlendParameter,
                null, //Children will be set in second pass
                null),

                nameof(TwoDBlendNode) => TwoDBlendNode.CreateAuto(
                serializableNode.NodeId,
                serializableNode.DisplayName,
                serializableNode.ParameterX,
                serializableNode.ParameterY),

                _ => null
            };
        }

        private static void LogValidationErrors(string treeId, IEnumerable<string> errors)
        {
            DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"BlendTreeSerializer: Blend tree '{treeId}' failed validation");
            foreach (var error in errors)
            {
                DLogger.Log(LogSubsystems.Animation, LogLevel.Error, $"  Validation Error: {error}");
            }
        }
    }

    ///<summary>
    ///P11-18-13: Serializable blend tree structure for JSON serialization.
    ///Deterministic data structure for blend tree serialization.
    ///</summary>
    public class SerializableBlendTree
    {
        public string TreeId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string RootNodeId { get; set; } = string.Empty;
        public List<SerializableBlendNode> Nodes { get; set; } = new();
    }

    ///<summary>
    ///P11-18-13: Serializable blend node structure for JSON serialization.
    ///Deterministic data structure for blend node serialization.
    ///</summary>
    public class SerializableBlendNode
    {
        public string NodeId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string NodeType { get; set; } = string.Empty;
        public List<string> RequiredParameters { get; set; } = new();

        //SingleClipNode properties
        public string ClipId { get; set; } = string.Empty;

        //LinearBlendNode properties
        public string BlendParameter { get; set; } = string.Empty;
        public string ChildAId { get; set; } = string.Empty;
        public string ChildBId { get; set; } = string.Empty;

        //TwoDBlendNode properties
        public string ParameterX { get; set; } = string.Empty;
        public string ParameterY { get; set; } = string.Empty;
        public string ChildBottomLeftId { get; set; } = string.Empty;
        public string ChildBottomRightId { get; set; } = string.Empty;
        public string ChildTopLeftId { get; set; } = string.Empty;
        public string ChildTopRightId { get; set; } = string.Empty;
    }
}







