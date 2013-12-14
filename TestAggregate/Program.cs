using System;
using System.Linq;

using NHibernate.Linq;
using TestDdd.DomainModels;

namespace TestDdd
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            //TestAddFavoriteHobbies();

            //TestUpdateFavoriteHobbies();

            //TestDeleteFavoriteHobbies();

            //TestMostFavorite();

            // TestFavoriteCountPragmatic();

            //TestListMostFavorite();

            //TestEagerLoad();

            // TestList();

            TestAddPerson();

            Console.ReadKey();

		}

        private static void TestAddPerson()
        {
            // This will get an error

            // We need repository here. Or maybe not, NHibernate already look like a repository 
            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = new Person();
                // p.FirstName = "blah"; will not compile
                p.SetFirstNameAndLastName("John", "Lennon");
                p.Age = -1; // will throw an exception
                s.Save(p);
                tx.Commit();
            }
        }

        static void TestList()
        {
            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {

                var aPersons = Person.AllStartsWithJ(s.Query<Person>());

                foreach (var item in aPersons)
                {
                    Console.WriteLine("{0}",item.FirstName);
                }

                tx.Commit();

                Console.ReadKey();
            }
        }

        private static void TestEagerLoad()
        {
            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);

                p.AccessingAnyMethodOrPropertyOfTheModelWillEagerLoadTheModel();

                p.ApplyBusinessLogic(tx.Commit);
            }
        }


		static void TestAddFavoriteHobbies()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);
                
                var fh = new FavoriteHobby { Hobby = "xAdd : " +  DateTime.Now.ToString () };
				p.AddFavoriteHobby (fh, s.Save);  

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}

		

		static void TestUpdateFavoriteHobbies ()
		{


            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);

                //foreach (var z in p.FavoriteHobbies.OrderBy(x => x.FavoriteHobbyId))
                //{
                //    Console.WriteLine("{0} {1}", z.FavoriteHobbyId, z.Hobby);
                //}

                // p.FirstName = p.FirstName + "X";


                var fh = s.Load<FavoriteHobby>(13);


                p.UpdateFavoriteHobby(fh, "YodelYo" + DateTime.Now.ToString());


                p.ApplyBusinessLogic(actionWhenValidated: tx.Commit);


                //foreach (var z in p.FavoriteHobbies.OrderBy(x => x.FavoriteHobbyId))
                //{
                //    Console.WriteLine("{0} {1}", z.FavoriteHobbyId, z.Hobby);
                //}



            }//using
    
		}




		static void TestDeleteFavoriteHobbies ()
	    {
                using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
                using (var tx = s.BeginTransaction())
                {
                  
                    var p = s.Load<Person>(1);

                    //bool isInit = NHibernate.NHibernateUtil.IsInitialized(p.FavoriteHobbiess);
                    
                    var fh = s.Load<FavoriteHobby>(23);

                    // If you know that a collection has no further collection, it's better to optimize the delete...
                    p.DeleteFavoriteHobby(fh, () => s.Delete(new FavoriteHobby { FavoriteHobbyId = fh.FavoriteHobbyId }));

                    //// ...otherwise, delete it via object, so delete can be cascaded:
                    //p.DeleteFavoriteHobby(fh, () => s.Delete(fh));

                    //// ...or if the FavoriteHobby list is already eagerly-loaded, delete it from the collection
                    //p.DeleteFavoriteHobby(fh);

                    // p.DeleteFavoriteHobby(fh); eager-loads both Person and FavoriteHobby

                    p.ApplyBusinessLogic(actionWhenValidated: () => tx.Commit());
                    
                }//using
  
		}



		static void TestMostFavorite ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);

                //{
                //    p.FavoriteHobbiessDirectDB = s.Query<FavoriteHobby>(); //  Person is eagerly-loaded when we access any (even the non-mapped) property on Person
                //    // Not DDD. Every intelligence on model should be bolted to it, no matter how trivial the operation in it, e.g. Take(1).Single()
                //    // Paging (Skip+Take combo) is exception to the rule, paging is not a domain model's concern, it can be addressed directly outside the domain model
                //    var fh = p.TwoRecentFavorites.OrderBy(x => x.FavoriteHobbyId).Take(1).Single();
                //    Console.WriteLine("{0} {1}", fh.FavoriteHobbyId, fh.Hobby);
                //}


                //{
                //    p.FavoriteHobbiessDirectDB = s.Query<FavoriteHobby>(); //  Person is eagerly-loaded when we access any (even the non-mapped) property on Person
                //    // DDD, yet not performant, as the Person model is eagerly-loaded
                //    var fh = p.FirstOfTwoRecentFavorites;
                //    Console.WriteLine("{0} {1}", fh.FavoriteHobbyId, fh.Hobby);
                //}


                {
                    // DDD and performant. Doesn't eagerly-load the Person
                    var fh = p.GetFirstOfTwoRecentFavoritesExtensionMethod(s.Query<FavoriteHobby>());
                    Console.WriteLine("{0} {1}", fh.FavoriteHobbyId, fh.Hobby);
                }

            }//using
		}



		
		static void TestFavoriteCountPragmatic ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession ())
			using (var tx = s.BeginTransaction ())
            {
                var p = s.Load<Person>(1);


                //{
                //    // Performant, yet not DDD.
                //    var count = s.Query<FavoriteHobby>().Count(x => x.Person == p);
                //    Console.WriteLine(count);
                //}



                //{
                //    // DDD, yet not performant. Person is eagerly-loaded when we access any property/method on Person. 
                //    p.FavoriteHobbiessDirectDB = s.Query<FavoriteHobby>();
                //    var count = p.FavoriteHobbiesCount;
                //    // Not future-proof, we cannot count selectively
                //    Console.WriteLine("Count: {0}", count);
                //}


                //{
                //     //Future-proof, we can count selectively on queryable.
                //     //DDD, yet not performant. Person is still eagerly-loaded when we access any property/method on Person.
                //    var count = p.GetFavoriteActiveHobbiesCountFromQueryable(s.Query<FavoriteHobby>());
                //    Console.WriteLine("Count: {0}", count);
                //}


                {
                    //DDD and performant. Person is not eagerly-loaded anymore, as we didn't touch any of the properties or methods of the Person model.
                    //The technique is to use extension method
                    var count = p.GetFavoriteActiveHobbiesCountFromQueryableExtensionMethod(s.Query<FavoriteHobby>());
                    Console.WriteLine("{0}", count); 
                }
            }
		}

        static void TestListMostFavorite()
        {
            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);
                var fsList = p.GetTwoRecentFavoritesExtensionMethod(s.Query<FavoriteHobby>());

                Console.WriteLine("Two Recent Favorites");
                foreach (var item in fsList)
                {
                    Console.WriteLine("{0} {1}", item.FavoriteHobbyId, item.Hobby);
                }
            }
        }

	}//MainClass
}
