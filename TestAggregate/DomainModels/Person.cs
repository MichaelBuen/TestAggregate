using System;

using System.Linq;

using System.Collections.Generic;
using TestDdd.DomainModels;


namespace TestDdd.DomainModels
{
	public class Person
	{
		public virtual int PersonId { get; set; }

		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }

		public virtual IList<FavoriteStuff> FavoriteStuffs { get; set; }




		// Stop the Anemic Domain Model! Let's practice Rich Domain Model!

		// When I mean purist here, the domain models only access the entities inside this aggregate(Person)


		// http://www.martinfowler.com/bliki/AnemicDomainModel.html

		// http://www.slideshare.net/chris.e.richardson/building-rich-domain-models




		public virtual void ApplyBusinessLogic(Action actionWhenValidated)
		{
			// Business Logic / Validation goes here

			actionWhenValidated ();
		}


		// ADD. Purist, efficient
		public virtual void AddFavoriteStuff(FavoriteStuff fs)
		{
			// Business Logic / Validation goes here

			fs.Person = this;
			this.FavoriteStuffs.Add (fs);
		}


		// UPDATE. Purist, inefficient
		public virtual void UpdateFavoriteStuffInefficient(int favoriteStuffId, string stuff)
		{
			var fs = this.FavoriteStuffs.Where (x => x.FavoriteStuffId == favoriteStuffId).SingleOrDefault ();

			if (fs == null)
				throw new Exception ("Already deleted");

			// Business Logic / Validation goes here

			fs.Stuff = stuff;
		}


		// UPDATE. Pragmatic, efficient
		public virtual void UpdateFavoriteStuffEfficient(FavoriteStuff fs, string stuff)
		{
			if (fs == null)
				throw new Exception ("Already deleted");

			// Business Logic / Validation goes here

			fs.Stuff = stuff;
		}


		// DELETE. Purist, inefficient
		public virtual void DeleteFavoriteStuffInefficient(int favoriteStuffId)
		{
			var fs = this.FavoriteStuffs.SingleOrDefault (x => x.FavoriteStuffId == favoriteStuffId);

			if (fs == null)
				throw new Exception ("Already deleted");	

			// Business Logic / Validation goes here	

			this.FavoriteStuffs.Remove (fs);
		}


		// DELETE. Pragmatic, efficient
		public virtual void DeleteFavoriteStuffEfficient(FavoriteStuff fs, Action<FavoriteStuff> actionWhenAllowed)
		{
			if (fs == null)
				throw new Exception ("Already deleted");		

			// Business Logic / Validation goes here

			actionWhenAllowed (fs);
		}


		// Get First Favorite. Purist, inefficient
		public virtual FavoriteStuff MostFavoriteInefficient
		{
			get { return this.FavoriteStuffs.OrderBy (x => x.FavoriteStuffId).Take (1).Single ();  }
		}


		// Get First Favorite. Pragmatic, efficient
		public virtual FavoriteStuff GetMostFavoriteEfficient(IQueryable<FavoriteStuff> fsQuery)
		{
			return fsQuery.Where(x => x.Person == this).OrderBy (x => x.FavoriteStuffId).Take (1).Single (); 
		}



		// Purist. Efficient
		public virtual int FavoriteStuffCount 
		{
			get {
				// Thanks Extra Lazy! Check this on PersonMapping
				// rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);

				return this.FavoriteStuffs.Count ();
			}
		}


		// If there's no Extra Lazy it would be more pragmatic to pass the IQueryable<FavoriteStuff> to Person
		// Pragmatic if there's no Extra Lazy. Efficient
		public virtual int GetFavoriteStuffCount(IQueryable<FavoriteStuff> favoriteStuff) 
		{
			return favoriteStuff.Count (x => x.Person == this);
		}



	}
}

