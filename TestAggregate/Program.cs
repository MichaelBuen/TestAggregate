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
            //TestAddFavoriteStuff();

            //TestUpdateFavoriteStuff();

            //TestDeleteFavoriteStuff();

            //TestMostFavorite();

            //TestFavoriteCountPragmatic();

            //TestListMostFavorite();

            TestEagerLoad();

            Console.ReadKey();

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


		static void TestAddFavoriteStuff()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

                var fs = new FavoriteStuff { Stuff = "xAdd : " +  DateTime.Now.ToString () };
				p.AddFavoriteStuff (fs);                

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}

		

		static void TestUpdateFavoriteStuff ()
		{


            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);

                //foreach (var z in p.FavoriteStuffs.OrderBy(x => x.FavoriteStuffId))
                //{
                //    Console.WriteLine("{0} {1}", z.FavoriteStuffId, z.Stuff);
                //}

                // p.FirstName = p.FirstName + "X";


                var fs = s.Load<FavoriteStuff>(9);

                fs.Stuff = "Yodelex";
 
   


                p.ApplyBusinessLogic(actionWhenValidated: tx.Commit);

                //foreach (var z in p.FavoriteStuffs.OrderBy(x => x.FavoriteStuffId))
                //{
                //    Console.WriteLine("{0} {1}", z.FavoriteStuffId, z.Stuff);
                //}


            }//using
    
		}




		static void TestDeleteFavoriteStuff ()
	    {
                using (var s = SessionMapper.Mapper.SessionFactory.OpenSession())
                using (var tx = s.BeginTransaction())
                {
                  
                    var p = s.Load<Person>(1);


                    //bool isInit = NHibernate.NHibernateUtil.IsInitialized(p.FavoriteStuffs);
                    

                    var fs = s.Load<FavoriteStuff>(32);


                    p.DeleteFavoriteStuff(fs);

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
                //    p.FavoriteStuffsDirectDB = s.Query<FavoriteStuff>(); //  Person is eagerly-loaded when we access any (even the non-mapped) property on Person
                //    // Not DDD. Every intelligence on model should be bolted to it
                //    var fs = p.TwoRecentFavorites.OrderBy(x => x.FavoriteStuffId).Take(1).Single();
                //    Console.WriteLine("{0} {1}", fs.FavoriteStuffId, fs.Stuff);
                //}

                
                //{
                //    p.FavoriteStuffsDirectDB = s.Query<FavoriteStuff>(); //  Person is eagerly-loaded when we access any (even the non-mapped) property on Person
                //    // DDD. But not performant, as the Person model is eagerly-loaded
                //    var fs = p.FirstOfTwoRecentFavorites;
                //    Console.WriteLine("{0} {1}", fs.FavoriteStuffId, fs.Stuff);    
                //}


                // DDD and performant.
                var fs = p.GetFirstOfTwoRecentFavorites(s.Query<FavoriteStuff>());


                Console.WriteLine("{0} {1}", fs.FavoriteStuffId, fs.Stuff);

            }//using
		}



		
		static void TestFavoriteCountPragmatic ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession ())
			using (var tx = s.BeginTransaction ()) 
            {

                //var count = s.Query<FavoriteStuff>().Count(x => x.Person == s.Load<Person>(1));
                //Console.WriteLine(count);

                var p = s.Load<Person>(1);

                // p.FavoriteStuffsDirectDB = s.Query<FavoriteStuff>(); 
                //var count = p.FavoriteStuffCount; // DDD, however Person is eagerly-loaded when we access any property on Person
                //Console.WriteLine("{0}", count);
                
                // DDD and performant
                var count = p.GetFavoriteStuffCountExceptFirstFromQueryable(s.Query<FavoriteStuff>());
                Console.WriteLine("{0}", count);
			}
		}

        static void TestListMostFavorite()
        {
            using (var s = SessionMapper.Mapper.SessionFactory.OpenSession ())
            using (var tx = s.BeginTransaction())
            {
                var p = s.Load<Person>(1);
                var fsList = p.GetTwoRecentFavorites(s.Query<FavoriteStuff>());

                foreach (var item in fsList)
                {
                    Console.WriteLine("{0} {1}", item.FavoriteStuffId, item.Stuff);
                }
            }
        }

	}//MainClass
}
