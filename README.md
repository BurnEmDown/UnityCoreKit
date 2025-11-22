# UnityCoreKit

A lightweight, modular, production-ready foundation for starting new Unity games.

---

## 🚀 Getting Started

UnityCoreKit is designed to be used as a **Git submodule** inside your Unity projects.  
It is **not** a standalone Unity project – it only contains reusable core scripts and services.

---

### 1️⃣ Add UnityCoreKit as a submodule to your Unity project

From your Unity project root (the folder that contains `Assets/`, `ProjectSettings/`, etc.):

```bash
cd /path/to/YourUnityProject

git submodule add https://github.com/BurnEmDown/UnityCoreKit.git Assets/UnityCoreKit
git commit -m "Add UnityCoreKit as submodule"
```
This will create:
```
YourUnityProject/
  Assets/
    UnityCoreKit/   ← shared core code (submodule)
```
Unity will compile everything in Assets/UnityCoreKit like normal scripts.

### 2️⃣ Install required Unity packages

UnityCoreKit depends on a few Unity packages that your project must have:
  - Newtonsoft Json (com.unity.nuget.newtonsoft-json)
  - Addressables (com.unity.addressables)

You can install them via Package Manager:
1. Open Window → Package Manager
2. In the top-left dropdown, select Unity Registry
3. Search for:
  - Newtonsoft Json → Install
  - Addressables → Install

Or add them manually to Packages/manifest.json:
```
{
  "dependencies": {
    "com.unity.nuget.newtonsoft-json": "3.2.1",
    "com.unity.addressables": "1.21.21"
    // ...other dependencies...
  }
}
```
(Use versions that are verified/recommended for your Unity version.)

### 3️⃣ Open the project in Unity

1. Open your Unity project (or re-open it if it was already open).
2. Unity will import and compile the scripts in Assets/UnityCoreKit.
3. You should now be able to use namespaces like:
```
using Core.Interfaces;
using Core.Services;
using Core.Utils;
```
in your own game code.

### 4️⃣ Register services in your bootstrap/loader
In your own Loader or bootstrap MonoBehaviour (created in your project), you can wire up UnityCoreKit services, for example:
```
using Core.Interfaces;
using Core.Services;
using Core.DefaultServices;
using Core.StubServices;
using UnityEngine;
using static Core.Services.CoreServices;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Core event system
        Register<IEventsManager>(() => new EventsManager());
        Register<IEventListenerManager>(() => new EventListenerManager(Get<IEventsManager>));

        // Object factory (Addressables + prefab map)
        var prefabMap = new System.Collections.Generic.Dictionary<string, GameObject>();
        Register<IObjectFactory>(() => new CompositeFactory(
            new AddressablesFactory(),
            new PrefabFactory(prefabMap)));

        // Pooling
        Register<IPoolManager>(() => new PoolManager(Get<IObjectFactory>()));

        // Save system
        Register<ISaveSystem>(() => new JsonFileSaveSystem());

        // Time, scenes, audio, localization
        Register<ITimeService>(() => new DefaultTimeService());
        Register<ISceneLoader>(() => new DefaultSceneLoader());
        Register<IAudioManager>(() => new StubAudioManager());
        Register<ILocalizationService>(() => new StubLocalizationService());

        // Analytics / remote config / crash reporting (stubs by default)
        Register<IAnalyticsService>(() => new StubAnalyticsService());
        Register<IRemoteConfigService>(() => new StubRemoteConfigService());
        Register<ICrashReportingService>(() => new StubCrashReportingService());

        // Optionally: hook Firebase if present (not required)
        // await FirebaseInitializer.InitializeAndOverrideServicesAsync();
    }
}

```
Adjust this to your project’s needs (e.g., real audio, real analytics, Firebase, etc.).

### 5️⃣ Use services in gameplay code
```
var audio = Services.Get<IAudioManager>();
audio.PlaySfx("button_click");

var saves = Services.Get<ISaveSystem>();
saves.Save("player", new PlayerData { Level = 3 });

var events = Services.Get<IEventsManager>();
events.InvokeEvent(GameEventType.LevelCompleted, null);

var timeService = Services.Get<ITimeService>();
if (pauseRequested)
{
    timeService.Pause();
}
```

---
## ✨ Features Overview

### 🧩 Modular Service Architecture

A central Services registry acts as a lightweight DI-style container.

Supports:

- Direct instance registration
- Lazy factory registration via Register(() => new MyService())
- Automatic caching and resolution
- Includes interfaces for:
  - IAnalyticsService
  - ICrashReportingService
  - IRemoteConfigService
  - IAudioManager
  - ILocalizationService
  - ISaveSystem
  - ITimeService
  - ISceneLoader
  - IPoolManager
  - IEventsManager
  - IEventListenerManager
  - IObjectFactory
  - INetworkManager
-Each service has:
  - A default stub implementation for development
  - The ability to easily swap in real implementations (Firebase, Addressables, custom audio, etc.)

## ⚙️ Core Systems

### 🔧 Object Pooling

A flexible PoolManager with:
- Async object creation via IObjectFactory
- Main-thread-safe instantiation using a custom dispatcher
- Support for both Addressables and plain prefab instantiation
- Persistent root holder object (PoolsHolder)

General flow:
- Use IObjectFactory to create objects (Addressables or prefabs)
- PoolManager stores and reuses them
- GetFromPool / ReturnToPool handle lifecycle

### 🏭 Factory System

Out-of-the-box factories:
AddressablesFactory
- Loads and caches Addressable-prefab assets
- Instantiates them as needed
- Returns components of type T
PrefabFactory
- Uses a Dictionary<string, GameObject> as a prefab map
- Instantiates from the map with type safety checks
CompositeFactory
- Wraps both factories
- Decides which one to use (e.g., based on key conventions like "addr:" prefix)
- All are accessed via the IObjectFactory interface:
```
var factory = Services.Get<IObjectFactory>();
var enemy = await factory.CreateAsync<Enemy>("addr:EnemyBasic");
```

### 🔥 Events System

A lightweight, thread-safe event bus with:
- IEventsManager for registering and invoking events
- IEventListenerManager for tracking which object listens to which events
- Central dictionary of event types to action lists
- Concurrent queue for safe modifications while events are processed

Example usage:
```
var eventsManager = Services.Get<IEventsManager>();
eventsManager.AddListener(GameEventType.PlayerDied, data => {
    // React to player death
});

eventsManager.InvokeEvent(GameEventType.PlayerDied, null);
```

EventListenerManager helps automatically register/unregister listeners, preventing leaks.

## 📁 Save System (JSON + Newtonsoft)

A JSON-based save system built on ISaveSystem, backed by JsonFileSaveSystem.
Features:
- Save<T>(string slot, T data)
- Load<T>(string slot, T defaultValue = default)
- HasSave(string slot)
- Extended methods (if you choose to use them):
- Task SaveAsync<T>(...)
- Task<T> LoadAsync<T>(...)
- Delete(string slot)
- DeleteAll()
- IEnumerable<string> GetAllSlots()
- Version-aware containers

Implementation details:
- Uses Application.persistentDataPath
- One JSON file per “slot”
- Version field to support migrations later

Example:
```
var saveSystem = Services.Get<ISaveSystem>();
saveSystem.Save("player", new PlayerData { Level = 3 });
var loaded = saveSystem.Load("player", new PlayerData());
```

## 🎵 Audio Service (Stub)

IAudioManager default implementation:
- Logs calls (play/stop/pause/etc.) via the Logger
- Maintains SFX and music volume
- Offers stubbed methods such as:
  - PlaySfx
  - PlayMusic
  - StopMusic
  - PauseMusic
  - ResumeMusic
  - MuteAll
  - IsMusicPlaying, IsSfxPlaying

You can later replace StubAudioManager with:
- A mixer-based system
- An Addressables-backed audio provider
- Third-party audio integration

## 🌍 Localization Service (Stub)

ILocalizationService default implementation:
- Returns the key itself ("menu.play" → "menu.play")
- Logs calls in editor/dev
- Supports formatted strings via string.Format
- Allows overrides at runtime for UI prototyping

Example:
```
var loc = Services.Get<ILocalizationService>();
titleLabel.text = loc.Get("ui.title");
```

## ⏱️ Time Service

ITimeService offers:
- DeltaTime
- UnscaledDeltaTime
- TimeScale property
- Pause() / Resume()

Backed by a DefaultTimeService that wraps Unity’s Time API.
This centralizes time logic and makes it easier to pause or slow down gameplay systems consistently.

## 🔁 Update Managers

Three update managers:
- UpdateManager
- FixedUpdateManager
- LateUpdateManager

Each manages a list of observers:
```
public interface IUpdateObserver
{
    void ObservedUpdate();
}
```

```
public interface IFixedUpdateObserver
{
    void ObservedFixedUpdate();
}
```

```
public interface ILateUpdateObserver
{
    void ObservedLateUpdate();
}
```

Gameplay systems can implement these interfaces and register/unregister themselves:

```
UpdateManager.RegisterObserver(this);
UpdateManager.UnregisterObserver(this);
```

Advantages:

- Central place to manage update loops
- Easy to debug
- Can throttle or control update order if needed

## 📜 Logging System

A simple but flexible Logger:
- Wraps Debug.Log, Debug.LogWarning, Debug.LogError, Debug.LogException
- Uses [Conditional("LOGS")] to compile logs only when LOGS is defined
- Used consistently in core systems and Firebase integrations
- This keeps logging centralized and easy to disable in production builds.

## 🧵 Async & Main-Thread Utilities

MainThreadDispatcher:
- Auto-initialized singleton MonoBehaviour (via RuntimeInitializeOnLoadMethod)
- Holds a thread-safe queue (ConcurrentQueue<Action>)
- Executes all queued actions in Update()

TaskExtensions:
- ContinueWithOnMainThread(this Task<T>, Action<Task<T>> continuation)
- ContinueWithOnMainThread(this Task, Action<Task> continuation)

These extensions enqueue continuation callbacks via MainThreadDispatcher, allowing you to safely interact with Unity objects after async calls or background tasks.

# 🔥 Firebase Integration (Optional)

All Firebase-related default services are located in:

Core/DefaultServices/Firebase/

Included:

- FirebaseAnalyticsService (IAnalyticsService)
- FirebaseCrashReportingService (ICrashReportingService)
- FirebaseRemoteConfigService (IRemoteConfigService)
- FirebaseInitializer (central entry point)

Behavior

If Firebase is installed and the scripting define FIREBASE_INSTALLED is set:
```
FirebaseInitializer.InitializeAndOverrideServicesAsync():
```
- Checks Firebase dependencies
- Registers Firebase-based Analytics, Crashlytics, and Remote Config services
- Initializes Remote Config (defaults + fetch + activate)

If Firebase is not installed:
- Stub services remain active
- No compile-time or runtime errors
- Logs indicate stub behavior

Setup Steps

1. Install Firebase packages (Analytics / Crashlytics / Remote Config).
2. Add scripting define symbol:
FIREBASE_INSTALLED

3. In your bootstrap flow, call:
```
await FirebaseInitializer.InitializeAndOverrideServicesAsync();
```

Your gameplay code always uses the interfaces (IAnalyticsService, ICrashReportingService, IRemoteConfigService) and doesn’t need to care whether Firebase is present.

## 🧱 Architecture Principles

Service-Oriented Design
- All core systems are accessed via interfaces.
- Concrete implementations are registered in one place (e.g., a Loader/bootstrapper).

Clean Assembly Definitions
- Core, Utils, Services, Editor utilities, etc. are split into .asmdef files.
- Faster compiles and clear separation of runtime vs editor code.
- Avoids accidental use of UnityEditor in runtime assemblies.

Reusable Across Projects
- Everything under /Core is project-agnostic.
- You can reuse this as a template for any new project.

Minimal External Dependencies
- Uses Newtonsoft.Json for the save system.
- Firebase is entirely optional and isolated in default services.
- No heavy DI frameworks (no Zenject required).

---


## 🗂 Folder Structure (Simplified)

A typical layout:

```
Assets/
  Scripts/
    Core/
      Interfaces/
      Services/
      DefaultServices/
        Firebase/
      Events/
      Logs/
      Utils/
        Extensions/
        Editor/
      UpdateManagers/
    Gameplay/
    Editor/
```

/Core is intended to be reused as-is between projects.

## 📜 License

UnityStarterKit is released under the MIT License.
You are free to use it in both commercial and non-commercial projects.

## 🤝 Contributing

Issues, feature requests, and PRs are welcome.

If you end up using UnityStarterKit in a released game, sharing a link back is always appreciated, but not required.
