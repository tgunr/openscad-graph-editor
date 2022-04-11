using System;
using Godot;
using GodotExt;
using JetBrains.Annotations;
using OpenScadGraphEditor.Library.External;
using OpenScadGraphEditor.Refactorings;

namespace OpenScadGraphEditor.Widgets.ImportDialog
{
    public class ImportDialog : WindowDialog
    {
        private OptionButton _importModeOptionButton;
        private OptionButton _pathModeOptionButton;
        private OptionButton _libraryFileOptionButton;
        private LineEdit _pathLineEdit;
        private FileDialog _fileDialog;
        private Control _fileSelectBox;
        private Control _fileLabel;
        private Control _libraryFileLabel;
        private Control _libraryFileBox;
        
        [CanBeNull]
        private string _currentProjectPath;

        private string[] _allLibraryFiles;
        private Button _okButton;

        public event Action<Refactoring[]> RefactoringsRequested;
        
        private enum  ImportMode
        {
            Use,
            Include
        }


        public override void _Ready()
        {

            _fileDialog = new FileDialog();
            _fileDialog.MoveToNewParent(GetParent());
            _fileDialog.Mode = FileDialog.ModeEnum.OpenFile;
            _fileDialog.Filters = new[] {"*.scad; OpenSCAD files"};

            _fileDialog
                .Connect("file_selected")
                .To(this, nameof(OnFileSelected));
            
            _fileSelectBox = this.AtPath<Control>("FileSelectBox");
            _fileLabel = this.AtPath<Control>("FileLabel");
            _libraryFileLabel = this.AtPath<Control>("LibraryFileLabel");
            _libraryFileBox = this.AtPath<Control>("LibraryFileBox");

            _libraryFileOptionButton = this.AtPath<OptionButton>("LibraryFileOptionButton");
            _importModeOptionButton = this.WithName<OptionButton>("ImportModeOptionButton");
            _pathModeOptionButton = this.WithName<OptionButton>("PathModeOptionButton");
            
            _importModeOptionButton.AddItem("Use", (int) ImportMode.Use );
            _importModeOptionButton.AddItem("Include", (int) ImportMode.Include );

            _pathModeOptionButton.AddItem("Relative", (int) ExternalFilePathMode.Relative );
            _pathModeOptionButton.AddItem("Absolute", (int) ExternalFilePathMode.Absolute );
            _pathModeOptionButton.AddItem("Library", (int) ExternalFilePathMode.Library );

            this.WithName<Button>("LibraryFileRefreshButton")
                .Connect("pressed")
                .To(this, nameof(RefreshLibraryFiles));
                
            this.WithName<Button>("CancelButton")
                .Connect("pressed")
                .To(this, nameof(OnCancelButtonPressed));

            _okButton = this.WithName<Button>("OkButton");
            _okButton
                .Connect("pressed")
                .To(this, nameof(OnOkButtonPressed));
            
            
            this.WithName<Button>("SelectButton")
                .Connect("pressed")
                .To(this, nameof(OnSelectButtonPressed));

            _pathLineEdit = this.WithName<LineEdit>("PathLineEdit");
            _pathLineEdit
                .Connect("text_changed")
                .To(this, nameof(OnPathLineEditTextChanged));
            
            
        }


        public void OpenForNewImport([CanBeNull] string currentProjectPath)
        {
            _currentProjectPath = currentProjectPath;
            _importModeOptionButton.Selected = 0;
            _pathModeOptionButton.Selected = 0;
            
            // if the current project path is not yet known, do not allow relative paths
            if (string.IsNullOrEmpty(currentProjectPath))
            {
                _pathModeOptionButton.Selected = 1;
                _pathModeOptionButton.SetItemDisabled(0, true);
                _pathModeOptionButton.SetItemText(0, "Relative (disabled, save project first)");
            }
            else
            {
                _pathModeOptionButton.SetItemDisabled(0, false);
                _pathModeOptionButton.SetItemText(0, "Relative");
            }

            RefreshLibraryFiles();
            RefreshUi();
            PopupCentered();
        }
        
        
        private void OnPathLineEditTextChanged([UsedImplicitly] string _)
        {
            RefreshUi();
        }


        private void RefreshUi()
        {
            var pathMode = (ExternalFilePathMode) _pathModeOptionButton.GetSelectedId();
            _fileLabel.Visible = pathMode != ExternalFilePathMode.Library;
            _fileSelectBox.Visible = pathMode != ExternalFilePathMode.Library;
            
            _libraryFileLabel.Visible = pathMode == ExternalFilePathMode.Library;
            _libraryFileBox.Visible =  pathMode == ExternalFilePathMode.Library;


            if (pathMode == ExternalFilePathMode.Library)
            {
                // need to select a library file 
                _okButton.Disabled = _libraryFileOptionButton.Selected == -1;
            }
            else
            {
                // if we are file mode, we need to have a file selected
                var file = _pathLineEdit.Text;
                if (string.IsNullOrEmpty(file))
                {
                    _okButton.Disabled = true;
                }
                else
                {
                    // if we are in relative mode, try if we can resolve the file relative to the project
                    // file. This should always work on Linux or Mac but on Windows thanks to having drive letters
                    // it may not be possible. 

                    if (pathMode == ExternalFilePathMode.Relative)
                    {
                        var canResolve = PathResolver.TryAbsoluteToRelative(_pathLineEdit.Text, _currentProjectPath, out  _);
                        _okButton.Disabled = !canResolve;
                    }
                    else
                    {
                        _okButton.Disabled = false;
                    }
                }
            }
        }
        
        private void OnOkButtonPressed()
        {
        }
        
        private void OnSelectButtonPressed()
        {
            // if we already have a file selected, use the file's directory as the initial directory
            var file = _pathLineEdit.Text;
            if (!string.IsNullOrEmpty(file))
            {
                var dir = PathResolver.DirectoryFromFile(file);
                _fileDialog.CurrentDir = dir;
            }
            
            // if not, try to run with the current project's path
            else if (!string.IsNullOrEmpty(_currentProjectPath))
            {
                _fileDialog.CurrentDir = _currentProjectPath;
            }
            else
            {
                // run with the user's documents folder
                _fileDialog.CurrentDir = PathResolver.GetUsersDocumentsFolder();
            }
            
            _fileDialog.PopupCentered();
        }


        private void OnFileSelected(string path)
        {
            _pathLineEdit.Text = path;
        }
        
        private void RefreshLibraryFiles()
        {
            _libraryFileOptionButton.Clear();
            _allLibraryFiles = PathResolver.GetAllLibraryFiles();
            foreach (var libraryFile in _allLibraryFiles)
            {
                _libraryFileOptionButton.AddItem(libraryFile);
            }
            
            // add a dummy entry in case no library files exist
            if (_allLibraryFiles.Length == 0)
            {
                _libraryFileOptionButton.AddItem("<No library files found>");
                _libraryFileOptionButton.SetItemDisabled(0, true);
            }

            _libraryFileOptionButton.Selected = -1;
        }
        
        private void OnCancelButtonPressed()
        {
            Hide();
        }
    }
}