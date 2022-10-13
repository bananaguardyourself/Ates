CREATE TABLE public.applicationusers
(
    id int NOT NULL GENERATED AS IDENTITY,
    publicid uuid not null,
    name text not null,
    role text not null,

    PRIMARY KEY (id)
);

CREATE TABLE public.tasks
(
    id int NOT NULL GENERATED AS IDENTITY,
    publicid uuid not null,
    userid uuid not null,
    taskname text not null,
    taskdescription text not null,
    taskstatus int not null,
    LastUpdated timestamp without time zone not null,    						

     PRIMARY KEY (id)
);