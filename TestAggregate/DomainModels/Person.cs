using System;

using System.Linq;

using System.Collections.Generic;
using TestDdd.DomainModels;


namespace TestDdd.DomainModels
{
	public class Person
    {

#pragma warning disable 0649
        int _personId;
#pragma warning restore
        public virtual int PersonId { get { return _personId; } }
        

        public virtual string FirstName { get; protected  internal set; }
        public virtual string LastName { get; protected internal set; }


        public virtual string MiddleName { get; set; } // needs no validation

        int _age;
        public virtual int Age 
        {
            get { return _age; }
            set
            {
                // Domain business rules and validation goes here

                if (_age <= 0)
                    throw new Exception("Can't set age lower than 0");

                _age = value;

            }
        }



		public virtual IList<FavoriteHobby> FavoriteHobbies { get; set; }

        

		// Stop the Anemic Domain Model! Let's practice Rich Domain Model!

        // It's better to put the Rich Domain Model's behavior on model's extension methods than on the model itself
        // NHibernate eagerly-loads the model if you access its properties/methods, regardless of them being mapped/unmapped


		// http://www.martinfowler.com/bliki/AnemicDomainModel.html

		// http://www.slideshare.net/chris.e.richardson/building-rich-domain-models


        public virtual IQueryable<FavoriteHobby> FavoriteHobbiessDirectDB { get; set; }


        public virtual void AccessingAnyMethodOrPropertyOfTheModelWillEagerLoadTheModel()
        {

        }

        public static IQueryable<Person> AllStartsWithJ(IQueryable<Person> persons)
        {
            return persons.Where(x => x.FirstName.StartsWith("J"));
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
                // Thanks Extra+Lazy! This is on PersonMapping:
                //    rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);

                // With Extra+Lazy, counting will be performed at the database-side instead of counting the in-memory objects
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

        // This eagerly-loads all the FavoriteHobby of FavoriteHobbies even we use Extra+Lazy
        //      return this.FavoriteHobbies.Count(x => x.IsActive);

        // TL;DR: So use IQueryable.
        public virtual int FavoriteActiveHobbiesCountFromQueryable
        {
            get
            {
                // Only the actives
                return this.FavoriteHobbiessDirectDB.Count(x => x.Person == this && x.IsActive); 
            }
        }

        public virtual int GetFavoriteActiveHobbiesCountFromQueryable(IQueryable<FavoriteHobby> fh)
        {            
            return fh.Count(x => x.Person == this && x.IsActive);            
        }

        public virtual void UpdateFavoriteHobbySlow(FavoriteHobby fh, string Hobbies)
        {
            // Business Logic / Validation goes here

            fh.Person = this;

            fh.Hobby = Hobbies;

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
            // If we don't access any of the properties of the Person p, the Person instance p will not be fetched from the database
            // Console.WriteLine("Name: {0}", p.FirstName);

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

        public static void ApplyBusinessLogic(this Person p, Action actionWhenValidated)
        {
            // Business Logic / Validation goes here

            actionWhenValidated();
        }



        // returns FavoriteHobby id
        public static int AddFavoriteHobby(this Person p, FavoriteHobby fh, Func<FavoriteHobby, object> immediateAddAction)
        {
            // Business Logic / Validation goes here

            fh.Person = p;

            int id = 0;

            if (immediateAddAction != null)
                id = (int)immediateAddAction(fh);
            else
            {
                p.FavoriteHobbies.Add(fh);
            }

            return id;
            
        }

        public static void UpdateFavoriteHobby(this Person p, FavoriteHobby fh, string Hobbies)
        {
            // Business Logic / Validation goes here

            fh.Person = p;

            fh.Hobby = Hobbies;

        }
        

        public static void DeleteFavoriteHobby(this Person p, FavoriteHobby fh, Action immediateDeleter = null)
        {
            // Business Logic / Validation goes here

            if (immediateDeleter != null)
                immediateDeleter();
            else
                p.FavoriteHobbies.Remove(fh);

        }



        public static void SetFirstNameAndLastName(
            this Person person, 
            string firstName, 
            string lastName
            // IQueryable<BannedPhrase> bannedPhrases
            )
        {
            // Domain business rules and validation goes here

            // if firstname in censored words throw exception
            // if lastname in censored words throw exception

            // validate firstname + lastname from bannedPhrases
            // firstname might not be in censored words, and the lastname too; but putting them together might be in censored phrases
            // e.g. Firstname is Jack, Lastname is Off
            // if it is, throw an exception

           


            // If the values conforms to business rules and validation, set the backing fields            
            person.FirstName = firstName;
            person.LastName = lastName;
            ;
        }
     

    }
}

