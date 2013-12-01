using System;
using NHibernate.Mapping.ByCode.Conformist;


using TestDdd.DomainModels;
using NHibernate.Mapping.ByCode;

namespace TestDdd.DomainModelMappings
{
	public class FavoriteStuffMapping : ClassMapping<FavoriteStuff>
	{
		public FavoriteStuffMapping ()
		{
			// http://stackoverflow.com/questions/15254051/nhibernate-mapping-by-code-conventions-to-postgres-sequence
			Table ("favorite_stuff");
			Id (x => x.FavoriteStuffId, c => {
				c.Column ("favorite_stuff_id");
				c.Generator(Generators.Sequence, m => m.Params(new { sequence = "favorite_stuff_favorite_stuff_id_seq"}));
			});
			ManyToOne (x => x.Person, c => {
				c.Column ("person_id");
				//c.Cascade(Cascade.ReAttach);


			});
			Property (x => x.Stuff, c => c.Column ("stuff"));


		}
	}
}

