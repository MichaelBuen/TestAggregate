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
            
            Id(x => x.PersonId, c => 
            {
                c.Column("person_id");
                c.Access(NHibernate.Mapping.ByCode.Accessor.Field);
                c.Generator(NHibernate.Mapping.ByCode.Generators.Sequence, m => m.Params(new { sequence = "person_person_id_seq" }));
            });

            Property(x => x.FirstName, c => 
            {
                c.Column("first_name");
                // c.Access(NHibernate.Mapping.ByCode.Accessor.Field);
            });

            Property(x => x.LastName, c =>
            {
                c.Column("last_name");
                // c.Access(NHibernate.Mapping.ByCode.Accessor.Field);
            });

            Property(x => x.Age, c =>
            {
                c.Column("age");
                c.Access(NHibernate.Mapping.ByCode.Accessor.Field);
            });
            

            Bag(list => list.FavoriteHobbies,
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

