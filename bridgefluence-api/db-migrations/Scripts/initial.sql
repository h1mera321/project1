--liquibase formatted sql
--changeset andrey:initial.sql

create table if not exists publisher
(
    id               integer not null
        primary key
        unique,
    name             text    not null,
    payment_qr       text,
    subscriber_count integer,
    geo              jsonb
);

alter table publisher
    owner to postgres;

create table if not exists brand
(
    id       integer not null
        primary key,
    title    text    not null,
    brief    text,
    website  text,
    hashtags text[] default '{}'::text[]
);

alter table brand
    owner to postgres;

create table if not exists post
(
    id                serial
        primary key
        unique,
    paid              boolean default false,
    requested_price   integer not null,
    publisher_id      integer not null
        constraint post_publisher_fk
            references publisher,
    brand_id          integer not null
        constraint post_brand_fk
            references brand,
    postponed         boolean default false,
    rejected          boolean default false,
    post_id           integer not null,
    rejection_reasons integer[],
    paid_price        integer,
    submitted_at date NOT NULL DEFAULT now(),
    postponed_at date,
    rejected_at date,
    paid_at date
);

alter table post
    owner to postgres;