using System;
using NHibernate.Mapping.ByCode.Conformist;


using TestDdd.DomainModels;
using NHibernate.Mapping.ByCode;

namespace TestDdd.DomainModelMappings
{
	public class FavoriteHobbiesMapping : ClassMapping<FavoriteHobby>
	{
		public FavoriteHobbiesMapping ()
		{
            // Placing the Person reference on top, emphasizes that the FavoriteHobby cannot exist out of the Person's aggregate
            ManyToOne(x => x.Person, c => c.Column("person_id"));


			// http://stackoverflow.com/questions/15254051/nhibernate-mapping-by-code-conventions-to-postgres-sequence			
            Table ("favorite_hobby");

			Id (x => x.FavoriteHobbyId, c => {
				c.Column ("favorite_hobby_id");
				c.Generator(Generators.Sequence, m => m.Params(new { sequence = "favorite_hobby_favorite_hobby_id_seq"}));
			});


			Property (x => x.Hobby, c => c.Column ("hobby"));
            Property(x => x.IsActive, c => c.Column("is_active"));


		}
	}
}

