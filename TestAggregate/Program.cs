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
			TestAddFavoriteStuff ();

			TestUpdateFavoriteStuffInefficient ();
			TestUpdateFavoriteStuffEfficient ();

			TestDeleteFavoriteStuffInefficient ();
			TestDeleteFavoriteStuffEfficient ();

			TestMostFavoriteInefficient ();
			TestMostFavoriteEfficient ();


			TestFavoriteCountPuristEfficient ();
			TestFavoriteCountPragmaticEfficient ();

		}//Main

		static void TestAddFavoriteStuff()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				p.AddFavoriteStuff (new FavoriteStuff { Stuff = DateTime.Now.ToString () });

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}

		static void TestUpdateFavoriteStuffInefficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				p.UpdateFavoriteStuffInefficient (57, stuff: DateTime.Now.ToString ());

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}

		static void TestUpdateFavoriteStuffEfficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				var fs = s.Load<FavoriteStuff> (57);
				p.UpdateFavoriteStuffEfficient (fs, stuff: DateTime.Now.ToString ());

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}



		static void TestDeleteFavoriteStuffInefficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				p.DeleteFavoriteStuffInefficient (68);

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}

		static void TestDeleteFavoriteStuffEfficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				var fs = s.Load<FavoriteStuff> (70);
				p.DeleteFavoriteStuffEfficient (fs, s.Delete);

				p.ApplyBusinessLogic (actionWhenValidated: tx.Commit);
			}//using
		}


		static void TestMostFavoriteInefficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				var fs = p.MostFavoriteInefficient;

			}//using
		}

		static void TestMostFavoriteEfficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession()) 
			using(var tx = s.BeginTransaction())
			{
				var p = s.Load<Person> (1);

				var fs = p.GetMostFavoriteEfficient (s.Query<FavoriteStuff> ());

			}//using
		}



		static void TestFavoriteCountPuristEfficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession ())
			using (var tx = s.BeginTransaction ()) {
				var p = s.Load<Person> (1);
				var c = p.FavoriteStuffCount;
				Console.WriteLine ("{0}", c);
			}
		}

		static void TestFavoriteCountPragmaticEfficient ()
		{
			using (var s = SessionMapper.Mapper.SessionFactory.OpenSession ())
			using (var tx = s.BeginTransaction ()) {
				var p = s.Load<Person> (1);
				var c = p.GetFavoriteStuffCount (s.Query<FavoriteStuff> ());
				Console.WriteLine ("{0}", c);
			}
		}
	}//MainClass
}
