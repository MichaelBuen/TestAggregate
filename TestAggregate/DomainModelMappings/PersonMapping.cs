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

			Bag<FavoriteStuff> (list => list.FavoriteStuffs, 
				rel => {

					rel.Key (k => {
						k.Column ("person_id");
						k.NotNullable(true);
					}); // id of child table
					
					rel.Inverse(true); // makes the parent handle the childlist

					// rel.Cascade(cascadeStyle: NHibernate.Mapping.ByCode.Cascade.Persist);

					rel.Cascade(NHibernate.Mapping.ByCode.Cascade.All | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans);

//					rel.Cascade(cascadeStyle: NHibernate.Mapping.ByCode.Cascade.All);
					rel.Lazy(NHibernate.Mapping.ByCode.CollectionLazy.Extra | NHibernate.Mapping.ByCode.CollectionLazy.Lazy);
				}, 
				relType => relType.OneToMany ());

		}
	}
}

