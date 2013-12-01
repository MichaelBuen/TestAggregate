using System;
using TestDdd.DomainModels;

namespace TestDdd.DomainModels
{
	public class FavoriteStuff
	{

		public virtual int FavoriteStuffId { get; set; }
		public virtual Person Person { get; set; }
		public virtual string Stuff { get; set; }

	}
}

