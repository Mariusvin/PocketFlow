using System;
using System.Collections.Generic;

namespace Utility.Extensions
{
    public static class StackExtensions
    {
        /// <summary>
        /// Pushes multiple items onto the stack.
        /// </summary>
        public static void PushMany<T>(this Stack<T> _stack, IEnumerable<T> _items)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }

            if (_items == null)
            {
                throw new ArgumentNullException(nameof(_items));
            }

            foreach (T item in _items)
            {
                _stack.Push(item);
            }
        }
        
        /// <summary>
        /// Pushes T _item onto the stack _count times.
        /// </summary>
        public static void PushMany<T>(this Stack<T> _stack, T _item, int _count)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }
            
            if (_count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_count), "Count cannot be negative.");
            }
            
            for (int i = 0; i < _count; i++)
            {
                _stack.Push(_item);
            }
        }

        /// <summary>
        /// Pops multiple items from the stack and returns them as a list.
        /// </summary>
        public static List<T> PopMany<T>(this Stack<T> _stack, int _count)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }

            if (_count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_count), "Count cannot be negative.");
            }

            List<T> _result = new();
            for (int i = 0; i < _count && _stack.Count > 0; i++)
            {
                _result.Add(_stack.Pop());
            }
            
            return _result;
        }

        /// <summary>
        /// Pops an item from the stack safely, returning a default value if empty.
        /// </summary>
        public static T PopOrDefault<T>(this Stack<T> _stack, T _defaultValue = default)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }

            return _stack.Count > 0 ? _stack.Pop() : _defaultValue;
        }

        /// <summary>
        /// Peeks at the top of the stack safely, returning a default value if empty.
        /// </summary>
        public static T PeekOrDefault<T>(this Stack<T> _stack, T _defaultValue = default)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }

            return _stack.Count > 0 ? _stack.Peek() : _defaultValue;
        }

        /// <summary>
        /// Safely checks whether a stack contains a value. Returns false if stack is null.
        /// </summary>
        public static bool ContainsSafe<T>(this Stack<T> _stack, T _value)
        {
            return _stack?.Contains(_value) ?? false;
        }

        /// <summary>
        /// Clears the stack safely.
        /// </summary>
        public static void SafeClear<T>(this Stack<T> _stack)
        {
            if (_stack == null)
            {
                throw new ArgumentNullException(nameof(_stack));
            }

            _stack.Clear();
        }
    }
}
