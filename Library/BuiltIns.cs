using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenScadGraphEditor.Nodes;

namespace OpenScadGraphEditor.Library
{
    /// <summary>
    /// This class contains all built-in modules, functions and nodes.
    /// </summary>
    public static class BuiltIns
    {
        public static IReadOnlyCollection<ModuleDescription> Modules { get; }
        public static IReadOnlyCollection<FunctionDescription> Functions { get; }
        public static IReadOnlyCollection<VariableDescription> Variables { get; }

        public static IReadOnlyCollection<Type> LanguageLevelNodes { get; }

        static BuiltIns()
        {
            LanguageLevelNodes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(ScadNode).IsAssignableFrom(t) && !t.IsAbstract)
                    // only nodes which can be directly created
                    // TODO: make a proper marker interface for language level nodes. 
                    .Where(it => !typeof(ICannotBeCreated).IsAssignableFrom(it))
                    .ToList();

            
            Modules = new List<ModuleDescription>()
            {
                // TODO: add remaining built-in modules
                ModuleBuilder.NewBuiltInModule("cube", "Cube")
                    .WithDescription("Creates a cube in the first octant.\nWhen center is true, the cube is\ncentered on the origin.")
                    .WithParameter("size", PortType.Vector3, label: "Size")
                    .WithParameter("center", PortType.Boolean, label: "Center")
                    .Build(),
                ModuleBuilder.NewBuiltInModule("translate", "Translate")
                    .WithDescription("Translates (moves) its child\nelements along the specified offset.")
                    .WithParameter("v", PortType.Vector3, label: "Offset")
                    .WithChildren()
                    .Build(),
                ModuleBuilder.NewBuiltInModule("rotate", "Rotate (Axis/Angle)")
                    .WithDescription("Rotates the next elements\nalong the given axis and angle.")
                    .WithParameter("v", PortType.Vector3, label: "Axis")
                    .WithParameter("a", PortType.Number, label: "Angle")
                    .WithChildren()
                    .Build(),
                ModuleBuilder.NewBuiltInModule("rotate", "Rotate (Euler Angles)")
                    .WithDescription("Rotates the next elements\nalong the given Euler angles.")
                    .WithParameter("a", PortType.Vector3, label: "Euler Angles")
                    .WithChildren()
                    .Build(),
                ModuleBuilder.NewBuiltInModule("linear_extrude", "Linear Extrude")
                    .WithDescription("Linear Extrusion is an operation that takes a 2D object as input and generates a 3D object as a result.")
                    .WithParameter("height", PortType.Number, label: "Height")
                    .WithParameter("center", PortType.Boolean, label: "Center")
                    .WithParameter("convexity", PortType.Number, label: "Convexity")
                    .WithParameter("twist", PortType.Number, label: "Twist")
                    .WithParameter("slices", PortType.Number, label: "Slices")
                    .WithParameter("scale", PortType.Vector3, label: "Scale")
                    .WithParameter("$fn", PortType.Number, label: "$fn")
                    .WithChildren()
                    .Build(),
                ModuleBuilder.NewBuiltInModule("color", "Color")
                    .WithDescription("Sets the color of the next elements.")
                    .WithParameter("c", PortType.Any, label: "Color")
                    .WithChildren()
                    .Build(),
            };
            
            Functions = new List<FunctionDescription>()
            {
                // TODO: add remaining built-in functions
              FunctionBuilder.NewBuiltInFunction("abs", "Abs", PortType.Number)
                  .WithDescription("Returns the absolute value of a number.")
                  .WithParameter("number", PortType.Number)
                  .Build()
            };

            Variables = new List<VariableDescription>();
        }
    }
}