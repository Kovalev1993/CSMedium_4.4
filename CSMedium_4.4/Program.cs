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
                new User(0, true, 20, new UserStatistic()),
                new User(1, true, 30, new UserStatistic()),
                new User(2, true, 40, new UserStatistic()),
                new User(3, false, 25, new UserStatistic()),
                new User(4, false, 35, new UserStatistic())
            };

            _preferMaleAnalytics = new PreferMalesAnalytics();
            _preferredAgeAnalytics = new PreferredAgeAnalytics();
            _topPreferredPagesAnalytics = new TopPreferredPagesAnalytics();

            SimulateUsersBehaviour();

            int targetUserId = 0;

            _preferMaleAnalytics.Calculate(_dataBase, _dataBase[targetUserId]);
            _preferredAgeAnalytics.Calculate(_dataBase, _dataBase[targetUserId]);
            _topPreferredPagesAnalytics.Calculate(_dataBase, _dataBase[targetUserId]);

            Console.WriteLine("User" + targetUserId + " PreferMaleAnalytics: " + _dataBase[targetUserId].Statistic.PreferMaleAnalytics);
            Console.WriteLine("User" + targetUserId + " PreferredAgeAnalytics: " + _dataBase[targetUserId].Statistic.PreferredAgeAnalytics);
            Console.Write("User" + targetUserId + " TopPreferredPages: ");
            for(int i = 0; i < _dataBase[targetUserId].Statistic.TopPreferredPages.Length; i++)
            {
                if(_dataBase[targetUserId].Statistic.TopPreferredPages.ElementAt(i) != null)
                    Console.Write(_dataBase[targetUserId].Statistic.TopPreferredPages.ElementAt(i).ID + ", ");
            }
            Console.Write("\b\b  \n");
        }

        private static void SimulateUsersBehaviour()
        {
            for(int i = 0; i < _dataBase.Count; i++)
            {
                int visits = _random.Next(1, 6);
                for(int j = 0; j < visits; j++)
                {
                    _dataBase[i].Statistic.AddToVisitedPages(_random.Next(_dataBase.Count));
                }
                Console.Write("User" + i + ". Gender: " + (_dataBase[i].IsMale ? "male. " : "female. ") + "Age: " + _dataBase[i].Age + ". Visited pages: ");
                foreach(var pageId in _dataBase[i].Statistic.VisitedPages)
                    Console.Write(pageId + ", ");
                Console.Write("\b\b  \n");
            }
            Console.WriteLine();
        }

        class UserStatistic
        {
            public bool PreferMaleAnalytics { get; private set; }
            public int PreferredAgeAnalytics { get; private set; }
            public User[] TopPreferredPages { get; private set; }
            public List<int> VisitedPages { get; private set; }

            public UserStatistic()
            {
                TopPreferredPages = new User[3];
                VisitedPages = new List<int>();
            }

            public void AddToVisitedPages(int userId)
            {
                VisitedPages.Add(userId);
            }

            public void SetPreferMale(bool preferMale)
            {
                PreferMaleAnalytics = preferMale;
            }

            public void SetPreferredAge(int age)
            {
                PreferredAgeAnalytics = age;
            }

            public void SetTopPreferredPages(User[] topPreferredPages)
            {
                for(int i = 0; i < topPreferredPages.Length; i++)
                {
                    TopPreferredPages[i] = topPreferredPages[i];
                }
            }
        }

        class User
        {
            public int ID { get; }
            public bool IsMale { get; }
            public int Age { get; }
            public UserStatistic Statistic { get; }

            public User(int id, bool isMale, int age, UserStatistic statistic)
            {
                ID = id;
                IsMale = isMale;
                Age = age;
                Statistic = statistic;
            }
        }

        interface IAnalytics
        {
            void Calculate(List<User> dataBase, User user);
        }

        class PreferMalesAnalytics : IAnalytics
        {
            public void Calculate(List<User> dataBase, User user)
            {
                int malePages = 0;
                int femalePages = 0;
                foreach(var pageId in user.Statistic.VisitedPages)
                {
                    if(dataBase[pageId].IsMale)
                        malePages++;
                    else
                        femalePages++;
                }
                user.Statistic.SetPreferMale(malePages > femalePages);
            }
        }

        class PreferredAgeAnalytics : IAnalytics
        {
            public void Calculate(List<User> dataBase, User user)
            {
                int totalAges = 0;
                foreach(var pageId in user.Statistic.VisitedPages)
                {
                    totalAges += dataBase[pageId].Age;
                }
                user.Statistic.SetPreferredAge(totalAges / user.Statistic.VisitedPages.Count);
            }
        }

        class TopPreferredPagesAnalytics : IAnalytics
        {
            public void Calculate(List<User> dataBase, User user)
            {
                Dictionary<int, int> topPages = new Dictionary<int, int>();
                foreach(var pageId in user.Statistic.VisitedPages)
                {
                    if(!topPages.ContainsKey(pageId))
                        topPages.Add(pageId, 0);
                    topPages[pageId]++;
                }
                topPages = topPages.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                User[] targetPagesList = new User[user.Statistic.TopPreferredPages.Length];
                for(int i = 0; i < user.Statistic.TopPreferredPages.Length; i++)
                {
                    if(topPages.Count > i)
                        targetPagesList[i] = dataBase[topPages.ElementAt(i).Key];
                }

                user.Statistic.SetTopPreferredPages(targetPagesList);
            }
        }
    }
}
