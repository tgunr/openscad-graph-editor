using System.Linq;
using JetBrains.Annotations;
using OpenScadGraphEditor.Library;
using OpenScadGraphEditor.Refactorings;
using OpenScadGraphEditor.Utils;

namespace OpenScadGraphEditor.Nodes.Max
{
    [UsedImplicitly]
    public class RemoveMaxInputPortRefactoring : UserSelectableNodeRefactoring
    {
        public override string Title => "Remove input port";
        public override int Order => 1;
        public override bool IsApplicableToNode => Node is Max echo && echo.InputPortCount > 1;

        public RemoveMaxInputPortRefactoring(IScadGraph holder, ScadNode node) : base(holder, node)
        {
        }
        
        public override void PerformRefactoring(RefactoringContext context)
        {
            var node = (Max) Node;

            // remove the connection that goes into the port to be removed.
            Holder.GetAllConnections()
                .Where(it => it.IsTo(node, node.InputPortCount))
                .ToList() // make a new list, so we don't change the collection while iterating over it
                .ForAll(it => Holder.RemoveConnection(it));
            
            node.RemoveInput();
        }
    }
}