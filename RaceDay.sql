create database RaceDay;

use RaceDay;


create table roles(
    role_id int identity(1,1) primary key,
    role_name nvarchar(20) not null unique);

create table users(
    user_id int identity(1,1) primary key,
    role_id int not null references roles(role_id),
    first_name nvarchar(50) not null,
    last_name nvarchar(50) not null,
    username nvarchar(50) not null unique,
    email nvarchar(100) not null unique,
    phone_number nvarchar(20) null,
    password_hash nvarchar(100) not null);

create table events(
    event_id int identity(1,1) primary key,
    organiser_id int not null references users(user_id),
    event_name nvarchar(150) not null,
    event_date datetime2 not null,
    location nvarchar(200) not null,
    description nvarchar(1000) not null,
    route_info nvarchar(1000) null,
    distance float not null,
    event_type nvarchar(10) not null check (event_type in ('Run','Walk','Cycle')),
    banner_image_url nvarchar(500) null);

create table categories(
    category_id int identity(1,1) primary key,
    category_name nvarchar(100) not null,
    description nvarchar(300) null);

create table event_categories(
    event_category_id int identity(1,1) primary key,
    event_id int not null references events(event_id),
    category_id int not null references categories(category_id),
    entry_fee decimal(10,2) not null,
    max_participants int not null,
    unique(event_id, category_id));

create table enrolments(
    enrolment_id int identity(1,1) primary key,
    user_id int not null references users(user_id),
    event_id int not null references events(event_id),
    event_category_id int not null references event_categories(event_category_id),
    enrolment_date datetime2 not null default sysutcdatetime(),
    status nvarchar(20) not null default 'Confirmed',
    unique(user_id, event_id));

create table results(
    result_id int identity(1,1) primary key,
    enrolment_id int not null unique references enrolments(enrolment_id),
    finish_time time null,
    position int null,
    result_status nvarchar(20) not null default 'Finished');
go

-- seed data that needs no passwords
insert into roles(role_name) values('Organiser'),('Participant');

insert into categories(category_name, description) values
('Under 20', 'Runners younger than 20'),
('Senior', 'Open senior category'),
('10km', '10 kilometre distance'),
('21km', 'Half marathon distance');


insert into events(organiser_id, event_name, event_date, location, description, route_info, distance, event_type) values
('@org1', 'Soweto Marathon Fun Run', '2026-12-05', 'Soweto', 'Community fun run', 'Loop through Soweto', 10, 'Run'),
('@org1', 'Pretoria Cycle Challenge', '2026-12-12', 'Pretoria', 'Charity cycling event', 'Union Buildings route', 40, 'Cycle'),
('@org2', 'Joburg Park Walk', '2026-12-19', 'Johannesburg', 'Family walk in the park', 'Zoo Lake circuit', 5, 'Walk');

insert into event_categories(event_id, category_id, entry_fee, max_participants)
select e.event_id, c.category_id, 100, 200
from events e cross join categories c
where c.category_name in ('Senior', '10km');

insert into enrolments(user_id, event_id, event_category_id)
select @par1, ec.event_id, ec.event_category_id
from event_categories ec join categories c on c.category_id = ec.category_id
where c.category_name = 'Senior';

insert into enrolments(user_id, event_id, event_category_id)
select @par2, ec.event_id, ec.event_category_id
from event_categories ec join categories c on c.category_id = ec.category_id
where c.category_name = '10km';



select * from categories;