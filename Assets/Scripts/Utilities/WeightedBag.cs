using System;
using System.Collections.Generic;

namespace Utilities
{
    public class WeightedBag<T>
    {
        public readonly Dictionary<T, int> items = new Dictionary<T, int>();
        private int maxBias;
        
        public WeightedBag() {}

        private WeightedBag(Dictionary<T, int> items)
        {
            this.items = items;
        }

        public WeightedBag<T> AddItem(T item, int bias = 1)
        {
            if (bias < 1)
                throw new ArgumentException("Item bias cannot be lesser than 1!");

            if (items.ContainsKey(item))
                maxBias -= items[item];
            
            items[item] = bias;
            maxBias += bias;
            return this;
        }

        public WeightedBag<T> RemoveItem(T item)
        {
            if (items.ContainsKey(item))
            {
                maxBias -= items[item];
                items.Remove(item);
            }

            return this;
        }

        public WeightedBag<T> AddRange(IEnumerable<T> itemsToAdd, Func<T, int> biasFunc)
        {
            foreach (T item in itemsToAdd)
            {
                int bias = biasFunc(item);
                AddItem(item, bias);
            }
            
            return this;
        }

        public WeightedBag<T> Copy() =>
            new WeightedBag<T>(new Dictionary<T, int>(items))
            {
                maxBias = maxBias
            };

        public List<T> PullSortedOrder()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException(
                    "Trying to pull out of a weighted bag, but the bag is empty!");
            }

            List<T> sortedItems = new List<T>();
            WeightedBag<T> bag = Copy();

            while (bag.items.Count > 0)
            {
                T item = bag.PullItem();
                bag.RemoveItem(item);
                sortedItems.Add(item);
            }
            
            return sortedItems;
        }

        public T PullItem()
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException(
                    "Trying to pull out of a weighted bag, but the bag is empty!");
            }

            int randomScore = UnityEngine.Random.Range(0, maxBias);
            T finalItem = default;
            bool foundItem = false;
            int currentScore = 0;

            foreach (var (item, bias) in items)
            {
                int itemEndScore = currentScore + bias;
                
                if (randomScore >= currentScore && randomScore < itemEndScore)
                {
                    finalItem = item;
                    foundItem = true;
                    break;
                }

                currentScore += bias;
            }

            if (!foundItem)
                throw new Exception("Failed to get an item out of the weighted bag!");

            return finalItem;
        }
    }
}
