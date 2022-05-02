using Godot;

namespace OpenScadGraphEditor.Utils
{
    public static class KeyMap
    {
        private static bool IsKeyPressed(this InputEvent evt, KeyList key)
        {
            if (!evt.IsKeyPress(out var keyEvent))
            {
                return false;
            }

            return keyEvent.Scancode == (ulong) key;
        }


        private static bool IsShiftPressed(this InputEvent evt)
        {
            return IsKeyPress(evt, out var keyEvent) && keyEvent.Shift;
        }
        
        private static bool IsKeyPress(this InputEvent evt, out InputEventKey keyEvent)
        {
            keyEvent = evt as InputEventKey;
            return keyEvent != null && keyEvent.Pressed;
        }

        private static bool IsCmdOrControlPressed(this InputEvent evt)
        {
            if (!IsKeyPress(evt, out var inputEventKey))
            {
                return false;
            }
            
            if (OS.GetName() == "OSX")
            {
                return inputEventKey.Command;
            }

            return inputEventKey.Control;
        }
        
        
        // copy+paste
        public static bool IsCopy(this InputEvent inputEvent) => inputEvent.IsCmdOrControlPressed() && inputEvent.IsKeyPressed(KeyList.C);
        public static bool IsPaste(this InputEvent inputEvent) => inputEvent.IsCmdOrControlPressed() && inputEvent.IsKeyPressed(KeyList.V);
        public static bool IsCut(this InputEvent inputEvent) => inputEvent.IsCmdOrControlPressed() && inputEvent.IsKeyPressed(KeyList.X);
        
        // undo/redo
        public static bool IsUndo(this InputEvent inputEvent) => inputEvent.IsCmdOrControlPressed() && inputEvent.IsKeyPressed(KeyList.Z);
        public static bool IsRedo(this InputEvent inputEvent) => inputEvent.IsCmdOrControlPressed() && inputEvent.IsKeyPressed(KeyList.Z) && inputEvent.IsShiftPressed();
        
        // straighten
        public static bool IsStraighten(this InputEvent inputEvent) => inputEvent.IsKeyPressed(KeyList.Q);
        
    }
}