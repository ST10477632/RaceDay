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


insert into roles(role_name) values('Organiser'),('Participant');

insert into categories(category_name, description) values
('Under 20', 'Runners younger than 20'),
('Senior', 'Open senior category'),
('10km', '10 kilometre distance'),
('21km', 'Half marathon distance');
