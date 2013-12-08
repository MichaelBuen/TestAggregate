using System;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;


using TestDdd.DomainModelMappings;

namespace TestDdd.SessionMapper
{
	public static class Mapper
	{
		static NHibernate.ISessionFactory _sessionFactory = Mapper.GetSessionFactory();


		public static NHibernate.ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

		static NHibernate.ISessionFactory GetSessionFactory()
		{
			var mapper = new NHibernate.Mapping.ByCode.ModelMapper ();

			mapper.AddMappings (
				new[] { 
					typeof(PersonMapping) ,
					typeof(FavoriteHobbiesMapping)
				});


			var cfg = new NHibernate.Cfg.Configuration ();

			cfg.DataBaseIntegration (c => {
				c.Driver<NHibernate.Driver.NpgsqlDriver>();
				c.Dialect<NHibernate.Dialect.PostgreSQLDialect>();
				c.ConnectionString = "Server=localhost; Database=test_aggregate; User=postgres; password=opensesame93; Port=5432";

				c.LogFormattedSql = true;
				c.LogSqlInConsole = true;
			});



			HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			cfg.AddMapping (domainMapping);



			_sessionFactory = cfg.BuildSessionFactory ();

			return _sessionFactory;
		}
	}
}

