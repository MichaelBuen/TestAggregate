create table person
(
	person_id serial primary key,
	first_name text not null,
	last_name text not null
);


insert into person(first_name, last_name) values
('John','Lennon');



drop table favorite_stuff;
create table favorite_stuff
(
	person_id int not null references person(person_id),

	favorite_stuff_id serial primary key,
	stuff text not null
);


insert into favorite_stuff(person_id, stuff) values(1, 'aaa')


select * from person
select * from favorite_stuff

