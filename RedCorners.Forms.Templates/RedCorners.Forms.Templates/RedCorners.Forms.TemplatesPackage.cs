using Microsoft.VisualStudio.Shell;

using System;
using System.Runtime.InteropServices;
using System.Threading;

using Task = System.Threading.Tasks.Task;

namespace RedCorners.Forms.Templates
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("9bc08ef7-9098-4e07-b340-1f193fe6910b")]
    public sealed class RedCornersFormsTemplatesPackage : AsyncPackage
    {
        /// <summary>
        /// RedCorners.Forms.TemplatesPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "17f534aa-4d2b-4061-af85-2a4305a332b9";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        #endregion
    }
}
