using System.Collections;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Services;
using Core.StubServices;
using Core.DefaultServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Core.Services.CoreServices;

namespace Core
{
    /// <summary>
    /// Bootstraps core services on startup and keeps itself alive
    /// until the first non-bootstrap scene is loaded.
    /// </summary>
    public class Loader : MonoBehaviour
    {
        // Add non-addressable prefabs here via Inspector if they need to be created via factory
        [SerializeField] private Dictionary<string, GameObject> PrefabMap;
        
        // Set the base URL for your backend API here
        [SerializeField] private string baseUrl = "https://api.example.com";

        // Set your api endpoint mappings here for networking
        [SerializeField] private Dictionary<string, string> apiMap;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            RegisterServices();

            // Kick off initialization flow (including async stuff like Firebase)
            StartCoroutine(BootstrapCoroutine());
        }

        /// <summary>
        /// Registers all core services in the service container.
        /// </summary>
        private void RegisterServices()
        {
            // Core event system
            Register<IEventsManager>(() => new EventsManager());
            Register<IEventListenerManager>(() => new EventListenerManager(Get<IEventsManager>()));

            // Object creation (Addressables + non-addressable prefabs)
            Register<IObjectFactory>(() =>
            {
                var addressablesFactory = new AddressablesFactory();
                var prefabFactory = new PrefabFactory(PrefabMap);
                return new CompositeFactory(addressablesFactory, prefabFactory);
            });

            // Pooling depends on IObjectFactory
            Register<IPoolManager>(() => new PoolManager(Get<IObjectFactory>()));

            // Persistence / save system
            Register<ISaveSystem>(() => new JsonFileSaveSystem());

            // Audio & localization (stubs by default)
            Register<IAudioManager>(() => new StubAudioManager());
            Register<ILocalizationService>(() => new StubLocalizationService());

            // Scene loading & time service
            Register<ISceneLoader>(() => new DefaultSceneLoader());
            Register<ITimeService>(() => new DefaultTimeService());
            
            // Networking
            Register<INetworkManager>(() => new DefaultNetworkManager(baseUrl, apiMap));

            // Analytics / remote config / crash reporting (stubs for now)
            Register<IAnalyticsService>(() => new StubAnalyticsService());
            Register<IRemoteConfigService>(() => new StubRemoteConfigService());
            Register<ICrashReportingService>(() => new StubCrashReportingService());
        }

        /// <summary>
        /// Handles any asynchronous initialization (e.g., Firebase) and then 
        /// continues with the next step (such as loading the main menu scene).
        /// </summary>
        private IEnumerator BootstrapCoroutine()
        {
#if FIREBASE_INSTALLED
            // If FirebaseInitializer.InitializeAsync() returns a Task, we can wait on it like this:
            var initTask = FirebaseInitializer.InitializeAsync();
            while (!initTask.IsCompleted)
            {
                yield return null;
            }

            // Optional: handle exceptions
            if (initTask.Exception != null)
            {
                // log or handle errors here
                Debug.LogException(initTask.Exception);
            }
#endif

            OnInitComplete();
            yield return null;
        }

        public virtual void OnInitComplete()
        {
            // TODO: load your first scene here (if Loader lives in a dedicated bootstrap scene)
            // Example:
            // SceneManager.LoadScene("MainMenu");
        }
        
        private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadMode)
        {
            // If this is not the bootstrap scene (index 0), destroy Loader
            if (loadedScene.buildIndex != 0)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}