using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MMORPG.Client.Dungeon
{
    public class PuzzleRoomController : MonoBehaviour
    {
        [SerializeField] private List<PuzzleTriggerPlate> plates = new List<PuzzleTriggerPlate>();
        [SerializeField] private SimpleDoor door;
        [SerializeField] private UnityEvent onPuzzleComplete;

        private bool _completed;

        void Awake()
        {
            if (plates.Count == 0)
                plates.AddRange(GetComponentsInChildren<PuzzleTriggerPlate>());
            if (!door)
                door = GetComponentInChildren<SimpleDoor>();
        }

        public void NotifyPlateActivated(PuzzleTriggerPlate plate)
        {
            if (_completed) return;
            foreach (var p in plates)
                if (p && !p.isActive) return; // not all active yet
            Complete();
        }

        public void Complete()
        {
            if (_completed) return;
            _completed = true;
            if (door) door.Unlock();
            onPuzzleComplete?.Invoke();
        }
    }
}

