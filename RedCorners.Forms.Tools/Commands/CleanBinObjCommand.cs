using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using System.Windows.Interop;
using EnvDTE;
using SuperCleaner;
using System.Collections.Generic;
using System.IO;

namespace RedCorners.Forms.Tools.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CleanBinObjCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4132;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("ad66aa71-34e0-4912-bd09-4141c4bb0395");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;
        public static DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanBinObjCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CleanBinObjCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CleanBinObjCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in CleanBinObjCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CleanBinObjCommand(package, commandService);
            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_dte != null)
            {
                var solution = _dte.Solution;
                if (solution != null)
                {
                    var fullName = solution.FullName;
                    var folders = new List<string>();
                    folders.Add(Path.GetDirectoryName(fullName));
                    foreach (Project p in solution.Projects)
                    {
                        try
                        {
                            folders.Add(Path.GetDirectoryName(p.FileName));
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }

                    solution.Close(true);

                    var dialog = new SuperCleaner.MainWindow();
                    dialog.LoadFolders(folders.ToArray());
                    var hwnd = new IntPtr(_dte.MainWindow.HWnd);
                    var window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
                    dialog.Owner = window;

                    dialog.ShowDialog();
                    _dte.Solution.Open(fullName);
                }
            }
        }
    }
}