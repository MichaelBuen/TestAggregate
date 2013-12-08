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

		public virtual IList<FavoriteHobby> FavoriteHobbies { get; set; }

        

		// Stop the Anemic Domain Model! Let's practice Rich Domain Model!


		// http://www.martinfowler.com/bliki/AnemicDomainModel.html

		// http://www.slideshare.net/chris.e.richardson/building-rich-domain-models


        public virtual IQueryable<FavoriteHobby> FavoriteHobbiessDirectDB { get; set; }


        public virtual void AccessingAnyMethodOrPropertyOfTheModelWillEagerLoadTheModel()
        {

        }


		public virtual void ApplyBusinessLogic(Action actionWhenValidated)
		{
			// Business Logic / Validation goes here



			actionWhenValidated ();
		}

		
		public virtual void AddFavoriteHobbies(FavoriteHobby fh)
		{
			// Business Logic / Validation goes here

			fh.Person = this;
			this.FavoriteHobbies.Add (fh);
		}


		
		public virtual void UpdateFavoriteHobbies(FavoriteHobby fh, string Hobbies)
		{
			// Business Logic / Validation goes here

			fh.Hobby = Hobbies;
		}




		public virtual void DeleteFavoriteHobby(FavoriteHobby fh, Action<object> immediateDeleter = null)
		{
			// Business Logic / Validation goes here

            if (immediateDeleter != null)
                immediateDeleter(fh);
            else
                this.FavoriteHobbies.Remove(fh);    

		}

        

		// Get First Favorite. DDD Purist, everything the model need to know are in its aggregate. Inefficient
		public virtual FavoriteHobby MostFavoriteInefficient
		{
			get { return this.FavoriteHobbies.OrderBy (x => x.FavoriteHobbyId).Take (1).Single ();  }
		}


		// Get First Favorite. Pragmatic, efficient
		public virtual IQueryable<FavoriteHobby> TwoRecentFavorites
		{
            get
            {
                return this.FavoriteHobbiessDirectDB.Where(x => x.Person == this).OrderByDescending(x => x.FavoriteHobbyId).Take(2);
            }
		}


        // DDD espouses code re-use and centralized code
        public virtual FavoriteHobby FirstOfTwoRecentFavorites 
        {
            get
            {
                return this.TwoRecentFavorites.OrderBy(x => x.FavoriteHobbyId).Take(1).Single();
            }
        }



        // Efficient when coupled with Extra and Lazy. Count will be performed on the DB side
        // Good for pure DDD. Onet-stop-shop.
        // However, inefficient when need to add conditions on Count, NHibernate will eagerly-load the list when we add a condition on its collection even we use Extra+Lazy
        public virtual int FavoriteHobbiesCount
        {
            get
            {
                // Thanks Extra+Lazy! Check this on PersonMapping
                // rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);

                return this.FavoriteHobbies.Count();
            }
        }

        
		// If there's no Extra+Lazy it would be more pragmatic to pass the IQueryable<FavoriteHobbies> to Person and use IQueryable. 		
		public virtual int FavoriteHobbiesCountFromQueryable
		{
            get
            {
                return this.FavoriteHobbiessDirectDB.Count(x => x.Person == this);
            }
		}



        // But really it's better and pragmatic to use IQueryable than using Extra+Lazy.
        // We can't be a DDD purist for far too long when we need our code to be performant.
        // Example, it is more efficient to add a condition to IQueryable than to add a condition on IList; 
        // all IList elements shall be eagerly-loaded when adding a condition on it.

        // This eagerly-loads all the FavoriteHobby of FavoriteHobbies:
        //      return this.FavoriteHobbies.Count(x => x.IsActive);

        // TL;DR: So use IQueryable.
        public virtual int FavoriteActiveHobbiesCountFromQueryable
        {
            get
            {
                // Except first
                return this.FavoriteHobbiessDirectDB.Count(x => x.Person == this && x.IsActive);
            }
        }

        public virtual int GetFavoriteActiveHobbiesCountFromQueryable(IQueryable<FavoriteHobby> fh)
        {            
            return fh.Count(x => x.Person == this && x.IsActive);            
        }

    }

    // HOWEVER!!!  
    // It's better to use an extension method than have the Person model contain the IQueryables.
    // A mere accessing of property inside a Person will eager-load the Person
    // Static method won't eager-load the Person
    public static class PersonBusinessIntelligence
    {        
        public static int GetFavoriteHobbiesCountFromQueryableExtensionMethod(this Person p, IQueryable<FavoriteHobby> fh)
        {
            return fh.Count(x => x.Person == p);
        }


        public static int GetFavoriteActiveHobbiesCountFromQueryableExtensionMethod(this Person p, IQueryable<FavoriteHobby> fh)
        {
            Console.WriteLine("Extension method version");
            return fh.Count(x => x.Person == p && x.IsActive);
        }

        
        public static IQueryable<FavoriteHobby> GetTwoRecentFavoritesExtensionMethod(this Person p, IQueryable<FavoriteHobby> fh)
        {
            return fh.Where(x => x.Person == p).OrderByDescending(x => x.FavoriteHobbyId).Take(2);            
        }

        
        public static FavoriteHobby GetFirstOfTwoRecentFavoritesExtensionMethod(this Person p, IQueryable<FavoriteHobby> fh)
        {
            return p.GetTwoRecentFavoritesExtensionMethod(fh).OrderBy(x => x.FavoriteHobbyId).Take(1).Single();            
        }

        

     
    }
}

