﻿using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotExt;

namespace OpenScadGraphEditor.Widgets
{
    [Tool]
    public class Foldout : MarginContainer
    {
        private List<NodePath> _controlButtons = new List<NodePath>();

        
        public event Action<bool> OnFoldoutChanged;
        
        [Export]
        public List<NodePath> ControlButtons
        {
            get => _controlButtons;
            set
            {
                _controlButtons = value;
                UpdateConfigurationWarning();
            }
        }

        public override void _Ready()
        {
            for (var i = 0; i < ControlButtons.Count; i++)
            {
                var button = GetNode<Button>(ControlButtons[i]);
                button.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array {i, button});
            }
        }
        
        public void ShowChild(Control child)
        {
            var index = GetChildren().IndexOf(child);
            GdAssert.That(index > -1, "Tried to show non-existing child");
            var button = GetNode<Button>(ControlButtons[index]);

            child.Visible = true;
            button.Pressed = true;
            
            // hide all other children
            for (var i = 0; i < GetChildCount(); i++)
            {
                if (i == index)
                {
                    continue;
                }
                GetChild<Control>(i).Visible = false;
            }
            
            // mark all other buttons as not pressed
            for (var i = 0; i < ControlButtons.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }
                GetNode<Button>(ControlButtons[i]).Pressed = false;
            }
        }

        private void OnButtonPressed(int index, Button button)
        {
            // set the visibility of the child that corresponds to the button index
            // to the button's pressed state
            if (index < GetChildCount())
            {
                // and update it's visibility
                GetChild<Control>(index).Visible = button.Pressed;
            }
            else
            {
                // if the child doesn't exist, print an error
                GD.PrintErr("Child index out of range");
                return;
            }

            // hide all other children and set the buttons to unpressed
            for (var i = 0; i < GetChildCount(); i++)
            {
                if (i != index)
                {
                    GetChild<Control>(i).Visible = false;
                    GetNode<Button>(ControlButtons[i]).Pressed = false;
                }
            }

            // if no child is visible hide the whole foldout
            Visible = this.GetChildNodes<Control>().Any(child => child.Visible);
            OnFoldoutChanged?.Invoke(Visible);
        }


        public override string _GetConfigurationWarning()
        {
            // the length of the control buttons array must match the number of children
            if (ControlButtons.Count != GetChildCount())
            {
                return "The number of control buttons must match the number of children";
            }

            // the node paths must all contain something
            if (ControlButtons.Any(t => t.IsEmpty()))
            {
                return "All control buttons must be set";
            }

            return "";
        }
    }
}