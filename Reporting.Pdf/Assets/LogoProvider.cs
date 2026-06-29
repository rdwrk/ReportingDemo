using System;
using System.IO;
using System.Reflection;

namespace Reporting.Pdf.Assets
{
    /// <summary>
    /// Provides the file-system path to the embedded default logo so that
    /// MigraDoc can load it as an image. The PNG is extracted from the
    /// assembly's embedded resources on first access and cached for the
    /// lifetime of the process.
    /// </summary>
    public static class LogoProvider
    {
        private static readonly object _lock    = new object();
        private static string          _tempPath = null;

        /// <summary>
        /// Returns the absolute path to the logo PNG, extracting it from the
        /// embedded resource to a temp file if this is the first call.
        /// </summary>
        public static string GetPath()
        {
            if (_tempPath != null && File.Exists(_tempPath))
                return _tempPath;

            lock (_lock)
            {
                if (_tempPath != null && File.Exists(_tempPath))
                    return _tempPath;

                var assembly     = typeof(LogoProvider).GetTypeInfo().Assembly;
                var resourceName = "Reporting.Pdf.Assets.logo.png";

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new InvalidOperationException(
                            $"Embedded resource '{resourceName}' not found in {assembly.FullName}.");

                    var path = Path.Combine(Path.GetTempPath(), "reporting_demo_logo.png");
                    using (var file = File.Create(path))
                        stream.CopyTo(file);

                    _tempPath = path;
                }
            }

            return _tempPath;
        }
    }
}
