using System.Collections.Generic;
using System.Linq;

namespace TeraTinder
{
    public class MainVM : BaseVM
    {
        private CardVM _current;
        private CardVM _next;
        private List<uint> _pastMatches;

        public Queue<CardVM> Matches { get; }
        public CardVM Current
        {
            get => _current;
            set
            {
                if (_current == value) return;
                _current = value;
                N();
            }
        }
        public CardVM Next
        {
            get => _next;
            set
            {
                if (_next == value) return;
                _next = value;
                N();
            }
        }

        public MainVM()
        {
            Matches = new Queue<CardVM>();
            _pastMatches = new List<uint>();
        }

        public void AddMatch(CardVM match)
        {
            if (Matches.Any(m => m.PlayerId == match.PlayerId)) return;
            if (_pastMatches.Contains(match.PlayerId)) return;
            Matches.Enqueue(match);
            SetCards();
        }

        private void SetCards()
        {
            if (Current == null)
            {
                Current = Matches.Count > 0 ? Matches.Dequeue() : null;
            }
            if (Current != null) Current.Rated += OnCardRated;
            if (Matches.Count <= 0 || Next != null) return;
            Next = Matches.Dequeue();
        }

        private void OnCardRated()
        {
            if (!Current.CardRated) return;
            if(!_pastMatches.Contains(Current.PlayerId)) _pastMatches.Add(Current.PlayerId);
            ShiftMatches();
        }

        public void ShiftMatches()
        {
            Current.Rated -= OnCardRated;
            Current = Next;
            Next = Matches.Count > 0 ? Matches.Dequeue() : null;
            SetCards();
        }

        public void ClearMatches()
        {
            Matches.Clear();
            Current = null;
            Next = null;
        }
    }
}