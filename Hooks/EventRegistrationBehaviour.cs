using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PainSaber.Hooks
{
    public class EventRegistrationBehaviour : MonoBehaviour
    {
        private static EventRegistrationBehaviour Instance;

        public static void OnLoad()
        {
            if (Instance != null) return;
            GameObject go = new GameObject(nameof(EventRegistrationBehaviour));
            go.AddComponent<EventRegistrationBehaviour>();
        }

        private void Awake() 
        {
            if (Instance != null) return;
            Instance = this;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            PainSaberPlugin.Log.Debug("Attached scene change hook.");   
        }

        private void SceneManagerOnActiveSceneChanged(Scene _, Scene scene)
        {
            if (scene.name != "GameCore") return;

            var gameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();

            if (gameScenesManager != null)
            {
                gameScenesManager.transitionDidFinishEvent -= GameSceneLoadedCallback;
                gameScenesManager.transitionDidFinishEvent += GameSceneLoadedCallback;
                PainSaberPlugin.Log.Debug("Reattached scene loaded hook.");
            }
        }

        private void GameSceneLoadedCallback(ScenesTransitionSetupDataSO transitionSetupData, DiContainer diContainer)
        {
            // Prevent firing this event when returning to menu
            var gameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            if (gameScenesManager == null)
                gameScenesManager.transitionDidFinishEvent -= GameSceneLoadedCallback;
    
            GameSceneSceneWasLoaded(transitionSetupData, diContainer);
        }

        private void GameSceneSceneWasLoaded(ScenesTransitionSetupDataSO _, DiContainer diContainer, MultiplayerController sync = null)
        {
            var beatmapObjectManager = diContainer.TryResolve<BeatmapObjectManager>();

            if (beatmapObjectManager == null) return;

            beatmapObjectManager.noteWasMissedEvent += OnNoteMissed;
            beatmapObjectManager.noteWasCutEvent += OnNoteCut;

            PainSaberPlugin.Log.Debug("Reattached note missed event hook.");
        }

        private void OnNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (noteController is BombNoteController bomb)
            {
                PainSaberPlugin.Log.Debug("Bomb was cut.");
                PainSaberPlugin.BombCut();
                return;
            }

            if (noteCutInfo.failReason != NoteCutInfo.FailReason.None)
            {
                PainSaberPlugin.Log.Debug("Note was cut incorrectly.");
                PainSaberPlugin.NoteIncorrect();
                return;
            }
        }

        private void OnNoteMissed(NoteController noteController) 
        {
            if (noteController is BombNoteController) return;
            
            PainSaberPlugin.Log.Debug("Note was missed.");
            PainSaberPlugin.NoteMissed();
        }
    }
}