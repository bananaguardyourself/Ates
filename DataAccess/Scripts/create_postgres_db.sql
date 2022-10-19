CREATE TABLE public.applicationusers
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    publicid uuid not null,
    name text not null,
    role text not null,

    PRIMARY KEY (id)
);

CREATE TABLE public.tasks
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    publicid uuid not null,
    publicuserid uuid not null,
    tasktitle text not null,
    taskjiraid text not null,
    taskdescription text not null,
    taskstatus int not null,
    LastUpdated timestamp without time zone not null,    						

     PRIMARY KEY (id)
);

CREATE TABLE public.userdeadletters
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    message text not null,
    timereceived timestamp without time zone not null,    						

    PRIMARY KEY (id)
);