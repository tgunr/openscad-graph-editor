using System.Linq;
using System.Text;
using GodotExt;
using JetBrains.Annotations;
using OpenScadGraphEditor.Library;
using OpenScadGraphEditor.Library.IO;
using OpenScadGraphEditor.Utils;

namespace OpenScadGraphEditor.Nodes.Echo
{
    [UsedImplicitly]
    public class Echo : ScadNode
    {
        public override string NodeTitle => "Echo";
        public override string NodeDescription => "Writes one or more values to the console";

        public int InputCount { get; private set; } = 1;

        public Echo()
        {
            RebuildPorts();
        }

        public override string GetPortDocumentation(PortId portId)
        {
            if (portId.IsInput)
            {
                if (portId.Port == 0)
                {
                    return "Input flow";
                }
                return "A value that will be written to the console";
            }

            if (portId.IsOutput)
            {
                return "Output flow";
            }

            return "";
        }

        private void RebuildPorts()
        {
            InputPorts
                .Clear();
            InputPorts
                .Geometry();

            OutputPorts
                .Clear();
            OutputPorts
                .Geometry();

            for (var i = 0; i < InputCount; i++)
            {
                InputPorts.Any($"Input {i + 1}");
            }

        }

        /// <summary>
        /// Adds a new input. The caller is responsible for fixing up port connections.
        /// </summary>
        public void AddInput()
        {
            InputCount += 1;
            RebuildPorts();
            // since we have no literals here, we can skip re-building port literals
        }

        /// <summary>
        /// Removes an input. The caller is responsible for fixing up port connections.
        /// </summary>
        public void RemoveInput()
        {
            GdAssert.That(InputCount > 1, "Cannot decrease nest inputs any further.");
            InputCount -= 1;
            RebuildPorts();
        }


        public override void SaveInto(SavedNode node)
        {
            node.SetData("input_count", InputCount);
            base.SaveInto(node);
        }

        public override void RestorePortDefinitions(SavedNode node, IReferenceResolver referenceResolver)
        {
            InputCount = node.GetDataInt("input_count", 1);
            RebuildPorts();
            base.RestorePortDefinitions(node, referenceResolver);
        }

        public override string Render(ScadGraph context, int portIndex)
        {
            if (portIndex != 0)
            {
                return "";
            }
            
            var parameters = InputCount.Range()
                .Select(it => RenderInput(context, it + 1).OrUndef())
                .JoinToString(", ");

            var before = RenderInput(context, 0);

            return $"{before}\necho({parameters});";
        }
    }
}