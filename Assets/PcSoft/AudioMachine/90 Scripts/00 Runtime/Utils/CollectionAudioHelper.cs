using System;
using PcSoft.AudioMachine._90_Scripts._00_Runtime.Assets;
using PcSoft.AudioMachine._90_Scripts._00_Runtime.Utils.Extensions;

namespace PcSoft.AudioMachine._90_Scripts._00_Runtime.Utils
{
    internal sealed class CollectionAudioHelper<T>
    {
        private readonly CollectionAudioBehavior _behavior;
        private int _index = -1;
        private readonly T[] _collection;
        private T _current;

        public CollectionAudioHelper(CollectionAudioBehavior behavior, T[] collection)
        {
            _behavior = behavior;
            _collection = collection;
        }

        public T Next()
        {
            T randomSource;
            switch (_behavior)
            {
                case CollectionAudioBehavior.PlayForward:
                    _index++;
                    if (_index >= _collection.Length)
                    {
                        _index = 0;
                    }

                    randomSource = _collection[_index];
                    break;
                case CollectionAudioBehavior.PlayBackward:
                    _index--;
                    if (_index < 0)
                    {
                        _index = _collection.Length - 1;
                    }

                    randomSource = _collection[_index];
                    break;
                case CollectionAudioBehavior.PlayRandom:
                    randomSource = _collection.GetRandomItem();
                    break;
                case CollectionAudioBehavior.PlayRandomNoDoublet:
                    randomSource = _collection.GetRandomItem(_current);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            if (randomSource == null)
                throw new InvalidOperationException("Collection is empty");

            _current = randomSource;

            return randomSource;
        }
    }
}