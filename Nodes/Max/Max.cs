using System.Linq;
using GodotExt;
using JetBrains.Annotations;
using OpenScadGraphEditor.Library;
using OpenScadGraphEditor.Utils;

namespace OpenScadGraphEditor.Nodes.Max
{
    /// <summary>
    /// Maximum node.
    /// </summary>
    [UsedImplicitly]
    public class Max : ScadNode, IAmAnExpression
    {
        public override string NodeTitle => "Max";
        public override string NodeDescription => "Returns the maximum of the input values.";

        public int InputCount { get; private set; } = 1;

        public Max()
        {
            RebuildPorts();
        }

        private void RebuildPorts()
        {
            InputPorts
                .Clear();

            OutputPorts
                .Clear();
            
            OutputPorts
                .Array();

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
        }

        public override string Render(IScadGraph context)
        {
            var parameters = InputCount.Range()
                .Select(it => RenderInput(context, it).OrUndef())
                .JoinToString(", ");

            return $"min({parameters})";
        }
    }
}