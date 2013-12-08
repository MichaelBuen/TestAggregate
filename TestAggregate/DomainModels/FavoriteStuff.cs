using System;
using TestDdd.DomainModels;

namespace TestDdd.DomainModels
{
	public class FavoriteHobby
	{
        // Placing the Person reference on top, emphasizes that the FavoriteHobby cannot exist out of the Person's aggregate
        public virtual Person Person { get; set; } 

		public virtual int FavoriteHobbyId { get; set; }		
		public virtual string Hobby { get; set; }

        public virtual bool IsActive { get; set; }

	}
}

