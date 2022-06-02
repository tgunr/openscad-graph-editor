using GodotExt;
using JetBrains.Annotations;
using OpenScadGraphEditor.Library;
using OpenScadGraphEditor.Library.IO;

namespace OpenScadGraphEditor.Nodes.IndexVector
{
    /// <summary>
    /// Node which allows to access vector/string indices.
    /// </summary>
    [UsedImplicitly]
    public class IndexVector : ScadNode, IAmAnExpression
    {
        public override string NodeTitle => "Index Vector/String";
        public override string NodeDescription => "Returns the value of the vector/string at the given index";

        public int IndexPortCount { get; private set; } = 1;

        public override string Render(ScadGraph context, int portIndex)
        {
            if (portIndex < 0 || portIndex >= IndexPortCount)
            {
                return "";
            }
            var vector = RenderInput(context, 0);
            var index = RenderInput(context, portIndex+1);
            return $"{vector}[{index}]";
        }
        
        public IndexVector()
        {
            RebuildPorts();
        }

        public override void SaveInto(SavedNode node)
        {
            node.SetData("ports", IndexPortCount);
            base.SaveInto(node);
        }

        public override void RestorePortDefinitions(SavedNode node, IReferenceResolver referenceResolver)
        {
            IndexPortCount = node.GetDataInt("ports", 1);
            RebuildPorts();
            base.RestorePortDefinitions(node, referenceResolver);
        }

        public void IncreasePorts()
        {
            IndexPortCount++;
            RebuildPorts();
            
            // build an input port literal
            BuildPortLiteral(PortId.Input(IndexPortCount));
            // build an output port literal
            BuildPortLiteral(PortId.Output(IndexPortCount-1));
        }

        public void DecreasePorts()
        {
            GdAssert.That(IndexPortCount > 1, "Cannot decrease ports below 1.");
            DropPortLiteral(PortId.Input(IndexPortCount));
            var idx = IndexPortCount - 1;
            DropPortLiteral(PortId.Output(idx));

            IndexPortCount--;
            RebuildPorts();
        }

        private void RebuildPorts()
        {
            InputPorts.Clear();
            OutputPorts.Clear();

            InputPorts
                .Any("Vector/String");
            
            for (var i = 0; i < IndexPortCount; i++)
            {
                InputPorts
                    .Number($"Index {i + 1}");
                OutputPorts
                    .Any($"Value {i + 1}");
            }
        }

        public override string GetPortDocumentation(PortId portId)
        {
            if (portId.IsInput)
            {
                if (portId.Port == 0)
                {
                    return "The vector from which a value should be extracted.";
                }
                return "The index of the value to extract.";
            }

            if (portId.IsOutput)
            {
                return "The value at the given index.";
            }

            return "";
        }


    }
}