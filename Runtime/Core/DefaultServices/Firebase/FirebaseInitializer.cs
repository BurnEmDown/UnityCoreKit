#if FIREBASE_INSTALLED
using Firebase;
#endif
using System.Threading.Tasks;
using Logger = Core.Utils.Logs.Logger;

namespace Core.DefaultServices.Firebase
{
    /// <summary>
    /// Centralized initializer for Firebase services.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This static helper detects whether Firebase is installed (via the
    /// <c>FIREBASE_INSTALLED</c> scripting define), checks that the required
    /// native dependencies are present, and—if everything is available—
    /// replaces any stub services with their Firebase-backed equivalents.
    /// </para>
    ///
    /// <para><b>What this class does:</b></para>
    /// <list type="bullet">
    ///     <item>Checks and fixes Firebase dependency availability.</item>
    ///     <item>Registers <see cref="FirebaseAnalyticsService"/>,
    ///           <see cref="FirebaseRemoteConfigService"/>,
    ///           and <see cref="FirebaseCrashReportingService"/>.</item>
    ///     <item>Executes remote config initialization during startup.</item>
    ///     <item>Falls back to stub services when Firebase is not installed or
    ///           dependencies cannot be resolved.</item>
    /// </list>
    ///
    /// <para><b>Why this is useful:</b></para>
    /// <list type="bullet">
    ///     <item>Consumer code can always request
    ///         <c>IAnalyticsService</c>, <c>IRemoteConfigService</c>,
    ///         or <c>ICrashReportingService</c> without conditional logic.</item>
    ///     <item>Both Firebase and non-Firebase builds are fully supported.</item>
    ///     <item>You can add Firebase to a project later without refactoring your gameplay code.</item>
    /// </list>
    ///
    /// <para><b>Typical usage:</b></para>
    /// <code>
    /// // Called during startup, BEFORE loading the first game scene:
    /// await FirebaseInitializer.InitializeAndOverrideServicesAsync();
    /// </code>
    ///
    /// <para><b>Safe for builds without Firebase:</b></para>
    /// When the <c>FIREBASE_INSTALLED</c> define is missing, the method completes
    /// immediately and logs a message. Stub implementations remain registered.
    /// </remarks>
    public static class FirebaseInitializer
    {
        /// <summary>
        /// Initializes Firebase, verifies dependencies, and registers Firebase-backed
        /// core services — replacing any previously registered stubs.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that completes when the initialization and service
        /// registration steps are finished.
        /// </returns>
        /// <remarks>
        /// <para><b>Asynchronous behavior:</b></para>
        /// Firebase's dependency check is inherently asynchronous.  
        /// The method should be awaited during your project bootstrap phase.
        ///
        /// <para><b>Service Replacement:</b></para>
        /// On success, the following services are re-registered:
        /// <list type="bullet">
        ///     <item><see cref="FirebaseAnalyticsService"/></item>
        ///     <item><see cref="FirebaseRemoteConfigService"/></item>
        ///     <item><see cref="FirebaseCrashReportingService"/></item>
        /// </list>
        ///
        /// <para><b>If Firebase is NOT installed:</b></para>
        /// The method simply returns a completed task and leaves stub services intact.
        /// </remarks>
        public static async Task InitializeAndOverrideServicesAsync()
        {
#if FIREBASE_INSTALLED
            Logger.Log("[Firebase] Checking and fixing dependencies...");

            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == DependencyStatus.Available)
            {
                Logger.Log("[Firebase] Dependencies available, initializing services...");

                // Replace stub services with Firebase implementations
                Services.Register<IAnalyticsService>(new FirebaseAnalyticsService());

                var remoteConfig = new FirebaseRemoteConfigService();
                Services.Register<IRemoteConfigService>(remoteConfig);

                Services.Register<ICrashReportingService>(new FirebaseCrashReportingService());

                // Initialize remote config explicitly
                await remoteConfig.InitializeAsync();

                Logger.Log("[Firebase] All Firebase services initialized and registered.");
            }
            else
            {
                Logger.LogError($"[Firebase] Could not resolve dependencies: {dependencyStatus}. Using stub services.");
            }
#else
            Logger.Log("[FirebaseInitializer] FIREBASE_INSTALLED not defined. Using stub services.");
            await Task.CompletedTask;
#endif
        }
    }
}