using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMedium_4._4
{
    class Program
    {
        private static List<User> _dataBase;
        private static Random _random = new Random();
        private static PreferMalesAnalytics _preferMaleAnalytics;
        private static PreferredAgeAnalytics _preferredAgeAnalytics;
        private static TopPreferredPagesAnalytics _topPreferredPagesAnalytics;

        static void Main(string[] args)
        {
            _dataBase = new List<User> {
                new User(0, true, 20),
                new User(1, true, 30),
                new User(2, true, 40),
                new User(3, false, 25),
                new User(4, false, 35)
            };

            _preferMaleAnalytics = new PreferMalesAnalytics();
            _preferredAgeAnalytics = new PreferredAgeAnalytics();
            _topPreferredPagesAnalytics = new TopPreferredPagesAnalytics();

            for(int i = 0; i < _dataBase.Count; i++)
            {
                int visits = _random.Next(1, 6);
                for(int j = 0; j < visits; j++)
                {
                    _dataBase[i].VisitPage(_random.Next(_dataBase.Count));
                }
                Console.Write("User" + i + ". Gender: " + (_dataBase[i].IsMale ? "male. " : "female. ") + "Age: " + _dataBase[i].Age + ". Visited pages: ");
                foreach(var pageId in _dataBase[i].VisitedPages)
                    Console.Write(pageId + ", ");
                Console.Write("\b\b  \n");
            }
            Console.WriteLine();

            int targetUserId = 0;

            _preferMaleAnalytics.Calcilate(_dataBase, _dataBase[targetUserId]);
            _preferredAgeAnalytics.Calcilate(_dataBase, _dataBase[targetUserId]);
            _topPreferredPagesAnalytics.Calcilate(_dataBase, _dataBase[targetUserId]);

            Console.WriteLine("User" + targetUserId + " PreferMaleAnalytics: " + _dataBase[targetUserId].PreferMaleAnalytics);
            Console.WriteLine("User" + targetUserId + " PreferredAgeAnalytics: " + _dataBase[targetUserId].PreferredAgeAnalytics);
            Console.Write("User" + targetUserId + " TopPreferredPages: ");
            for(int i = 0; i < _dataBase[targetUserId].TopPreferredPages.Count; i++)
            {
                if(_dataBase[targetUserId].TopPreferredPages.ElementAt(i) != null)
                    Console.Write(_dataBase[targetUserId].TopPreferredPages.ElementAt(i).ID + ", ");
            }
            Console.Write("\b\b  \n");
        }

        class User
        {
            public int ID { get; }
            public bool IsMale { get; }
            public int Age { get; }
            public bool PreferMaleAnalytics { get; private set; }
            public int PreferredAgeAnalytics { get; private set; }
            public IReadOnlyCollection<User> TopPreferredPages => _topPreferredPages;
            public IReadOnlyCollection<int> VisitedPages => _visitedPages;

            public readonly User[] _topPreferredPages;
            private readonly List<int> _visitedPages;

            public User(int id, bool isMale, int age)
            {
                ID = id;
                IsMale = isMale;
                Age = age;
                _topPreferredPages = new User[3];
                _visitedPages = new List<int>();
            }

            public void VisitPage(int userId)
            {
                _visitedPages.Add(userId);
            }

            public void SetPreferMaleAnalytics(bool preferMale)
            {
                PreferMaleAnalytics = preferMale;
            }

            public void SetPreferredAgeAnalytics(int preferredAge)
            {
                PreferredAgeAnalytics = preferredAge;
            }

            public void SetTopPreferredPages(User[] topPreferredPages)
            {
                for(int i = 0; i < topPreferredPages.Length; i++)
                {
                    _topPreferredPages[i] = topPreferredPages[i];
                }
            }
        }

        interface IAnalytics
        {
            void Calcilate(List<User> dataBase, User user);
        }

        class PreferMalesAnalytics : IAnalytics
        {
            public void Calcilate(List<User> dataBase, User user)
            {
                int malePages = 0;
                int femalePages = 0;
                foreach(var pageId in user.VisitedPages)
                {
                    if(dataBase[pageId].IsMale)
                        malePages++;
                    else
                        femalePages++;
                }
                user.SetPreferMaleAnalytics(malePages > femalePages);
            }
        }

        class PreferredAgeAnalytics : IAnalytics
        {
            public void Calcilate(List<User> dataBase, User user)
            {
                int totalAges = 0;
                foreach(var pageId in user.VisitedPages)
                {
                    totalAges += dataBase[pageId].Age;
                }
                user.SetPreferredAgeAnalytics(totalAges / user.VisitedPages.Count);
            }
        }

        class TopPreferredPagesAnalytics : IAnalytics
        {
            public void Calcilate(List<User> dataBase, User user)
            {
                Dictionary<int, int> topPages = new Dictionary<int, int>();
                foreach(var pageId in user.VisitedPages)
                {
                    if(!topPages.ContainsKey(pageId))
                        topPages.Add(pageId, 0);
                    topPages[pageId]++;
                }
                topPages = topPages.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                User[] targetPagesList = new User[user.TopPreferredPages.Count];
                for(int i = 0; i < user.TopPreferredPages.Count; i++)
                {
                    if(topPages.Count > i)
                        targetPagesList[i] = dataBase[topPages.ElementAt(i).Key];
                }

                user.SetTopPreferredPages(targetPagesList);
            }
        }
    }
}
