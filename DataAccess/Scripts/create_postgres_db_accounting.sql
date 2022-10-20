CREATE TABLE public.accountingusers
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    publicid uuid not null,
    name text not null,
    role text not null,
    balance int NOT NULL,
    lastupdated timestamp without time zone not null,

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
    taskcostassign int not null,
    taskcostcomplete int not null,

    PRIMARY KEY (id)
);

CREATE TABLE public.accountingtransactions
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    publicid uuid not null,
    publicuserid uuid not null,
    publictaskid uuid,
    action string not null,
    increase int not null,
    decrease int not null,
    createdat timestamp without time zone not null,

    PRIMARY KEY (id)
);

CREATE TABLE public.userdeadletters
(
    id int NOT NULL GENERATED ALWAYS AS IDENTITY,
    message text not null,
    timereceived timestamp without time zone not null,    						

    PRIMARY KEY (id)
);