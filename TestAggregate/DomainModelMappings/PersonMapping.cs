using System;
using NHibernate.Mapping.ByCode.Conformist;

using TestDdd.DomainModels;

namespace TestDdd.DomainModelMappings
{
	public class PersonMapping : ClassMapping<Person>
	{
		public PersonMapping ()
		{
			Table ("person");
			Id (x => x.PersonId, c => c.Column("person_id"));
			Property (x => x.FirstName, c => c.Column ("first_name"));
			Property (x => x.LastName, c => c.Column("last_name"));

            Bag<FavoriteStuff>(list => list.FavoriteStuffs,
                rel =>
                {

                    rel.Key(k =>
                    {
                        k.Column("person_id");
                        k.NotNullable(true);
                        
                    }); // id of child table

                    rel.Inverse(true); // makes the parent handle the childlist


                    rel.Cascade(NHibernate.Mapping.ByCode.Cascade.Persist | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans);


                    rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);

                    // rel.Fetch(NHibernate.Mapping.ByCode.CollectionFetchMode.Select);

                    

                },
                relType =>
                {
                    relType.OneToMany();
                    
                });

		}
	}
}

