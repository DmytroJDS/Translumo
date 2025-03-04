﻿using System;
using System.Collections.Generic;
using System.Linq;
using Translumo.Infrastructure.Collections;
using Translumo.Utils.Extensions;

namespace Translumo.Processing.TextProcessing
{
    public class TextResultCacheService
    {
        public int CacheCapacity
        {
            get => _cachedTexts.Capacity;
            set => _cachedTexts.Capacity = value;
        }

        public int CacheLifeTimeMs { get; set; } = 3000;

        private const int CACHE_DEF_CAPACITY = 30;
        private const int CACHE_TRANSLATED_DEF_CAPACITY = 6;
        private const double SIMILARITY_THRESHOLD = 0.7;
        private const double TRANSLATED_SIMILARITY_THRESHOLD = 0.955;

        private DateTime? _cachedDateTime;
        private Guid _iterationId = Guid.NewGuid();

        private readonly LimitedDictionary<string, float> _cachedTexts;
        private readonly List<KeyValuePair<string, float>> _cachedTextsCurrentIteration;
        private readonly LimitedQueue<(string, Guid)> _cachedTranslated;

        public TextResultCacheService()
        {
            _cachedTranslated = new LimitedQueue<(string, Guid)>(CACHE_TRANSLATED_DEF_CAPACITY);
            _cachedTexts = new LimitedDictionary<string, float>(CACHE_DEF_CAPACITY, StringComparer.OrdinalIgnoreCase);
            _cachedTextsCurrentIteration = new List<KeyValuePair<string, float>>(CACHE_DEF_CAPACITY);
        }

        public bool IsCached(string text)
        {
            bool isCached = IsCachedInternal(text);
            _cachedTextsCurrentIteration.Add(new KeyValuePair<string, float>(text, default(float)));

            return isCached;
        }

        public bool IsCached(string text, float score, out Guid iterationId)
        {
            try
            {
                if (IsCachedInternal(text))
                {
                    return true;
                }

                var isCached = HasBetterSimilarText(text, score);
                _cachedTextsCurrentIteration.Add(new KeyValuePair<string, float>(text, score));

                return isCached;
            }
            finally
            {
                iterationId = _iterationId;
            }
        }

        public bool IsTranslatedCached(string text, Guid iterationId)
        {
            var hasSimilarity = _cachedTranslated.Any(cached => 
                cached.Item2 == iterationId && cached.Item1.GetSimilarity(text) > TRANSLATED_SIMILARITY_THRESHOLD);
            if (!hasSimilarity)
            {
                _cachedTranslated.Enqueue((text, iterationId));
            }

            return hasSimilarity;
        }

        public void EndIteration()
        {
            _cachedTextsCurrentIteration.ForEach(AddCachedText);
            _cachedTextsCurrentIteration.Clear();
        }

        public void Reset()
        {
            _cachedTextsCurrentIteration.Clear();
            _cachedTexts.Clear();
            _cachedTranslated.Clear();
            _iterationId = Guid.NewGuid();
        }

        private bool IsCachedInternal(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            try
            {
                if (_cachedDateTime.HasValue &&
                    _cachedDateTime.Value < DateTime.UtcNow.AddMilliseconds(CacheLifeTimeMs * -1))
                {
                    _cachedTexts.Clear();
                }

                if (_cachedTexts.ContainsKey(text))
                {
                    return true;
                }

            }
            finally
            {
                _cachedDateTime = DateTime.UtcNow;
            }

            return false;
        }

        private void AddCachedText(KeyValuePair<string, float> item)
        {
            if (_cachedTexts.TryGetValue(item.Key, out var score))
            {
                if (item.Value > score)
                {
                    _cachedTexts[item.Key] = item.Value;
                }
            }
            else
            {
                _cachedTexts[item.Key] = item.Value;
            }
        }

        private bool HasBetterSimilarText(string text, float score)
        {
            var hasSimilar = false;
            foreach (var cachedTextPair in _cachedTexts)
            {
                var textSimilarity = cachedTextPair.Key.GetSimilarity(text);
                if (textSimilarity > SIMILARITY_THRESHOLD)
                {
                    if (cachedTextPair.Value >= score)
                    {
                        return true;
                    }

                    hasSimilar = true;
                }
            }

            if (!hasSimilar)
            {
                _iterationId = Guid.NewGuid();
            }

            return false;
        }
    }
}
