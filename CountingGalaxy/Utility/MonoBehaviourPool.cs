using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utility
{
    public class MonoBehaviourPool<T> where T : MonoBehaviour
    {
        private event Action<T> OnObjectCreated;

        private readonly Queue<T> inactiveObjects;
        private readonly List<T> activeObjects;
        private readonly T objectPrefab;
        private readonly Transform objectsParent;
        private readonly bool isInitialized;

        public List<T> ActiveObjects => activeObjects;

        public MonoBehaviourPool(T _objectPrefab, int _initialObjectsCount, Transform _objectsParent = null, Action<T> _onObjectCreated = null)
        {
            inactiveObjects = new Queue<T>();
            activeObjects = new List<T>();
            objectPrefab = _objectPrefab;
            objectsParent = _objectsParent;
            OnObjectCreated = _onObjectCreated;

            for (int i = 0; i < _initialObjectsCount; i++)
            {
                T _object = CreateObject();
                _object.gameObject.SetActive(false);
                inactiveObjects.Enqueue(_object);
            }
            isInitialized = true;
        }

        public T GetFromPool()
        {
            if (!isInitialized)
            {
                Debug.LogError("Pool isn't initialized. Call the constructor first");
                return null;
            }

            T _object;
            if (inactiveObjects.Count > 0)
            {
                _object = inactiveObjects.Dequeue();
            }
            else
            {
                _object = CreateObject();
            }

            _object.gameObject.SetActive(true);
            activeObjects.Add(_object);
            return _object;
        }

        public void ReturnToPool(T _object)
        {
            if (!_object || !isInitialized)
            {
                return;
            }

            _object.gameObject.SetActive(false);
            activeObjects.Remove(_object);
            inactiveObjects.Enqueue(_object);
        }

        private T CreateObject()
        {
            T _newObject = Object.Instantiate(objectPrefab, objectsParent);
            OnObjectCreated?.Invoke(_newObject);
            return _newObject;
        }
    }
}