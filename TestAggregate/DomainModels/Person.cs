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


		// http://www.martinfowler.com/bliki/AnemicDomainModel.html

		// http://www.slideshare.net/chris.e.richardson/building-rich-domain-models


        public virtual IQueryable<FavoriteStuff> FavoriteStuffsDirectDB { get; set; }


		public virtual void ApplyBusinessLogic(Action actionWhenValidated)
		{
			// Business Logic / Validation goes here



			actionWhenValidated ();
		}

		
		public virtual void AddFavoriteStuff(FavoriteStuff fs)
		{
			// Business Logic / Validation goes here

			fs.Person = this;
			this.FavoriteStuffs.Add (fs);
		}


		
		public virtual void UpdateFavoriteStuff(FavoriteStuff fs, string stuff)
		{
			// Business Logic / Validation goes here

			fs.Stuff = stuff;
		}




		public virtual void DeleteFavoriteStuff(FavoriteStuff fs, Action<object> immediateDeleter = null)
		{
			// Business Logic / Validation goes here

            if (immediateDeleter != null)
                immediateDeleter(fs);
            else
                this.FavoriteStuffs.Remove(fs);    

		}

        

		// Get First Favorite. DDD Purist, everything the model need to know are in its aggregate. Inefficient
		public virtual FavoriteStuff MostFavoriteInefficient
		{
			get { return this.FavoriteStuffs.OrderBy (x => x.FavoriteStuffId).Take (1).Single ();  }
		}


		// Get First Favorite. Pragmatic, efficient
		public virtual IQueryable<FavoriteStuff> TwoRecentFavorites
		{
            get
            {
                return this.FavoriteStuffsDirectDB.Where(x => x.Person == this).OrderByDescending(x => x.FavoriteStuffId).Take(2);
            }
		}


        // DDD espouses code re-use and centralized code
        public virtual FavoriteStuff FirstOfTwoRecentFavorites 
        {
            get
            {
                return this.TwoRecentFavorites.OrderBy(x => x.FavoriteStuffId).Take(1).Single();
            }
        }



        // Efficient when coupled with Extra and Lazy
        // Good for pure DDD. Onet-stop-shop
        // But inefficient when need to add conditions on Count        
        public virtual int FavoriteStuffCount
        {
            get
            {
                // Thanks Extra Lazy! Check this on PersonMapping
                // rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);

                return this.FavoriteStuffs.Count();
            }
        }

        
		// If there's no Extra+Lazy it would be more pragmatic to pass the IQueryable<FavoriteStuff> to Person and use that IQueryable		
		public virtual int FavoriteStuffCountFromQueryable
		{
            get
            {
                return this.FavoriteStuffsDirectDB.Where(x => x.Person == this).Count();
            }
		}



        // But really it's better to use IQueryable than using Extra+Lazy.
        // We can't be a DDD purist for far too long when need our code to be performant.
        // Example, it is more efficient to add a condition to IQueryable than to a condition on IList; 
        // all IList elements shall be eagerly-loaded when adding a condition on it.
        // So use IQueryable.
        public virtual int FavoriteStuffCountExceptFirstFromQueryable
        {
            get
            {
                // Except first
                return this.FavoriteStuffsDirectDB.Where(x => x.FavoriteStuffId > 1 && x.Person == this).Count();
            }
        }

    }

    // HOWEVER!!!  
    // It's better to use an extension method than have the Person model contain the IQueryables.
    // A mere accessing of property inside a Person will eager-load the Person
    // Static method won't eager-load the Person
    public static class PersonBusinessLogic
    {        
        public static int GetFavoriteStuffCountExceptFirstFromQueryable(this Person p, IQueryable<FavoriteStuff> fs)
        {
            return fs.Where(x => x.Person == p).Count();
        }

        
        public static IQueryable<FavoriteStuff> GetTwoRecentFavorites(this Person p, IQueryable<FavoriteStuff> fs)
        {
            return fs.Where(x => x.Person == p).OrderByDescending(x => x.FavoriteStuffId).Take(2);            
        }

        
        public static FavoriteStuff GetFirstOfTwoRecentFavorites(this Person p, IQueryable<FavoriteStuff> fs)
        {
            return p.GetTwoRecentFavorites(fs).OrderBy(x => x.FavoriteStuffId).Take(1).Single();            
        }

     
    }
}

